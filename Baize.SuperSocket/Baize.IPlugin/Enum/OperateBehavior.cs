using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Enum
{
    /// <summary>
    /// 操作行为
    /// </summary>
    public enum OperateBehavior
    {
        /// <summary>
        /// 从新载入
        /// </summary>
        Reload = 0,
        /// <summary>
        /// 从新加载实时拦截插件
        /// </summary>
        ReloadRealIntercept = 1,
        /// <summary>
        /// 从新加载归档拦截插件
        /// </summary>
        ReloadArchiveIntercept = 2,
        /// <summary>
        /// 增加
        /// </summary>
        Add = 3,
        /// <summary>
        /// 修改
        /// </summary>
        Modify = 4,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 5
    }
}
