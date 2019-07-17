using Baize.IPlugin.Enum;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 设备自定义属性
    /// </summary>
    /// <remarks>自定义属性可以描述控件类型和控件的默认值等信息</remarks>
    public class CustomProperty
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomProperty()
        {
            OptionValue = new ConcurrentBag<ExtendKeyValue>();
        }
        /// <summary>
        /// 控件类型
        /// </summary>
        /// <remarks>0=textbox,1=combox,2=datetime，3=复选框</remarks>
        public ControlType ControlType
        {
            get;
            set;
        }

        /// <summary>
        /// 控件的可选值
        /// </summary>
        public ConcurrentBag<ExtendKeyValue> OptionValue
        {
            get;
            set;
        }

        /// <summary>
        /// 控件默认值
        /// </summary>
        public ExtendKeyValue DefaultValue
        {
            get;
            set;
        }
        /// <summary>
        /// 是否独占一行
        /// </summary>
        public bool IsSingleLine
        {
            get;
            set;
        }
    }
}
