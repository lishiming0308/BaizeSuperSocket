using Baize.IPlugin.Model;
using Baize.SuperSocket.Channel;
using Baize.SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.SuperSocket.Primitives
{
    /// <summary>
    /// 服务选项
    /// </summary>
    public class ServerOptions 
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }
       
        /// <summary>
        /// 监听选项
        /// </summary>
        public List<ListenOptions> Listeners
        {
            get; set;
        } = new List<ListenOptions>();
        /// <summary>
        /// 过滤数据包选项
        /// </summary>
        public ChannelOptions FilterDataOptions
        {
            get;
            set;
        } = new ChannelOptions();

        /// <summary>
        /// 产品信息
        /// </summary>
        public List<ProductConfig> ProductInfos
        {
            get;
            set;
        } = new List<ProductConfig>();
        /// <summary>
        /// 会话是否启动超时
        /// </summary>
        public bool IsTimeout
        {
            get;
            set;
        } = true;
        /// <summary>
        /// 空会话时间
        /// </summary>
        public int IdleTime
        {
            get;
            set;
        } = 120;
    }
}
