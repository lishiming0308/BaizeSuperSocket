using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 驱动插件信息
    /// </summary>
    public class DriverInfo
    {
        public IDAQDrive DAQDrive
        {
            get;
            set;
        }
        public ProductConfig ProductConfig
        {
            get;
            set;
        }
    }
}
