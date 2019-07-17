using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 开关量值描述
    /// </summary>
    /// <remarks>开关量值描述</remarks>
    public class SwitchKeyValue
    {
        /// <summary>
        /// 值
        /// </summary>
        /// <remarks>值</remarks>
        public double SwitchValue
        {
            get;
            set;
        }

        /// <summary>
        /// 值描述
        /// </summary>
        /// <remarks>描述</remarks>
        public string SwitchCaption
        {
            get;
            set;
        }
    }
}
