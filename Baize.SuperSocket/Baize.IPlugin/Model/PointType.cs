using Baize.IPlugin.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 采集点类型
    /// </summary>
    public class PointType

    {
        /// <summary>
        /// 采集点类型英文编码
        /// </summary>
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        /// <remarks>采集点类型名称</remarks>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 类型
        /// </summary>
        /// <remarks>采集点类型的分类0=采集值，1=系统值</remarks>
        public PointParamType Type
        {
            get;
            set;
        }
    }
}
