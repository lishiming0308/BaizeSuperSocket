using Baize.SuperSocket.Channel;
using Baize.SuperSocket.Primitives;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    /// UDP协议监听类
    /// </summary>
    public class UdpSocketListener : IListener
    {

        /// <summary>
        /// socket监听对象
        /// </summary>
        private Socket _listenSocket;
        /// <summary>
        /// 接收socket数据事件参数
        /// </summary>
        private SocketAsyncEventArgs _receiveSAE;
        /// <summary>
        /// 取消操作标志
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;
        /// <summary>
        /// 
        /// </summary>
        private TaskCompletionSource<bool> _stopTaskCompletionSource;
        /// <summary>
        /// 通道创建工厂
        /// </summary>
        private readonly Func<Socket,EndPoint, IChannel> _channelFactory;
        /// <summary>
        /// 日志
        /// </summary>
        private ILogger _logger;
   
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options"></param>
        /// <param name="channelFactory"></param>
        /// <param name="logger"></param>
        public UdpSocketListener(ListenOptions options, Func<Socket, EndPoint, IChannel> channelFactory, ILogger logger)
        {
            Options = options;
            _channelFactory = channelFactory;
            _logger = logger;
        }
       /// <summary>
       /// 接收连接事件
       /// </summary>
        public event NewClientAcceptHandler NewClientAccepted;
        public event UDPDataHandler UDPData;

        /// <summary>
        /// 获得IPendPoint
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
        /// 启动
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            try
            {
                var listenEndpoint = GetListenEndPoint(Options.Ip, Options.Port);
                var listenSocket = _listenSocket = new Socket(listenEndpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                listenSocket.Bind(listenEndpoint);
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                byte[] optionInValue = { Convert.ToByte(false) };
                byte[] optionOutValue = new byte[4];
                listenSocket.IOControl((int)SIO_UDP_CONNRESET, optionInValue, optionOutValue);
                var eventArgs = new SocketAsyncEventArgs();
                _receiveSAE = eventArgs;
                eventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(eventArgs_Completed);
                eventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                int receiveBufferSize = Options.ChannelOptions.ReceiveBufferSize <= 0 ? 2048 : Options.ChannelOptions.ReceiveBufferSize;
                var buffer = new byte[receiveBufferSize];
                eventArgs.SetBuffer(buffer, 0, buffer.Length);
                _cancellationTokenSource = new CancellationTokenSource();
                listenSocket.ReceiveFromAsync(eventArgs);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "The listener failed to start.");
                return false;
            }
        }
        /// <summary>
        /// 接收UDP会话
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void eventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError != SocketError.Success)
                {
                    var errorCode = (int)e.SocketError;
                    //The listen socket was closed
                    if (errorCode == 995 || errorCode == 10004 || errorCode == 10038)
                        return;
                    _logger.LogError($"UDP协议监听异常，异常码{errorCode}");
                }
                if (e.LastOperation == SocketAsyncOperation.ReceiveFrom)
                {
                    try
                    {
                        var handler = NewClientAccepted;
                        IChannel channel = _channelFactory(_listenSocket,e.RemoteEndPoint);
                        if (handler != null && channel != null)
                        {
                            handler.Invoke(channel);
                        }
                        if (UDPData != null)
                        {
                            UDPData.Invoke(e.MemoryBuffer.Slice(0, e.BytesTransferred), e.RemoteEndPoint);
                        }
                    }
                    catch (Exception exc)
                    {
                        _logger.LogError(exc.ToString());
                    }
                    _listenSocket.ReceiveFromAsync(e);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
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
            if (_listenSocket == null)
                return _stopTaskCompletionSource.Task;
            lock (this)
            {
                if (_listenSocket == null)
                    return _stopTaskCompletionSource.Task;
                _receiveSAE.Completed -= new EventHandler<SocketAsyncEventArgs>(eventArgs_Completed);
                _receiveSAE.Dispose();
                _receiveSAE = null;
                try
                {
                    _listenSocket.Shutdown(SocketShutdown.Both);
                    _listenSocket.Close();
                }
                catch { }
                finally
                {
                    _listenSocket = null;
                }
            }
            return _stopTaskCompletionSource.Task;
        }     
        #region 属性
        /// <summary>
        /// 监听选项
        /// </summary>
        public ListenOptions Options { get; }
        /// <summary>
        /// 只是是否运行
        /// </summary>
        public bool IsRunning { get; private set; }
        /// <summary>
        /// 监听服务Socket对象
        /// </summary>
        public Socket Socket
        {
            get
            {
                return _listenSocket;
            }
        }
        #endregion
    }
}
