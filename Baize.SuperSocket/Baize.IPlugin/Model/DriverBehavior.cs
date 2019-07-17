using Baize.IPlugin.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 插件行为
    /// </summary>
    public class DriverBehavior
    {
        /// <summary>
        /// 内容对象OID
        /// </summary>
        public string OID
        {
            get;
            set;
        }
        /// <summary>
        /// 辅助对象OID,在内容为采集点时此对象为采集点数据流模板对象
        /// </summary>
        public string SecondaryOID
        {
            get;
            set;
        }
        /// <summary>
        /// 内存类型
        /// </summary>
        public BaizePlugContentType ContentType
        {
            get;
            set;
        }
        /// <summary>
        /// 操作类型
        /// </summary>
        public OperateBehavior OperateBehavior
        {
            get;
            set;
        }
    }
}
