using Baize.IPlugin.Enum;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Baize.IPlugin.Model
{

    /// <summary>
    /// 采集点
    /// </summary>
    public class DAQPoint
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DAQPoint()
        {
            ExtendProperties = new List<CustomProperty>();
            PointType = new PointType()
            {
                Code = "Param",
                Name = "参数",
                Type = PointParamType.Custom
            };
            DataValType = new DataValType()
            {
                Code = "Object",
                Name = "对象"
            };
            PointShadowData = new PointShadowData()
            {
                CollectionTime = DateTime.Now,
                DataFlowOID = "",
                ExtendProperties = new ConcurrentBag<ExtendKeyValue>(),
                Value = 0,
                ValueStr = ""
            };
        }

        /// <summary>
        /// 采集点对象OID
        /// </summary>
        [Key]
        public string OID
        {
            get;
            set;
        } = Guid.NewGuid().ToString();
        /// <summary>
        /// 英文名称
        /// </summary>
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 采集点类型
        /// </summary>
        public PointType PointType
        {
            get;
            set;
        }

        /// <summary>
        /// 数据值类型
        /// </summary>
        /// <remarks>数据值类型</remarks>
        public DataValType DataValType
        {
            get;
            set;
        }

        /// <summary>
        /// 数字量描述信息
        /// </summary>
        public PointValueCaption ValCaptionType
        {
            get;
            set;
        }

        /// <summary>
        /// 是否为开关量
        /// </summary>
        /// <remarks>是否是开关量</remarks>
        public bool IsSwitchValue
        {
            get;
            set;
        }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit
        {
            get;
            set;
        }

        /// <summary>
        /// SN序号
        /// </summary>
        public string SN
        {
            get;
            set;
        }

        /// <summary>
        /// Modbus服务地址
        /// </summary>
        public int ModbusRegisterAddress
        {
            get;
            set;
        }
        /// <summary>
        /// Modbus服务位地址
        /// </summary>
        public int ModbusRegisterBit
        {
            get;
            set;
        }
        /// <summary>
        /// 数据流模板对象OID
        /// </summary>
        public string DAQPointGroupOID
        {
            get;
            set;
        }
        /// <summary>
        /// 倍率
        /// </summary>
        public float MultipleRate
        {
            get;
            set;
        }
        /// <summary>
        /// 扩展属性
        /// </summary>
        /// <remarks>扩展属性</remarks>
        public List<CustomProperty> ExtendProperties
        {
            get;
            set;
        }
        /// <summary>
        /// 插件表达式
        /// </summary>
        public string PlugPointExpress
        {
            get;
            set;
        }
        /// <summary>
        /// 应用服务级别表达式(预留)
        /// </summary>
        public string AppServicePointExpress
        {
            get;
            set;
        }
        /// <summary>
        /// 采集点实时数据信息
        /// </summary>
        public PointShadowData PointShadowData
        {
            get;
            set;
        }
        /// <summary>
        /// 状态0=正常,1=删除
        /// </summary>
        public DataState DataState
        {
            get;
            set;
        }
        /// <summary>
        /// 当前采集点的写入值
        /// </summary>
        public object WriteValue
        {
            get;
            set;
        }

    }
}
