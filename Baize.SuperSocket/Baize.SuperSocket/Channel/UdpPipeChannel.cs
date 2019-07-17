using Baize.SuperSocket.Primitives;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Baize.SuperSocket.Channel
{
    /// <summary>
    /// UDP协议通道
    /// </summary>
    public class UdpPipeChannel : PipeChannel
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket">socket对象</param>
        /// <param name="endPoint">远程地址</param>
        /// <param name="options">通道选项</param>
        /// <param name="logger">日志</param>
        public UdpPipeChannel(Socket socket,EndPoint endPoint, ChannelOptions options, ILogger logger)
            : base(socket,options, logger)
        {
            this.RemoteIPEndPoint = endPoint;
        }

        /// <summary>
        /// socket底层发送数据
        /// </summary>
        /// <param name="buffer"></param>
        private int SendData(ReadOnlyMemory<byte> buffer)
        {
            int rtn = 1;
          

          
           
            return rtn;
        }

       
        /// <summary>
        /// 主动关闭会话
        /// </summary>
        public override void Close()
        {
        }

        /// <summary>
        /// 具体管道实现的socket数据发送方法
        /// </summary>
        /// <param name="buffer">发送命令数据</param>
        /// <returns></returns>
        public override async ValueTask<int> Send(ReadOnlyMemory<byte> buffer)
        {
            Task<int> task =  this.Socket.SendToAsync(GetArrayByMemory(buffer), SocketFlags.None, this.RemoteIPEndPoint);
            return await Task.FromResult(task.Result);
        }

    }
}
