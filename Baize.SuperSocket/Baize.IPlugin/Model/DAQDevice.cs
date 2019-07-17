using Baize.IPlugin.Enum;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 设备
    /// </summary>
    public class DAQDevice
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DAQDevice()
        {
            ExtendProperties = new List<CustomProperty>();
            DAQPoints = new ConcurrentDictionary<string, DAQPoint>();
            PointShadowDatas = new ConcurrentDictionary<string, PointShadowData>();
        }
        /// <summary>
        /// 租户对象OID
        /// </summary>
        public string TenantOID
        {
            get;
            set;
        }
        [Key]
        /// <summary>
        /// 设备对象OID
        /// </summary>
        public string OID
        {
            get;
            set;
        } = Guid.NewGuid().ToString();

        /// <summary>
        /// 产品对象OID
        /// </summary>
        public string ProductOID
        {
            get;
            set;
        }

        /// <summary>
        /// 所在采集服对象OID
        /// </summary>
        public string DAQServiceOID
        {
            get;
            set;
        }

        /// <summary>
        /// 服设备对象OID
        /// </summary>
        public string ParentDeviceOID
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get;
            set;
        }

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
        /// SN序号
        /// </summary>
        public string SN
        {
            get;
            set;
        }

        /// <summary>
        /// 安装地址
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// 经度
        /// </summary>
        public double X
        {
            get;
            set;
        }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Y
        {
            get;
            set;
        }

        /// <summary>
        /// 海拔高度
        /// </summary>
        public double H
        {
            get;
            set;
        }

        /// <summary>
        /// 是否可以控制
        /// </summary>
        public bool IsControl
        {
            get;
            set;
        }

        /// <summary>
        /// Modbus服务从设备地址
        /// </summary>
        public int ModbusDevID
        {
            get;
            set;
        }

        /// <summary>
        /// Modbus服务起始地址
        /// </summary>
        public int ModbusBeginAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Modubs服务寄存器长度
        /// </summary>
        public int ModbusRegisterLength
        {
            get;
            set;
        }

        /// <summary>
        /// 数据是否存库
        /// </summary>
        public bool IsCycleStore
        {
            get;
            set;
        }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public List<CustomProperty> ExtendProperties
        {
            get;
            set;
        }

        /// <summary>
        /// 采集频率
        /// </summary>
        public double Frequency
        {
            get;
            set;
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        public double Timeout
        {
            get;
            set;
        }
        /// <summary>
        /// 数据流分组模板对象OID
        /// </summary>
        public string DAQPointGroupOID
        {
            get;
            set;
        }

        /// <summary>
        /// 创建设备的用户OID
        /// </summary>
        public string CreateUserOID
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
        /// 设备下的采集点字典列表
        /// </summary>
        public ConcurrentDictionary<string, DAQPoint> DAQPoints
        {
            get;
            set;
        }
        /// <summary>
        /// 设备下的采集点列表
        /// </summary>
        public List<DAQPoint> DAQPointList
        {
            get
            {
                return DAQPoints.Values.ToList();
            }
        }
        /// <summary>
        /// 采集点影子数据
        /// </summary>
        public ConcurrentDictionary<string, PointShadowData> PointShadowDatas
        {
            get;
            internal set;
        }

    }
}
