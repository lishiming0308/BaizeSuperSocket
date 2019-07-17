using System.Buffers;
using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;

namespace Baize.SuperSocket.PipelineFilter
{
    /// <summary>
    /// 短报文过滤器
    /// </summary>
    class ShortMessagePipelineFilter : PipelineFilterBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filterInfo">过滤条件</param>
        public ShortMessagePipelineFilter(FilterInfo filterInfo)
        {
            this._filterInfo = filterInfo;
        }
        public override bool Filter(BaizeSession baizeSession)
        {
            return true;
        }
        public override bool FilterData(BaizeSession baizeSession, ReadOnlySequence<byte> data)
        {
            baizeSession.Data = data;
            return Filter(baizeSession);
        }

    }
}
