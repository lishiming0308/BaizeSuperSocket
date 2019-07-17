using Baize.SuperSocket.Primitives;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Baize.SuperSocket.Channel;
using System.Buffers;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Concurrent;
using Baize.SuperSocket.PipelineFilter;
using System.Net.Sockets;
using System.Net;
using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;

namespace Baize.SuperSocket.Server
{
    class SuperSocketService : IHostedService, IServer
    {
        /// <summary>
        /// 服务选项
        /// </summary>
        private readonly IOptions<ServerOptions> _serverOptions;
        /// <summary>
        /// 日志工厂
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// 过滤条件工厂
        /// </summary>
        private readonly IPipelineFilterFactory _pipelineFilterFactory;
        /// <summary>
        /// 过滤条件列表
        /// </summary>
        private List<IFilter> _pipelineFilters;
        /// <summary>
        /// 监听工厂
        /// </summary>
        private List<IListenerFactory> _listenerFactorys;
        /// <summary>
        /// 监听端口列表
        /// </summary>
        private List<IListener> _listeners;
        /// <summary>
        /// 当前服务中的所有会话
        /// </summary>
        private ConcurrentDictionary<string, IAppSession> _appSessions;
        /// <summary>
        /// 接收数据包
        /// </summary>
        private Func<IServer,List<string>,BaizeSession, Task> _packageHandler;
        /// <summary>
        /// 会话创建
        /// </summary>
        private Func<IServer,BaizeSession, ValueTask> _sessionConnectedHandler;
        /// <summary>
        /// 会话关闭
        /// </summary>
        private Func<IServer,BaizeSession, CloseReason, ValueTask> _sessionClosedHandler;
        /// <summary>
        /// 空连接清理定时器
        /// </summary>
        private System.Threading.Timer m_ClearIdleSessionTimer = null;
        /// <summary>
        /// 会话数据量
        /// </summary>
        private int _sessionCount;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="serverOptions"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="pipelineFilterFactory"></param>

        public SuperSocketService(IServiceProvider serviceProvider, IOptions<ServerOptions> serverOptions, ILoggerFactory loggerFactory)
        {
            _serverOptions = serverOptions;
            Name = serverOptions.Value.Name;
            _serverOptions = serverOptions;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger("BaizeSuperSocketService");
            _pipelineFilterFactory = new DefaultPipelineFilterFactory(serverOptions.Value.FilterDataOptions);
            _pipelineFilters = _pipelineFilterFactory.Create(serverOptions.Value.ProductInfos);
            _appSessions = new ConcurrentDictionary<string, IAppSession>();
            _listenerFactorys = new List<IListenerFactory>();
            _listenerFactorys.Add(new TcpSocketListenerFactory());
            _listenerFactorys.Add(new UdpSocketListenerFactory());
            _packageHandler = serviceProvider.GetService<Func<IServer,List<string>, BaizeSession, Task>>();
            _sessionConnectedHandler = serviceProvider.GetService<Func<IServer, BaizeSession, ValueTask>>();
            _sessionClosedHandler = serviceProvider.GetService<Func<IServer, BaizeSession, CloseReason, ValueTask>>();
            StartClearSessionTimer();
        }
        /// <summary>
        /// 清理空闲会话
        /// </summary>
        private void StartClearSessionTimer()
        {
            int interval = _serverOptions.Value.IdleTime * 1000;//in milliseconds
            m_ClearIdleSessionTimer = new System.Threading.Timer(ClearIdleSession, new object(), 0, interval);
        }

