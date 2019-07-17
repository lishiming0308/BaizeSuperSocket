using System.Buffers;
using System.Collections.Generic;
using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;

namespace Baize.SuperSocket.PipelineFilter
{
    public interface IFilter
    {
        /// <summary>
        /// 产品列表
        /// </summary>
        FilterInfo FilterInfo { get;  }
        /// <summary>
        /// 过滤返回会话
        /// </summary>
        /// <param name="baizeSession">当前连接会话</param>
        /// <param name="data">接收的原始数据</param>
        /// <returns>是否解析成功</returns>
        bool FilterData(BaizeSession baizeSession, ReadOnlySequence<byte> data);
       


    }
}
