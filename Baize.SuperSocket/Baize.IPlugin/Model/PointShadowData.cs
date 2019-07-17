using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 采集点影子数据
    /// </summary>
    public class PointShadowData
    {
        private double value = 0;
        private string valueStr = "";
        internal bool valueChange = false;
        /// <summary>
        /// 构造函数
        /// </summary>
        public PointShadowData()
        {
            CollectionTime = DateTime.Now;
            ExtendProperties = new ConcurrentBag<ExtendKeyValue>();
            HistoryPoints = new ConcurrentBag<ArchivePointData>();
        }
        /// <summary>
        /// 数据流对象OID
        /// </summary>
        public string DataFlowOID
        {
            get;
            set;
        }
        /// <summary>
        /// 数据点值
        /// </summary>
        public double Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                valueChange = true;
            }
        }
        /// <summary>
        /// 数据点字符值
        /// </summary>
        public string ValueStr
        {
            get
            {
                return valueStr;
            }
            set
            {
                valueStr = value;
                valueChange = true;
            }
        }
        /// <summary>
        /// 采集点时间
        /// </summary>
        public DateTime CollectionTime
        {
            get;
            set;
        }
        /// <summary>
        /// 采集点实时扩展属性
        /// </summary>
        public ConcurrentBag<ExtendKeyValue> ExtendProperties
        {
            get;
            set;
        }
        /// <summary>
        /// 采集点打包上传的历史数据
        /// </summary>
        public ConcurrentBag<ArchivePointData> HistoryPoints
        {
            get;
            set;
        }


    }
}