        /// <summary>
        /// 清理会话
        /// </summary>
        /// <param name="state">The state.</param>
        private void ClearIdleSession(object state)
        {
            if (_serverOptions.Value.IsTimeout)
            {
                if (Monitor.TryEnter(state))
                {
                    try
                    {
                        DateTime now = DateTime.Now;
                        DateTime timeOut = now.AddSeconds(0 - _serverOptions.Value.IdleTime);
                        var timeOutSessions = _appSessions.Where(s => s.Value.Session.LastTime <= timeOut).Select(s => s.Value);
                        System.Threading.Tasks.Parallel.ForEach(timeOutSessions, s =>
                        {
                            if (_logger.IsEnabled(LogLevel.Information))
                                _logger.LogInformation($"The session will be closed for {now.Subtract(s.Session.LastTime).TotalSeconds} timeout, the session start time: { s.Session.StartTime}, last active time: {s.Session.LastTime}!");
                            s.Close();
                        });
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                            _logger.LogError("Clear idle session error!", e);
                    }
                    finally
                    {
                        Monitor.Exit(state);
                    }
                }
            }
        }
        /// <summary>
        /// 启动监听
        /// </summary>
        /// <returns></returns>
        private Task<bool> StartListenAsync()
        {
            _listeners = new List<IListener>();
            var serverOptions = _serverOptions.Value;
            foreach (ListenOptions listenOptions in serverOptions.Listeners)
            {
                IListenerFactory _listenerFactory = _listenerFactorys[0];
                if (listenOptions.ProtocolType == Baize.IPlugin.SuperSocket.ProtocolType.UDP)
                {
                    _listenerFactory = _listenerFactorys[1];
                }
                var listener = _listenerFactory.CreateListener(_appSessions, listenOptions, _loggerFactory);
                listener.NewClientAccepted += OnNewClientAccept;
                if (listener is UdpSocketListener)
                {
                    UdpSocketListener udpSocketListener = (UdpSocketListener)listener;
                    udpSocketListener.UDPData += Listener_UDPDataHandler;
                }
                if (!listener.Start())
                {
                    _logger.LogError($"Failed to listen {listener}.");
                }
                _listeners.Add(listener);
            }
            return Task.FromResult(true);
        }
        /// <summary>
        /// 接收UDP数据
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="endPoint"></param>
        private async ValueTask Listener_UDPDataHandler(Memory<byte> memory, System.Net.EndPoint endPoint)
        {
            if (_appSessions.TryGetValue(endPoint.ToString(), out var appSession))
            {
                var readOnlymemory = (ReadOnlyMemory<byte>)memory;
                await FilterData(appSession, new ReadOnlySequence<byte>(readOnlymemory));
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="cancellationToken">启动服务选项</param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await StartListenAsync();
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="cancellationToken">停止选项</param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var tasks = _listeners.Where(l => l.IsRunning).Select(l => l.StopAsync()).ToArray();
            await Task.WhenAll(tasks);
            _sessionCount = 0;
            foreach (AppSession appSession in _appSessions.Values)
            {
                appSession.Close();
            }
            _appSessions.Clear();
        }
        /// <summary>
        /// 过滤数据
        /// </summary>
        /// <param name="appSession"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async ValueTask FilterData(IAppSession appSession, ReadOnlySequence<byte> data)
        {
            try
            {
                appSession.Session.LastTime = DateTime.Now;
                appSession.Session.Data = data;
                if (_pipelineFilters.Count > 0)
                {
                    foreach (IFilter pipelineFilter in _pipelineFilters)
                    {
                        bool result = pipelineFilter.FilterData(appSession.Session, data);
                        if (result)
                        {
                            await _packageHandler(this,pipelineFilter.FilterInfo.ProductOIDS, appSession.Session);
                        }
                    }
                }
                else
                {
                    await _packageHandler(this,new List<string>(), appSession.Session);
                }
            }
            catch (Exception e)
            {
                OnSessionError(appSession, e);
            }
        }
        /// <summary>
        /// 新连接
        /// </summary>
        /// <param name="appSession">会话</param>
        /// <returns></returns>
        private async Task SessionConnectedHandler(AppSession appSession)
        {
            var connectedHandler = _sessionConnectedHandler;
            if (connectedHandler != null)
            {
               await  _sessionConnectedHandler(this, appSession.Session);
            }
        }
        /// <summary>
        /// 处理会话
        /// </summary>
        /// <param name="appSession">会话信息</param>
        /// <returns></returns>
        private async Task HandleSession(AppSession appSession)
        {
            try
            {
                var packageHandler = _packageHandler;
                if (packageHandler != null)
                {
                    appSession.Channel.PackageReceived += async (data) =>
                    {
                        await FilterData(appSession, data);
                    };
                    appSession.Channel.Closed += async (IChannel channel, CloseReason closeReason) =>
                    {
                     
                        if (_sessionClosedHandler != null)
                        {
                            await _sessionClosedHandler(this, appSession.Session, closeReason);
                        }
                    };
                }
                Interlocked.Increment(ref _sessionCount);
                bool run = true;
                _logger.LogInformation($"DateTime:{DateTime.Now},A new session connected: {appSession.Session.SessionID},会话数量:{_sessionCount}");
                _appSessions.TryAdd(appSession.Session.SessionID, appSession);
                await SessionConnectedHandler(appSession);
                run = await appSession.Channel.StartAsync();
                if (run)
                {
                    _appSessions.TryRemove(appSession.Session.SessionID, out var appSession1);
                    Interlocked.Decrement(ref _sessionCount);
                }
                _logger.LogInformation($"DateTime:{DateTime.Now}The session disconnected: {appSession.Session.SessionID},会话数量:{_sessionCount}");        
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to handle the session {appSession.Session.SessionID}.", e);
            }
           
        }
        /// <summary>
        /// 接收新连接
        /// </summary>
        /// <param name="channel">通道</param>
        protected virtual async void OnNewClientAccept(IChannel channel)
        {
            var appSession = new AppSession(channel);
            await HandleSession(appSession);
         
        }
        /// <summary>
        /// 会话错误处理
        /// </summary>
        /// <param name="appSession">会话</param>
        /// <param name="exception">异常</param>
        protected virtual void OnSessionError(IAppSession appSession, Exception exception)
        {
            _logger.LogError($"Session[{appSession.Session.SessionID}]: session exception.", exception);
        }     
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        async Task<bool> IServer.StartAsync()
        {
            await StartAsync(CancellationToken.None);

            return true;
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        async Task IServer.StopAsync()
        {
            await StopAsync(CancellationToken.None);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sessionID">会话ID</param>
        /// <param name="buffer">数据</param>
        /// <returns></returns>
        public async ValueTask<int> SendDataAsync(string sessionID, ReadOnlyMemory<byte> buffer)
        { 
            if(_appSessions.TryGetValue(sessionID,out var appSession))
            {
              return  await appSession.Channel.Send(buffer);  
            }
            else
            {
              return  await Task.FromResult<int>(-1);
            }
        }
        /// <summary>
        /// 连接远程地址
        /// </summary>
        /// <param name="ip">远程IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="protocolType">协议类型</param>
        /// <returns>连接成功返回会话，失败返回空</returns>
        public async ValueTask<BaizeSession> CreateClientAsync(string ip, int port, Baize.IPlugin.SuperSocket.ProtocolType protocolType)
        {
            BaizeSession rtn = null;
            try
            {
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                if (protocolType == Baize.IPlugin.SuperSocket.ProtocolType.TCP)
                {
                    var socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                    await socket.ConnectAsync(iPEndPoint);
                    TcpPipeChannel channel = new TcpPipeChannel(socket, _serverOptions.Value.FilterDataOptions, _logger);
                    var appSession = new AppSession(channel);
                    rtn = appSession.Session;
                    HandleSession(appSession).DoNotAwait();
                }
                else
                {
                    var listener = _listeners.Where(i => i.Socket.ProtocolType == System.Net.Sockets.ProtocolType.Udp).FirstOrDefault();
                    if(listener != null)
                    {
                        UdpPipeChannel channel = new UdpPipeChannel(listener.Socket, iPEndPoint, _serverOptions.Value.FilterDataOptions, _logger);
                        var appSession = new AppSession(channel);
                        rtn = appSession.Session;
                        HandleSession(appSession).DoNotAwait();
                    }      
                }          
                return rtn;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to connect to {ip}:{port}", e);
                return null;
            }
        }
        /// <summary>
        /// 关闭会话
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public bool CloseSession(string sessionID)
        {
            bool rtn = false;
            if(_appSessions.TryRemove(sessionID,out var appSession))
            {
                rtn = true;
                appSession.Close();
                Interlocked.Decrement(ref _sessionCount);
            }
            return rtn;  
        }
        /// <summary>
        /// 返回会话的连接状态
        /// </summary>
        /// <param name="sessionID">会话ID</param>
        /// <returns>会话为TCP会话是返回的是会话Socket的连接状态,会话为UDP时返回是当前会话是否存在</returns>
        public bool IsSessionConnected(string sessionID)
        {
            bool rtn = false;
            if(_appSessions.TryGetValue(sessionID,out var appSession))
            {
                rtn = true;
                if(appSession.Session.ProtocolType== IPlugin.SuperSocket.ProtocolType.TCP)
                {
                    rtn = appSession.Channel.Socket.Connected;
                }
            }
            return rtn;
        }
        #region 属性
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 会话数量
        /// </summary>
        public int SessionCount => _sessionCount;
        /// <summary>
        /// 当前会话字典
        /// </summary>
        private ConcurrentDictionary<string, IAppSession> AppSessions
        {
            get
            {
                return _appSessions;
            }
        }

        #endregion
    }
}
