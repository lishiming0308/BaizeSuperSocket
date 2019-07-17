using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 打包上传的归档点数据
    /// </summary>
    public class ArchivePointData
    {
        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime CollectionTime
        {
            get;
            set;
        }

        /// <summary>
        /// 采集点历史值
        /// </summary>
        public double Value
        {
            get;
            set;
        }
        /// <summary>
        /// 采集点值字符串
        /// </summary>
        public String ValueString
        {
            get;
            set;
        }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public ConcurrentBag<ExtendKeyValue> ExtendProperties
        {
            get;
            set;
        }
    }
}
