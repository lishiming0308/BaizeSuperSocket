using Baize.SuperSocket.Channel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
namespace Baize.SuperSocket.Primitives
{
    /// <summary>
    /// 新连接
    /// </summary>
    /// <param name="channel">根据当前连接创建新通道</param>
    public delegate void NewClientAcceptHandler(IChannel channel);
    /// <summary>
    /// UDP数据包
    /// </summary>
    /// <param name="memory">数据</param>
    /// <param name="endPoint">远程地址</param>
    public delegate ValueTask UDPDataHandler(Memory<byte> memory, EndPoint endPoint);

    /// <summary>
    /// Socket监听接口
    /// </summary>
    public interface IListener
    {
        /// <summary>
        /// 监听选项
        /// </summary>
        ListenOptions Options { get; }
        /// <summary>
        /// 启动监听
        /// </summary>
        /// <returns></returns>
        bool Start();
        /// <summary>
        /// 接收连接
        /// </summary>
        event NewClientAcceptHandler NewClientAccepted;
        /// <summary>
        /// 停止监听
        /// </summary>
        /// <returns></returns>
        Task StopAsync();
        /// <summary>
        /// 是否监听
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// 服务监听对象
        /// </summary>
        Socket Socket { get; }
    }
}
