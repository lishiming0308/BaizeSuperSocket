using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;
using System.Net.Sockets;
using System.Net;
using Baize.IPlugin.SuperSocket;

namespace Baize.SuperSocket.Channel
{
    /// <summary>
    /// 数据通道接口
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// 通道关联的基础Sokcet对象信息
        /// </summary>
        Socket Socket
        {
            get;
            set;
        }
        /// <summary>
        /// 远程连接地址
        /// </summary>
        EndPoint RemoteIPEndPoint
        {
            get;
            set;
        }
        /// <summary>
        /// 启动通道
        /// </summary>
        /// <returns></returns>
        Task<bool> StartAsync();
        /// <summary>
        /// 发送命令数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        ValueTask<int> Send(ReadOnlyMemory<byte> data);
        /// <summary>
        /// 通道关闭事件
        /// </summary>
        event Action<IChannel, CloseReason> Closed;
        /// <summary>
        /// 通道接收数据事件委托
        /// </summary>
        event Func<ReadOnlySequence<byte>, ValueTask> PackageReceived;

        void Close();
   
    }
  
}
