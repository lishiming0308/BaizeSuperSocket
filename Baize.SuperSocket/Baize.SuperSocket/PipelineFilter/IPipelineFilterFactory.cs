using System.Collections.Generic;
using Baize.IPlugin.Model;
using Baize.SuperSocket.Channel;
using Baize.SuperSocket.ProtoBase;

namespace Baize.SuperSocket.PipelineFilter
{
    /// <summary>
    /// 创建通道过滤条件工厂
    /// </summary>
    interface IPipelineFilterFactory
    {
        /// <summary>
        /// 创建过滤器列表
        /// </summary>
        /// <param name="productInfos">产品信息列表</param>
        /// <returns></returns>
        List<IFilter> Create( List<ProductConfig> productInfos);
    }
}
