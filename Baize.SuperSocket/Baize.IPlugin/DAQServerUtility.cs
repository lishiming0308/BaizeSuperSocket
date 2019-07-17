using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Baize.IPlugin
{
    public class DAQServerUtility
    {
        private static readonly Lazy<DAQServerUtility> instance = new Lazy<DAQServerUtility>();
        /// <summary>
        /// 接收命令总数量
        /// </summary>
        private  long _receiveDataCount = 0;
        /// <summary>
        /// 发送命令总数量
        /// </summary>
        private long _sendDataCount = 0;
        /// <summary>
        /// 启动时间
        /// </summary>
        private readonly DateTime _startTime = DateTime.Now;
        private  ConcurrentDictionary<string, DriverInfo> _dAQDrives = new ConcurrentDictionary<string, DriverInfo>();
        /// <summary>
        /// 静态实例
        /// </summary>
        public static DAQServerUtility Instance
        {
            get
            {
                return instance.Value;
            }
        }
        /// <summary>
        /// 初始化插件接口集合
        /// </summary>
        /// <param name="_dAQDrives">插件集合</param>
        public void Init(ConcurrentDictionary<string, DriverInfo> _dAQDrives)
        {
            this._dAQDrives = _dAQDrives;
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="productOID">产品对象OID</param>
        /// <param name="baizeSession">会话数据</param>
        /// <returns></returns>
        public int NewRequestReceived(string productOID, BaizeSession baizeSession)
        {
            int rtn = 0;
            if(_dAQDrives.TryGetValue(productOID,out var dAQDrive))
            {
                Interlocked.Increment(ref _receiveDataCount);
                rtn = dAQDrive.DAQDrive.NewRequestReceived(baizeSession);
            }
            return rtn;
        }
        /// <summary>
        /// 将设备对象推送给平台
        /// </summary>
        /// <remarks>将设备数据值对象推送给平台</remarks>
        /// <param name="daqDevice">采集设备列表</param>
        /// <param name="runningNumber">发送信息的流水号,信息唯一标号.此参数用于和result参数一起用于判断此设备信息是否真正发送到了应用服务中</param>
        /// <param name="result">设备数据推送到应用服务的回调结果。此参数对性能有损耗如果不是必须校验每次都要发送到应用服务不必调用此参数</param>
        /// <returns>1=成功,0=失败,2=系统繁忙</returns>
        public int PostDeviceData(DAQDevice daqDevice, string runningNumber = "", Action<bool, string> result = null)
        {
            
            return 1;
        }
        /// <summary>
        /// 将指定设备的通讯元数据推送到平台
        /// </summary>
        /// <remarks>将指定设备的通讯元数据推送到平台</remarks>
        /// <param name="devOID">设备对象OID</param>
        /// <param name="message">通讯元数据</param>
        /// <returns>1=成功,0=失败</returns>
        public int PostMetadataMessage(string devOID, string message)
        {
            int rtn = 1;
            return rtn;
        }
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="sessionID">会话ID</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public ValueTask<int> SendData(string sessionID,ReadOnlyMemory<byte> data)
        {
            if (_sendDataEvent != null)
            {
                Interlocked.Increment(ref _sendDataCount);
                return _sendDataEvent(sessionID, data);
            }
            else
            {
                return new ValueTask<int>(0);
            }
        }
        #region 事件
        private event Func<string, ReadOnlyMemory<byte>, ValueTask<int>> _sendDataEvent;
        /// <summary>
        /// 发送数据命令事件
        /// </summary>
        public event Func<string, ReadOnlyMemory<byte>, ValueTask<int>> SendDataEvent
        {
            add
            {
                _sendDataEvent +=value;
            }
            remove
            {
                _sendDataEvent -= value;
            }
        }
        public long ReceiveDataCount
        {
            get
            {
                return _receiveDataCount;
            }
        }
        public long SendDataCount
        {
            get
            {
                return _sendDataCount;
            }
        }
        #endregion
    }
}
