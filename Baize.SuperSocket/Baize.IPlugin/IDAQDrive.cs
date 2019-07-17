using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin
{
    /// <summary>
    /// 数据采集驱动接口
    /// </summary>
    /// <remarks>数据采集驱动接口</remarks>
    public interface IDAQDrive //API
    {
        /// <summary>
        /// 获得当前默认产品设置信息
        /// </summary>
        /// <returns></returns>
        DefaultProductInfo GetDefaultProductInfo();
        /// <remarks>启动数据采集</remarks>
        /// <param name="product">产品配置信息</param>
        /// <param name="devices">当前产品的设备列表</param>
        /// <param name="daqService">采集服务</param>
        /// <returns>1=成功,0=失败</returns>
        int Startup(ProductConfig product, ConcurrentDictionary<string, DAQDevice> devices, DAQService daqService);
        /// <remarks>停止数据采集</remarks>
        /// <returns>1=成功，0=失败</returns>
        int Stop();
        /// <remarks>建立连接会话</remarks>
        /// <param name="session">当前会话</param>
        /// <returns>1=成功,0=失败</returns>
        int NewSessionConnected(BaizeSession session);
        /// <remarks>接收会话中的数据</remarks>
        /// <param name="session">当前会话</param>
        /// <returns>1=成功,0=失败</returns>
        int NewRequestReceived(BaizeSession session);
        /// <summary>
        /// 连接会话关闭
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="reason">关闭原因</param>
        /// <returns></returns>
        int SessionClosed(BaizeSession session, CloseReason reason);
        /// <summary>
        /// 控制设备
        /// </summary>
        /// <param name="deviceOID">设备对象OID</param>
        /// <param name="data">指令数据</param>
        /// <returns>成功=ok,其他信息为具体错误信息</returns>
        string ControlDevice(string deviceOID, byte[] data);
        /// <remarks>接收向指定设备发送的数据</remarks>
        /// <param name="sourceDevOID">源设备对象</param>
        /// <param name="destDevOID">目标设备对象</param>
        /// <param name="data">接收的数据</param>
        /// <param name="args">其他相关参数</param>
        /// <returns>1=成功,0=失败</returns>
        int ReceiveDeviceData(string sourceDevOID, string destDevOID, byte[] data, params object[] args);
        /// <summary>
        /// 控制驱动行为
        /// </summary>
        /// <param name="driverBehaviors">行为列表</param>
        /// <returns>成功=ok,其他信息为具体错误信息</returns>
        string ControlBehavior(List<DriverBehavior> driverBehaviors);
        /// <summary>
        /// 控制设备下的多采集点
        /// </summary>
        /// <param name="pointValues">采集点值列表</param>
        /// <returns>成功=ok,其他信息为具体错误信息</returns>
        string ControlDevicePoints(List<PointValue> pointValues);
        /// <summary>
        /// 向驱动发送控制指令
        /// </summary>
        /// <param name="data">控制指令</param>
        /// <returns></returns>
        string ControlDriver(byte[] data);
        /// <summary>
        /// 控制采集点
        /// </summary>
        /// <param name="deviceOID">设备对象OID</param>
        /// <param name="pointOID">采集点对象OID</param>
        /// <param name="value">采集点值</param>
        /// <returns>成功=ok,其他信息为具体错误信息</returns>
        string ControlPoint(string deviceOID, string pointOID, double value);
        /// <summary>
        /// 获得设备巡测数据
        /// </summary>
        /// <param name="devOID">设备对象OID</param>
        byte[] GetDevicePatrolData(string devOID);
        /// <summary>
        /// 发送设备巡测指令并返回巡测指令数据
        /// </summary>
        /// <param name="devOID">设备对象OID</param>
        byte[] SendDevicePatrolCmd(string devOID);
    }
}
