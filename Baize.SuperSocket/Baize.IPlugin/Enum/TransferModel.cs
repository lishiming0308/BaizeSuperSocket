using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Enum
{
    /// <summary>
    /// 采集服向应用服务传输数据模式
    /// </summary>
    public enum TransferModel
    {
        /// <summary>
        /// 即刻传输
        /// </summary>
        Immediately = 0,
        /// <summary>
        /// 缓存传输
        /// </summary>
        Cache = 1
    }
}
