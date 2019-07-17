using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.Primitives;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Baize.SuperSocket.Channel
{
    /// <summary>
    /// Socket管道通道基础服务,此基础管道负责Socket发送命令发送功能,和重此基础管道读取插入管道的数据
    /// </summary>
    public abstract class PipeChannel : IChannel
    { 
        private Action<IChannel, CloseReason> _closed;
        private Func<ReadOnlySequence<byte>, ValueTask> _packageReceived;
        /// <summary>
        /// 高性能管道
        /// </summary>
        protected Pipe Output { get; }
        /// <summary>
        /// 日志
        /// </summary>
        protected ILogger Logger { get; }
        /// <summary>
        /// 管道选项
        /// </summary>
        protected ChannelOptions Options { get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket">socket对象</param>
        /// <param name="options">管道选项</param>
        /// <param name="logger">日志</param>
        protected PipeChannel(Socket socket,ChannelOptions options, ILogger logger)
        {
            this.Socket = socket;
            Options = options;
            Logger = logger;
            Output = new Pipe();
        }
        /// <summary>
        /// 调用接收数据包事件
        /// </summary>
        /// <param name="package">通道接收数据包</param>
        /// <returns></returns>
        protected async ValueTask OnPackageReceived(ReadOnlySequence<byte> package)
        {
            if (_packageReceived != null)
            {
                await _packageReceived.Invoke(package);
            }
        }
        /// <summary>
        /// 调用通道关闭事件
        /// </summary>
        /// <param name="closeReason">通道关闭原因</param>
        protected virtual void OnClosed(CloseReason closeReason)
        {
            _closed?.Invoke(this, closeReason);
        }    
        /// <summary>
        /// 处理管道接收的Socket数据,此方法由具体管道实现，具体管道分为tcp和udp管道
        /// </summary>
        /// <returns></returns>

        protected virtual Task<bool> ProcessReads()
        {
            return Task.FromResult(false);
        }      
        /// <summary>
        /// 将内存数据换回为数组端
        /// </summary>
        /// <typeparam name="T">内存数据类型</typeparam>
        /// <param name="memory">内存数据</param>
        /// <returns></returns>
        protected internal ArraySegment<T> GetArrayByMemory<T>(ReadOnlyMemory<T> memory)
        {
            if (!MemoryMarshal.TryGetArray(memory, out var result))
            {
                throw new InvalidOperationException("Buffer backed by array was expected");
            }
            return result;
        }
        /// <summary>
        /// 读取管道数据
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected async Task ReadPipeAsync(PipeReader reader)
        {
            while (true)
            {
                try
                {
                    ReadResult result = await reader.ReadAsync();
                    ReadOnlySequence<byte> buffer = result.Buffer;
                    if (buffer.Length > 0)
                    {
                        var package = buffer.Slice(buffer.Start);
                        await OnPackageReceived(package);
                        buffer = buffer.Slice(buffer.End);
                        // We sliced the buffer until no more data could be processed
                        // Tell the PipeReader how much we consumed and how much we left to process
                        reader.AdvanceTo(buffer.Start, buffer.End);   
                    }
                    if (result.IsCompleted)
                    {
                        break;
                    }
                }
                catch(Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            }
            reader.Complete();
        }
        #region 接口函数
        /// <summary>
        /// 启动收据收发任务
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> StartAsync()
        {
            var rtn = false;
            try
            {
                rtn = await ProcessReads();
                if (rtn)
                {
                    OnClosed(CloseReason.ServerShutdown);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unhandled exception in the method PipeChannel.StartAsync.");
            }
            return rtn;
        }
        /// <summary>
        /// 发送命令数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <returns></returns>
        public abstract ValueTask<int> Send(ReadOnlyMemory<byte> buffer);
        public abstract void Close();
        #endregion
        #region 属性
        public virtual Socket Socket { get; set; }
        public virtual EndPoint RemoteIPEndPoint { get; set; }
        #endregion
        #region 事件
        /// <summary>
        /// 通道接收数据包事件
        /// </summary>
        public event Func<ReadOnlySequence<byte>, ValueTask> PackageReceived
        {
            add => _packageReceived += value;
            remove => _packageReceived -= value;
        }
        /// <summary>
        /// 通道关闭事件
        /// </summary>
        public event Action<IChannel, CloseReason> Closed
        {
            add => _closed += value;
            remove => _closed -= value;
        }
        #endregion
    }
}
