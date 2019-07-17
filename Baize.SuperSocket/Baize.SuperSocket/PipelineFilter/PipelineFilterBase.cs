using System;
using System.Buffers;
using Baize.SuperSocket.ProtoBase;
using Baize.SuperSocket.BuffersExtensions;
using Baize.SuperSocket.Primitives;
using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using System.Collections.Generic;

namespace Baize.SuperSocket.PipelineFilter
{
    public abstract class PipelineFilterBase : IFilter
    {
        protected FilterInfo _filterInfo;
        /// <summary>
        /// 接收数据链表头
        /// </summary>
        protected BaizeBufferSegment _head = null;
        /// <summary>
        /// 接收数据链表尾
        /// </summary>
        protected BaizeBufferSegment _tail = null;
        /// <summary>
        /// 接收的缓存数据字节数
        /// </summary>
        protected long _bufferDataCount = 0;
        /// <summary>
        /// 通道选项
        /// </summary>
        protected ChannelOptions _channelOptions;
        /// <summary>
        /// 过滤信息
        /// </summary>
        public FilterInfo FilterInfo
        {
            get
            {
                return _filterInfo;
            }
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="data">填充数据</param>
        public void FillData(ReadOnlySequence<byte> data)
        {
            _bufferDataCount += data.Length;
            foreach (var memory in data)
            {
                if (_head == null)
                {
                    _head = _tail = new BaizeBufferSegment();
                    _tail.SetUnownedMemory(memory);
                }
                else
                {
                    BaizeBufferSegment next = new BaizeBufferSegment();
                    next.SetUnownedMemory(memory);
                    _tail.SetNext(next);
                    _tail = next;
                }
            }
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _head.ResetMemory();
            _head = null;
            _bufferDataCount = 0;
        }
        public virtual bool FilterData(BaizeSession baizeSession, ReadOnlySequence<byte> data)
        {
            bool rtn = true;
            FillData(data);
            rtn = Filter(baizeSession);
            if (rtn || _bufferDataCount > _channelOptions.MaxPackageLength)
            {
                Reset();
            }
            return rtn;
        }
        public abstract bool Filter(BaizeSession baizeSession);

       
    }
}
