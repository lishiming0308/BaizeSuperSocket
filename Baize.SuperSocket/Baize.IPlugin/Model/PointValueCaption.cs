using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 开关量描述
    /// </summary>
    public class PointValueCaption
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PointValueCaption()
        {
            Value = new ConcurrentBag<SwitchKeyValue>();
        }
        /// <summary>
        /// 英文描述
        /// </summary>
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        /// <remarks>开关量名称</remarks>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 值描述列表
        /// </summary>
        /// <remarks>开关量值对应的文字描述</remarks>
        public ConcurrentBag<SwitchKeyValue> Value
        {
            get;
            set;
        }
    }
}
