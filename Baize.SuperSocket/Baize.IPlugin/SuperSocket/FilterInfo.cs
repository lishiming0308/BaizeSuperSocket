using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.SuperSocket
{
    public enum ProtoType
    {
        /// <summary>
        /// 短报文
        /// </summary>
        ShortMessage=0,
        /// <summary>
        /// 固定头
        /// </summary>
        FixHeaderSize=1,
        /// <summary>
        /// 固定长度
        /// </summary>
        FixSize=2,
        /// <summary>
        /// 起始结束定位符
        /// </summary>
        BeginEndMark=3,
        /// <summary>
        /// 结束定位符
        /// </summary>
        Terminator=4
    }
    /// <summary>
    /// 过滤信息
    /// </summary>
    public  class FilterInfo
    {
        /// <summary>
        /// 基础协议过滤信息
        /// </summary>
        public BasePortocalFilterInfo BasePortocalFilterInfo
        {
            get;
            set;
        }
        /// <summary>
        /// 产品对象列表
        /// </summary>
        public List<string> ProductOIDS
        {
            get;
            set;
        }
      
    }
}
