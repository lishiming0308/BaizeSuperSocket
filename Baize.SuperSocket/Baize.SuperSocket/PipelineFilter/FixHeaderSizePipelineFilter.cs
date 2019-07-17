using System.Buffers;
using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.BuffersExtensions;
using Baize.SuperSocket.Primitives;
using Baize.SuperSocket.ProtoBase;

namespace Baize.SuperSocket.PipelineFilter
{
    /// <summary>
    /// 固定头部长度过滤条件
    /// </summary>
    public class FixHeaderSizePipelineFilter : PipelineFilterBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="channelOptions">通道选项</param>
        /// <param name="filterInfo">过滤信息</param>
        public FixHeaderSizePipelineFilter(ChannelOptions channelOptions, FilterInfo filterInfo)
        {
            this._channelOptions = channelOptions;
            this._filterInfo = filterInfo;
        }
        /// <summary>
        /// 获得数据包全部长度
        /// </summary>
        /// <param name="sequence">缓存数据</param>
        /// <returns></returns>
        private int GetPacketALlLength(ReadOnlySequence<byte> sequence)
        {
            int rtn = -1;
            if (sequence.IsSingleSegment)
            {
                if (sequence.Length >= this._filterInfo.BasePortocalFilterInfo.Size)
                {
                    rtn = GetPacketLength(sequence);
                }
            }
            else
            {
                if (sequence.First.Length >= this._filterInfo.BasePortocalFilterInfo.Size)
                {
                    rtn = GetPacketLength(sequence);
                }
                else
                {
                    rtn = GetMultiSegmentPacketLength(sequence);
                }
            }
            return rtn;
        }
        /// <summary>
        /// 获得多段的包的长度
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private int GetMultiSegmentPacketLength(ReadOnlySequence<byte> sequence)
        {
            int rtn = 0;
            int length = 0;
            BaizeBufferSegment tmpHead = null, tmpTail = null;
            int tmpHeadIndex = 0;
            int tmpTailIndex = 0;
            bool complete = false;
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
                if (length + memory.Length >= this._filterInfo.BasePortocalFilterInfo.Size)
                {
                    tmpTailIndex = this._filterInfo.BasePortocalFilterInfo.Size - length;
                    complete = true;
                    break;

                }
                length += memory.Length;
            }
            if (complete)
            {
                ReadOnlySequence<byte> readOnly = new ReadOnlySequence<byte>(tmpHead, tmpHeadIndex, tmpTail, tmpTailIndex);
                if (readOnly.First.Length <= this._filterInfo.BasePortocalFilterInfo.Size)
                {
                    rtn = GetMultSegmentPacketLength(readOnly);
                }
                else
                {          
                    rtn = GetPacketLength(readOnly);
                }
            }
            else
            {
                if (tmpHead != null)
                {
                    tmpHead.ResetMemory();
                    tmpHead = null;
                }
            }
            return rtn;
        }

        /// <summary>
        /// 获得数据包长度
        /// </summary>
        /// <param name="sequence">包头数据</param>
        /// <returns></returns>
        private int GetPacketLength(ReadOnlySequence<byte> sequence)
        {
            int rtn = -1;
            ReadOnlySequence<byte> readOnly = new ReadOnlySequence<byte>(sequence.First.Slice(0, this._filterInfo.BasePortocalFilterInfo.Size));
            int bodyLength = this._filterInfo.BasePortocalFilterInfo.GetBodyLengthFromHeader(readOnly);
            rtn = this._filterInfo.BasePortocalFilterInfo.Size + bodyLength;
            return rtn;
        }
        /// <summary>
        /// 获得数据包长度
        /// </summary>
        /// <param name="sequence">包头数据</param>
        /// <returns></returns>
        private int GetMultSegmentPacketLength(ReadOnlySequence<byte> sequence)
        {
            int rtn;
            ReadOnlySequence<byte> readOnly = new ReadOnlySequence<byte>(sequence.Slice(0, this._filterInfo.BasePortocalFilterInfo.Size).ToArray());
            int bodyLength = this._filterInfo.BasePortocalFilterInfo.GetBodyLengthFromHeader(readOnly);
            rtn = this._filterInfo.BasePortocalFilterInfo.Size + bodyLength;
            return rtn;
        }
        /// <summary>
        /// 过滤全部包长度的数据内容
        /// </summary>
        /// <param name="sequence">缓存数据</param>
        /// <param name="baizeSession">会话</param>
        /// <param name="size">数据包长度</param>
        private bool FilterAllData(ReadOnlySequence<byte> sequence ,BaizeSession baizeSession, int size)
        {
            bool rtn = false; 
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
                rtn = MultiSegmentHandler(baizeSession, sequence, size);
            }
            return rtn;
        }
        /// <summary>
        /// 多数据段处理
        /// </summary>
        /// <param name="baizeSession">会话</param>
        /// <param name="sequence">数据</param>
        /// <returns></returns>
        private bool MultiSegmentHandler(BaizeSession baizeSession, ReadOnlySequence<byte> sequence,int size)
        {
            bool rtn = false;
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
        public override bool Filter(BaizeSession baizeSession)
        {
            bool rtn = false;
            if (this._filterInfo.BasePortocalFilterInfo.GetBodyLengthFromHeader != null)
            {
                ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(_head, 0, _tail, _tail.End);
                int size = GetPacketALlLength(sequence);
                if (size > -1)
                {
                    rtn = FilterAllData(sequence, baizeSession, size);
                }
            }
            else
            {
                Reset();
            }
            return rtn;
        }
 

    }
}
