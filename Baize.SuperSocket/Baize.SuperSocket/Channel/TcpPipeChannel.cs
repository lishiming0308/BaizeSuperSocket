using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.Primitives;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Baize.SuperSocket.Channel
{
    /// <summary>
    /// TCP通道
    /// </summary>
    public class TcpPipeChannel : PipeChannel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket">socket对象</param>
        /// <param name="options">通道选项</param>
        /// <param name="logger">日志</param>
        public TcpPipeChannel(Socket socket, ChannelOptions options, ILogger logger)
            : base(socket,options, logger)
        {
            this.RemoteIPEndPoint = socket.RemoteEndPoint;
        }
        /// <summary>
        /// 从Socket对象接收数据
        /// </summary>
        /// <param name="socket">sokcet镀锡</param>
        /// <param name="memory">字节内存</param>
        /// <param name="socketFlags">socket选项</param>
        /// <returns></returns>
        private async Task<int> ReceiveAsync(Socket socket, Memory<byte> memory, SocketFlags socketFlags)
        {
            var memory1 = (ReadOnlyMemory<byte>)memory;
            ArraySegment<byte> buffer = GetArrayByMemory(memory1);
            return await socket.ReceiveAsync(buffer, socketFlags);
        }
        /// <summary>
        /// 接收TCP连接数据并插入管道写入器中
        /// </summary>
        /// <param name="socket">socket对象</param>
        /// <param name="writer">管道写入器</param>
        /// <returns></returns>
        private async Task FillPipeAsync(Socket socket, PipeWriter writer)
        {
            var options = Options;
            try
            {
                while (true)
                {
                    try
                    {
                        var bufferSize = options.ReceiveBufferSize;
                        var maxPackageLength = options.MaxPackageLength;
                        if (maxPackageLength > 0)
                            bufferSize = Math.Min(bufferSize, maxPackageLength);
                        var memory = writer.GetMemory(bufferSize);
                        var bytesRead = await ReceiveAsync(socket, memory, SocketFlags.None);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        // Tell the PipeWriter how much was read
                        writer.Advance(bytesRead);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Exception happened in ReceiveAsync");
                        break;
                    }
                    // Make the data available to the PipeReader
                    var result = await writer.FlushAsync();
                    if (result.IsCompleted)
                    {
                        break;
                    }
                }
                // Signal to the reader that we're done writing
                writer.Complete();
                Output.Writer.Complete();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }
        /// <summary>
        /// 处理通道接收数据
        /// </summary>
        /// <returns></returns>
        protected override async Task<bool> ProcessReads()
        {
            bool rtn = false;
            var pipe = new Pipe(new PipeOptions(
          pauseWriterThreshold: this.Options.MaxPackageLength));
            Task writing = FillPipeAsync(this.Socket, pipe.Writer);
            Task reading = ReadPipeAsync(pipe.Reader);
            await Task.WhenAll(reading, writing);
            rtn = true;
            return rtn;
        }
        /// <summary>
        /// 主动关闭当前通道中的sokcet
        /// </summary>
        public override void Close()
        {
            try
            {
                var _socket = this.Socket;
                var socket = this.Socket;
                if (socket == null)
                    return;
                if (Interlocked.CompareExchange(ref _socket, null, socket) == socket)
                {
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both);
                    }
                    finally
                    {
                        socket.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        /// <summary>
        /// 通道被动关闭事件
        /// </summary>
        /// <param name="closeReason">关闭原因</param>
        protected override void OnClosed(CloseReason closeReason)
        {
            this.Socket = null;
            base.OnClosed(closeReason);
        }
        /// <summary>
        /// 将要发送个Socket的数据推入发送管道等待发送
        /// </summary>
        /// <param name="buffer">发送数据</param>
        /// <returns></returns>
        public override async ValueTask<int> Send(ReadOnlyMemory<byte> buffer)
        {
            try
            {
                if (this.Socket !=null && this.Socket.Connected && buffer.Length > 0)
                {
                   return await this.Socket.SendAsync(buffer, SocketFlags.None);
                }
                else
                {
                   return await Task.FromResult<int>(-1);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return await Task.FromResult<int>(-1);
            }
        }
    }
}
