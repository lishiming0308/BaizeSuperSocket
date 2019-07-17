using Baize.IPlugin.Enum;
using Baize.IPlugin.SuperSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 产品配置信息
    /// </summary>
    public class ProductConfig
    {
        /// <summary>
        /// 产品对象OID
        /// </summary>
        public string ProductOID
        {
            get;
            set;
        }
        /// <summary>
        /// 数据包超时时间（s）
        /// </summary>
        public double DataPackTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// 最大超时次数
        /// </summary>
        public int MaxTimeoutNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 超时次数
        /// </summary>
        public int TimeoutNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 接收数据类型0=直接接收，1=中转数据
        /// </summary>
        public DAQType DAQType
        {
            get;
            set;
        }

        /// <summary>
        /// 产品运行版本
        /// </summary>
        public string RunVersion
        {
            get;
            set;
        }
        /// <summary>
        /// 产品类型
        /// </summary>
        public ProductType ProductType
        {
            get;
            set;
        }
        /// <summary>
        /// 当前产品数据包解析时间
        /// </summary>
        public double CurrentDataPackParseTime
        {
            get;
            set;
        }
        /// <summary>
        /// 基础协议过滤信息
        /// </summary>
        public BasePortocalFilterInfo BasePortocalFilterInfo
        {
            get;
            set;
        } = new BasePortocalFilterInfo();
    }
}
