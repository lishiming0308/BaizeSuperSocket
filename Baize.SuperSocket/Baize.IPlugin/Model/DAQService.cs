using Baize.IPlugin.Enum;
using Baize.IPlugin.SuperSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.Model
{
    /// <summary>
    /// 采集服务
    /// </summary>
    public class DAQService
    {
        /// <summary>
        /// 对象OID
        /// </summary>
        public string OID
        {
            get;
            set;
        }

        /// <summary>
        /// 节点对象OID
        /// </summary>
        public string NodeOID
        {
            get;
            set;
        }

        /// <summary>
        /// 开发商用户OID
        /// </summary>
        public string UserOID
        {
            get;
            set;
        }

        /// <summary>
        /// 创建用户OID
        /// </summary>
        public string CreateUserOID
        {
            get;
            set;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }
        /// <summary>
        /// 应用服务对象OID
        /// </summary>
        public string AppServiceOID
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
        /// 是否启用超时
        /// </summary>
        public bool IsTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// 空闲会话时间
        /// </summary>
        public int IdleSessionTime
        {
            get;
            set;
        }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// 协议
        /// </summary>
        public ProtocolType ProtocolType
        {
            get;
            set;
        }

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxConnectNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 运行时ID
        /// </summary>
        public int RuntimePID
        {
            get;
            set;
        }

        /// <summary>
        /// 采集服务向应用服务传递数据模式
        /// </summary>
        /// <remarks>0=即刻传输，1=缓存传输</remarks>
        public TransferModel TransferModel
        {
            get;
            set;
        }

        /// <summary>
        /// 缓存时间，单位ms
        /// </summary>
        public int CahceTime
        {
            get;
            set;
        }

        /// <summary>
        /// 状态，0=停止，1=运行，2=禁止
        /// </summary>
        public ServiceStatus Status
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get;
            set;
        }
    }
}
