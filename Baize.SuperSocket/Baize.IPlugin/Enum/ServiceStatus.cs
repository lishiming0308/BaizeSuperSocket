using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Enum
{
    /// <summary>
    /// 服务状态
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>
        /// 正常停止
        /// </summary>
        NormalStop = 0,
        /// <summary>
        /// 运行
        /// </summary>
        Run = 1,
        /// <summary>
        /// 禁止
        /// </summary>
        Disable = 2,
        /// <summary>
        /// 异常停止
        /// </summary>
        ExceptStop = 3,
        /// <summary>
        /// 启动中
        /// </summary>
        Starting = 4


    }
}
