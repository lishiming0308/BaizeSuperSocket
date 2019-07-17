using Baize.IPlugin.Enum;
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Baize.SuperSocket")]
namespace Baize.IPlugin.SuperSocket
{
    /// <summary>
    /// BaizeSocket会话
    /// </summary>
    public class BaizeSession
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sessionID">会话ID</param>
        public BaizeSession(string sessionID)
        {
            SessionID = sessionID;
            LastTime = DateTime.Now;
        }
        /// <summary>
        /// 构造喊
        /// </summary>
        /// <param name="Data">数据</param>
        public BaizeSession(ReadOnlySequence<byte> Data)
        {
            SessionID = Guid.NewGuid().ToString();
            this.Data = Data;
        }
        /// <summary>
        /// 会话ID
        /// </summary>
        public string SessionID
        {
            get;
        }
        /// <summary>
        /// 会话最新时间
        /// </summary>
        public DateTime LastTime
        {
            get;
            set;
        }
        /// <summary>
        /// 会话启动时间
        /// </summary>
        public DateTime StartTime
        {
            get;
            set;
        } = DateTime.Now;
        /// <summary>
        /// 连接远程地址
        /// </summary>
        public string RemoteIpEndPoint
        {
            get;
            internal set;
        }
        /// <summary>
        /// 本地监听的IP地址
        /// </summary>
        public string LocalIp
        {
            get;
            internal set;
        }
        /// <summary>
        /// 本地监听的端口
        /// </summary>
        public int LocalPort
        {
            get;
            internal set;
        }
        /// <summary>
        /// 本地监听协议
        /// </summary>
        public ProtocolType ProtocolType
        {
            get;
            internal set;
        }
        /// <summary>
        /// 会话数据
        /// </summary>
        public ReadOnlySequence<byte> Data
        {
            get;
            internal set;
        }
    }

}
