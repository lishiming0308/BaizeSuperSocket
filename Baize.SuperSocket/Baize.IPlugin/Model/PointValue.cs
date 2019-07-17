using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 点值
    /// </summary>
    public class PointValue
    {
        /// <summary>
        /// 设备对OID
        /// </summary>
        public string DeviceOID
        {
            get;
            set;
        }
        /// <summary>
        /// 采集点对象OID
        /// </summary>

        public string PointOID
        {
            get;
            set;
        }
        /// <summary>
        /// 采集点值
        /// </summary>

        public double Value
        {
            get;
            set;
        }
    }
}
