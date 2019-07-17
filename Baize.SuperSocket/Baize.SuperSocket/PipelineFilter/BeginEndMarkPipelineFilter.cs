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
    /// 起始结束符过滤
    /// </summary>
    public class BeginEndMarkPipelineFilter : PipelineFilterBase
    {
        private readonly ReadOnlyMemory<byte> _beginMark;

        private readonly ReadOnlyMemory<byte> _endMark;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="channelOptions">通道选项</param>
        /// <param name="filterInfo">过滤条件</param>
        public BeginEndMarkPipelineFilter(ChannelOptions channelOptions, FilterInfo filterInfo)
        {
            this._channelOptions = channelOptions;
            this._filterInfo = filterInfo;
            _beginMark = this._filterInfo.BasePortocalFilterInfo.BeginMark;
            _endMark = this._filterInfo.BasePortocalFilterInfo.EndMark;
        }
        /// <summary>
        /// 过滤数据
        /// </summary>
        /// <param name="baizeSession">会话</param>
        /// <param name="data">接收的socket数据</param>
        /// <returns>true=成功,false=失败</returns>
        public   override  bool Filter(BaizeSession baizeSession)
        {
            bool rtn = false;
            ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(_head, 0, _tail, _tail.End);    
            if(sequence.IsSingleSegment)
            {
                int beginIndex = sequence.First.Span.IndexOf(_beginMark.Span);
                if (beginIndex > -1)
                { 
                    int endIndex = sequence.First.Span.Slice(beginIndex + _beginMark.Length).IndexOf(_endMark.Span);            
                    if (endIndex > -1)
                    {
                        int length = _beginMark.Length + _endMark.Length + endIndex;
                        ReadOnlySequence<byte> readOnly =new ReadOnlySequence<byte>( sequence.First.Slice(beginIndex, length));
                        baizeSession.Data = readOnly;
                        rtn = true;
                    }
                }
            }
            else
            {
                rtn= MultiSegmentHandler(baizeSession,sequence);
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
            bool foundBeginMark = false;
            foreach (var memory in sequence)
            {
                bool addSegment = false;
                if (!foundBeginMark)
                {
                    int beginIndex = memory.Span.IndexOf(_beginMark.Span);
                    if (beginIndex != -1)
                    {
                        foundBeginMark = true;
                        tmpHeadIndex = beginIndex;
                        tmpHead = tmpTail = new BaizeBufferSegment();
                        tmpTail.SetUnownedMemory(memory);
                        addSegment = true;
                    }
                }
                if (foundBeginMark && !addSegment)
                {
                    BaizeBufferSegment next = new BaizeBufferSegment();
                    next.SetUnownedMemory(memory);
                    tmpTail.SetNext(next);
                    tmpTail = next;
                }
                int endIndex = -1;
                if (addSegment)
                {
                    endIndex = memory.Span.Slice(tmpHeadIndex + _beginMark.Length).IndexOf(_endMark.Span);
                    if (endIndex != -1)
                    {
                        endIndex += _beginMark.Length + tmpHeadIndex;
                        tmpTailIndex = endIndex;
                        rtn = true;
                        break;
                    }
                }
                else
                {
                    endIndex = memory.Span.IndexOf(_endMark.Span);
                    if (endIndex != -1)
                    {
                        tmpTailIndex = endIndex;
                        rtn = true;
                        break;
                    }
                }
            }
            if (rtn)
            {
                ReadOnlySequence<byte> readOnly = new ReadOnlySequence<byte>(tmpHead, tmpHeadIndex, tmpTail, tmpTailIndex + _endMark.Length);
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
