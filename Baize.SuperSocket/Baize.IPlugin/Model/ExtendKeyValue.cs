using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 设备和采集点的扩展属性
    /// </summary>
    public class ExtendKeyValue
    {
        /// <summary>
        /// 键
        /// </summary>
        /// <remarks>键</remarks>
        public string KeyCode
        {
            get;
            set;
        }
        /// <summary>
        /// 键名称
        /// </summary>
        public string KeyName
        {
            get;
            set;
        }
        /// <summary>
        /// 值
        /// </summary>
        /// <remarks>值</remarks>
        public string KeyValue
        {
            get;
            set;
        }
    }
}
