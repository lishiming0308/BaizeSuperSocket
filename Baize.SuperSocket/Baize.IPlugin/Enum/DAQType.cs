using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Enum
{
    /// <summary>
    /// 采集产品类型
    /// </summary>
    public enum DAQType
    {
        /// <summary>
        /// 接收从采集服务获得数据
        /// </summary>
        Direct = 0,
        /// <summary>
        /// 接收同一个采集服务下的其他插件数据
        /// </summary>
        Transfer = 1
    }
}
