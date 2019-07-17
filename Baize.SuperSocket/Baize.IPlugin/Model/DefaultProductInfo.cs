using Baize.IPlugin.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 默认产品信息
    /// </summary>
    public class DefaultProductInfo
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name
        {
            get;
            set;
        } = "";

        /// <summary>
        /// 产品类型，0=数据采集,1=数据订阅
        /// </summary>
        public ProductType ProductType
        {
            get;
            set;
        } = ProductType.DAQ;
        /// <summary>
        /// 作者
        /// </summary>
        public string Author
        {
            get;
            set;
        } = "";
        /// <summary>
        /// 数据包超时时间（s）
        /// </summary>
        public double DataPackTimeout
        {
            get;
            set;
        } = 10;

        /// <summary>
        /// 最大超时次数
        /// </summary>
        public int MaxTimeoutNumber
        {
            get;
            set;
        } = 20;
        /// <summary>
        /// 超时次数
        /// </summary>
        public int TimeoutNumber
        {
            get;
            set;
        } = 0;

        /// <summary>
        /// 接收数据类型0=直接接收，1=中转数据
        /// </summary>
        public DAQType DAQType
        {
            get;
            set;
        } = DAQType.Direct;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get;
            set;
        } = "";
        /// <summary>
        /// 产品默认设备
        /// </summary>
        public DAQDevice DefaultDevice
        {
            get;
            set;
        }
        /// <summary>
        /// 产品默认数据流
        /// </summary>
        public List<DAQPoint> DAQDataFlows
        {
            get;
            set;
        } = new List<DAQPoint>();
        /// <summary>
        /// 设备自定义属性列表
        /// </summary>
        public List<CustomProperty> DeviceCustom
        {
            get;
            set;
        } = new List<CustomProperty>();
        /// <summary>
        /// 采集点自定义属性列表
        /// </summary>
        public List<CustomProperty> DataFlowCustom
        {
            get;
            set;
        } = new List<CustomProperty>();
    }
}
