using Baize.SuperSocket.Channel;
using Baize.SuperSocket.Primitives;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Baize.SuperSocket.Server
{
    /// <summary>
    /// TCP协议监听类
    /// </summary>
    public class TcpSocketListener : IListener
    {
        /// <summary>
        /// socket监听对象
        /// </summary>
        private Socket _listenSocket;
        /// <summary>
        /// 取消操作标志
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;
       /// <summary>
       /// 停止任务
       /// </summary>
        private TaskCompletionSource<bool> _stopTaskCompletionSource;
        /// <summary>
        /// 通道创建工厂
        /// </summary>
        private readonly Func<Socket, IChannel> _channelFactory;
        /// <summary>
        /// 日志
        /// </summary>
        private ILogger _logger;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">监听选项</param>
        /// <param name="channelFactory">通道工厂</param>
        /// <param name="logger">日志</param>
        public TcpSocketListener(ListenOptions options, Func<Socket, IChannel> channelFactory, ILogger logger)
        {
            Options = options;
            _channelFactory = channelFactory;
            _logger = logger;
        }
        /// <summary>
        /// 获得监听IPEndPoint
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        private IPEndPoint GetListenEndPoint(string ip, int port)
        {
            IPAddress ipAddress;

            if ("any".Equals(ip, StringComparison.OrdinalIgnoreCase))
            {
                ipAddress = IPAddress.Any;
            }
            else if ("IpV6Any".Equals(ip, StringComparison.OrdinalIgnoreCase))
            {
                ipAddress = IPAddress.IPv6Any;
            }
            else
            {
                ipAddress = IPAddress.Parse(ip);
            }

            return new IPEndPoint(ipAddress, port);
        }
        /// <summary>
        /// 接收连接
        /// </summary>
        /// <param name="listenSocket">监听socket</param>
        /// <returns></returns>
        private async Task KeepAccept(Socket listenSocket)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var client = await listenSocket.AcceptAsync();
                    OnNewClientAccept(client);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
            _stopTaskCompletionSource.TrySetResult(true);
        }
        /// <summary>
        /// 调用新连接建立事件
        /// </summary>
        /// <param name="socket">接入的socket对象</param>
        private void OnNewClientAccept(Socket socket)
        {
            var handler = NewClientAccepted;
            handler?.Invoke(_channelFactory(socket));
        }
        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        public  bool Start()
        {
            var options = Options;
            try
            {
                var listenEndpoint = GetListenEndPoint(options.Ip, options.Port);
                var listenSocket = _listenSocket = new Socket(listenEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                listenSocket.LingerState = new LingerOption(false, 0);
                if (options.NoDelay)
                    listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                listenSocket.Bind(listenEndpoint);
                listenSocket.Listen(options.BackLog);
                IsRunning = true;
                _cancellationTokenSource = new CancellationTokenSource();
                KeepAccept(listenSocket).DoNotAwait(); ;
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "The listener failed to start.");
                return false;
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        /// <returns></returns>
        public Task StopAsync()
        {
            _stopTaskCompletionSource = new TaskCompletionSource<bool>();
            _cancellationTokenSource.Cancel();
            _listenSocket.Close();
            return _stopTaskCompletionSource.Task;
        }
        #region 属性
        /// <summary>
        /// 监听选项
        /// </summary>
        public ListenOptions Options
        {
            get;
        }
        /// <summary>
        /// 是否运行
        /// </summary>
        public bool IsRunning
        {
            get;
            private set;
        }
        /// <summary>
        /// 监听服务对象
        /// </summary>
        public Socket Socket
        {
            get
            {
                return _listenSocket;
            }
        }
        #endregion
        #region 事件
        public event NewClientAcceptHandler NewClientAccepted;
        #endregion
    }
}
