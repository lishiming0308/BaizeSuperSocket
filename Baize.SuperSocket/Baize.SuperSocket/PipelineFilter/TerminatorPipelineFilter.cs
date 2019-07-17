using System;
using System.Buffers;
using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.BuffersExtensions;
using Baize.SuperSocket.Primitives;
using Baize.SuperSocket.ProtoBase;
namespace Baize.SuperSocket.PipelineFilter
{
    /// <summary>
    /// 指定结束符过滤
    /// </summary>
    public class TerminatorPipelineFilter : PipelineFilterBase
    {
        private readonly ReadOnlyMemory<byte> _terminatorMark;
        public TerminatorPipelineFilter(ChannelOptions channelOptions,FilterInfo filterInfo)
        {
            this._channelOptions = channelOptions;
            this._filterInfo = filterInfo;
            _terminatorMark = this._filterInfo.BasePortocalFilterInfo.Terminator;
        }
        public override bool Filter(BaizeSession baizeSession)
        {
            bool rtn = false;
            ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(_head, 0, _tail, _tail.End);
            if (sequence.IsSingleSegment)
            {
                int terminatorIndex = sequence.First.Span.IndexOf(_terminatorMark.Span);
                if (terminatorIndex > -1)
                {
                    int length = terminatorIndex + _terminatorMark.Length;
                    ReadOnlySequence<byte> readOnly = new ReadOnlySequence<byte>(sequence.First.Slice(0, length));
                    baizeSession.Data = readOnly;
                    rtn = true;
                }
            }
            else
            {
                rtn= MultiSegmentHandler(baizeSession,  sequence);
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
            BaizeBufferSegment tmpHead = null, tmpTail = null;
            int tmpHeadIndex = 0;
            int tmpTailIndex = 0;
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
                int terminatorIndex = memory.Span.IndexOf(_terminatorMark.Span);
                if (terminatorIndex != -1)
                {
                    tmpTailIndex = terminatorIndex;
                    rtn = true;
                    break;
                }
            }
            if (rtn)
            {
                ReadOnlySequence<byte> readOnly = new ReadOnlySequence<byte>(tmpHead, tmpHeadIndex, tmpTail, tmpTailIndex + _terminatorMark.Length);
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
            return rtn;
        }
    }
}
