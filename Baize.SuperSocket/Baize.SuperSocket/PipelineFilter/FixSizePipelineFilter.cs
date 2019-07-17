using System.Buffers;
using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.BuffersExtensions;
using Baize.SuperSocket.Primitives;
using Baize.SuperSocket.ProtoBase;

namespace Baize.SuperSocket.PipelineFilter
{
    /// <summary>
    /// 固定报文长度过滤器
    /// </summary>
    public class FixSizePipelineFilter : PipelineFilterBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="channelOptions">通道选项</param>
        public FixSizePipelineFilter(ChannelOptions channelOptions, FilterInfo filterInfo)
        {
            this._channelOptions = channelOptions;
            this._filterInfo = filterInfo;
        }
        public override bool Filter(BaizeSession baizeSession)
        {
            bool rtn = false;
            int size = this._filterInfo.BasePortocalFilterInfo.Size;
            ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(_head, 0, _tail, _tail.End);
            if (sequence.IsSingleSegment)
            {
                if (sequence.Length >= size)
                {
                    ReadOnlySequence<byte> readOnly = new ReadOnlySequence<byte>(sequence.First.Slice(0, size));
                    baizeSession.Data = readOnly;
                    rtn = true;
                }
            }
            else
            {
                rtn = MultiSegmentHandler(baizeSession, sequence);
            }
            return rtn;
        }
        /// <summary>
        /// 多数据段处理
        /// </summary>
        /// <param name="baizeSession">会话</param>
        /// <param name="sequence">数据</param>
        /// <returns></returns>
        private bool MultiSegmentHandler(BaizeSession baizeSession, ReadOnlySequence<byte> sequence)
        {
            bool rtn = false;
            int size= this._filterInfo.BasePortocalFilterInfo.Size;
            BaizeBufferSegment tmpHead = null, tmpTail = null;
            int tmpHeadIndex = 0;
            int tmpTailIndex = 0;
            if (sequence.First.Length >= size)
            {
                ReadOnlySequence<byte> readOnly = new ReadOnlySequence<byte>(sequence.First.Slice(0, size));
                baizeSession.Data = readOnly;
            }
            else
            {
                int length = 0;
                foreach (var memory in sequence)
                {
                    if (tmpHead == null)
                    {
                        tmpHead = tmpTail = new BaizeBufferSegment();
                        tmpTail.SetUnownedMemory(memory);
                    }
                    else
                    {
                        BaizeBufferSegment next = new BaizeBufferSegment();
                        next.SetUnownedMemory(memory);
                        tmpTail.SetNext(next);
                        tmpTail = next;
                    }
                    if (length + memory.Length >= size)
                    {
                        tmpTailIndex = size - length;
                        rtn = true;
                        break;

                    }
                    length += memory.Length;
                }
                if (rtn)
                {
                    ReadOnlySequence<byte> readOnly = new ReadOnlySequence<byte>(tmpHead, tmpHeadIndex, tmpTail, tmpTailIndex);
                    baizeSession.Data = readOnly;
                }
                else
                {
                    if (tmpHead != null)
                    {
                        tmpHead.ResetMemory();
                        tmpHead = null;
                    }
                }
            }
            return rtn;
        }
    }
}
