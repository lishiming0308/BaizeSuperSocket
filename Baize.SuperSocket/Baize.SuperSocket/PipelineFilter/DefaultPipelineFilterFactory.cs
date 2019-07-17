using System;
using System.Collections.Generic;
using System.Linq;
using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.Channel;
using Baize.SuperSocket.Primitives;
using Baize.SuperSocket.ProtoBase;
using Microsoft.Extensions.Options;

namespace Baize.SuperSocket.PipelineFilter
{
    /// <summary>
    /// 默认通道过滤条件创建工厂
    /// </summary>
    public class DefaultPipelineFilterFactory : IPipelineFilterFactory
    {
        /// <summary>
        /// 通道设置选项
        /// </summary>
        private ChannelOptions _channelOptions;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="channelOptions">通道设置选项</param>
        public DefaultPipelineFilterFactory(ChannelOptions channelOptions)
        {
            this._channelOptions = channelOptions;
        }
        /// <summary>
        /// 创建过滤条件列表
        /// </summary>
        /// <param name="productInfos">产品信息列表</param>
        /// <returns></returns>
        public List<IFilter> Create( List<ProductConfig> productInfos)
        {
            List<IFilter> rtn = new List<IFilter>();
            if (productInfos != null)
            {
                var groups = productInfos.GroupBy(p => p.BasePortocalFilterInfo, new ProductInfoEqualityComparer());
                foreach (var group in groups)
                {
                   
                    FilterInfo filterInfo = new FilterInfo();
                    filterInfo.BasePortocalFilterInfo = group.Key;
                    filterInfo.ProductOIDS = group.Select(i => i.ProductOID).ToList();
                    IFilter pipelineFilter = new ShortMessagePipelineFilter(filterInfo);
                    switch (group.Key.ProtoType)
                    {
                        case ProtoType.BeginEndMark:
                            pipelineFilter = new BeginEndMarkPipelineFilter(_channelOptions, filterInfo);
                            break;
                        case ProtoType.FixHeaderSize:
                            pipelineFilter = new FixHeaderSizePipelineFilter(_channelOptions, filterInfo);
                            break;
                        case ProtoType.FixSize:
                            pipelineFilter = new FixHeaderSizePipelineFilter(_channelOptions, filterInfo);
                            break;
                        case ProtoType.Terminator:
                            pipelineFilter = new TerminatorPipelineFilter(_channelOptions, filterInfo);
                            break;
                    }
                    rtn.Add(pipelineFilter);
                }
            }
            return rtn;
        }
    }
}
