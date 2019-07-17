using Baize.IPlugin;
using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Driver.WriteModbus
{
    public class DAQDrive : IDAQDrive
    {
        private ProductConfig _productConfig;
        private ConcurrentDictionary<string, DAQDevice> _devices;
        private ConcurrentDictionary<string, DAQDevice> _devsSN = new ConcurrentDictionary<string, DAQDevice>();
        private DAQService _dAQService;
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="product">产品</param>
        /// <param name="devices">设备</param>
        /// <param name="daqService">采集服务</param>
        /// <returns></returns>
        public int Startup(ProductConfig product, ConcurrentDictionary<string, DAQDevice> devices, DAQService daqService)
        {
            int rtn = 1;
            this._productConfig = product;
            this._devices = devices;
            this._dAQService = daqService;
            foreach(DAQDevice dev in _devices.Values)
            {
                _devsSN.TryAdd(dev.SN, dev);
            }
            return rtn;
        }

        public int NewRequestReceived(BaizeSession session)
        {
            int rtn = 1;      
            ushort devID =BitConverter.ToUInt16(new byte[] { session.Data.First.Span[4],session.Data.First.Span[3]});
            if(_devsSN.TryGetValue(devID.ToString(),out var dAQDevice))
            if (dAQDevice != null)
            {
                DAQPoint dAQPoint= dAQDevice.DAQPointList.Where(i => i.SN == "0").FirstOrDefault();
                if(dAQPoint !=null)
                {
                    dAQPoint.PointShadowData.Value = 1;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "1").FirstOrDefault();
                if (dAQPoint != null)
                {
                    System.DateTime startTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                    long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
                    dAQPoint.PointShadowData.Value = t;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "2").FirstOrDefault();
                if (dAQPoint != null)
                {
                    double bkssll = BitConverter.ToSingle(new byte[] { session.Data.First.Span[8], session.Data.First.Span[7], session.Data.First.Span[10], session.Data.First.Span[9], });
                    dAQPoint.PointShadowData.Value = bkssll;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "3").FirstOrDefault();
                if (dAQPoint != null)
                {
                    double gkssll = BitConverter.ToSingle(new byte[] { session.Data.First.Span[12], session.Data.First.Span[11], session.Data.First.Span[14], session.Data.First.Span[13] });
                    dAQPoint.PointShadowData.Value = gkssll;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "4").FirstOrDefault();
                if (dAQPoint != null)
                {
                    double yl = BitConverter.ToSingle(new byte[] { session.Data.First.Span[16], session.Data.First.Span[15], session.Data.First.Span[18], session.Data.First.Span[17] });
                    dAQPoint.PointShadowData.Value = yl;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "5").FirstOrDefault();
                if (dAQPoint != null)
                {
                    double wd = BitConverter.ToSingle(new byte[] { session.Data.First.Span[20], session.Data.First.Span[19], session.Data.First.Span[22], session.Data.First.Span[21] });
                    dAQPoint.PointShadowData.Value = wd;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "6").FirstOrDefault();
                if (dAQPoint != null)
                {
                    double bkljll = BitConverter.ToDouble(new byte[] { session.Data.First.Span[24], session.Data.First.Span[23], session.Data.First.Span[26], session.Data.First.Span[25], session.Data.First.Span[28], session.Data.First.Span[27], session.Data.First.Span[30], session.Data.First.Span[29] });
                    dAQPoint.PointShadowData.Value = bkljll;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "7").FirstOrDefault();
                if (dAQPoint != null)
                {
                    double gkljll = BitConverter.ToDouble(new byte[] { session.Data.First.Span[32], session.Data.First.Span[31], session.Data.First.Span[34], session.Data.First.Span[33], session.Data.First.Span[36], session.Data.First.Span[35], session.Data.First.Span[38], session.Data.First.Span[37] });
                    dAQPoint.PointShadowData.Value = gkljll;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "8").FirstOrDefault();
                if (dAQPoint != null)
                {
                    double bkssll2 = BitConverter.ToSingle(new byte[] { session.Data.First.Span[40], session.Data.First.Span[39], session.Data.First.Span[42], session.Data.First.Span[41] });
                    dAQPoint.PointShadowData.Value = bkssll2;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "9").FirstOrDefault();
                if (dAQPoint != null)
                {
                    double gkssll2 = BitConverter.ToSingle(new byte[] { session.Data.First.Span[44], session.Data.First.Span[43], session.Data.First.Span[46], session.Data.First.Span[45] });
                    dAQPoint.PointShadowData.Value = gkssll2;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "10").FirstOrDefault();
                if (dAQPoint != null)
                {
                    double yl2 = BitConverter.ToSingle(new byte[] { session.Data.First.Span[48], session.Data.First.Span[47], session.Data.First.Span[50], session.Data.First.Span[49] });
                    dAQPoint.PointShadowData.Value = yl2;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "11").FirstOrDefault();
                if (dAQPoint != null)
                {
                    double wd2 = BitConverter.ToSingle(new byte[] { session.Data.First.Span[52], session.Data.First.Span[51], session.Data.First.Span[54], session.Data.First.Span[53] });
                    dAQPoint.PointShadowData.Value = wd2;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "12").FirstOrDefault();
                if (dAQPoint != null)
                {
                    double bkljll2 = BitConverter.ToSingle(new byte[] { session.Data.First.Span[56], session.Data.First.Span[55], session.Data.First.Span[58], session.Data.First.Span[57] });
                    dAQPoint.PointShadowData.Value = bkljll2;
                }
                dAQPoint = dAQDevice.DAQPointList.Where(i => i.SN == "13").FirstOrDefault();
                if (dAQPoint != null)
                {
                    double gkljll2 = BitConverter.ToSingle(new byte[] { session.Data.First.Span[60], session.Data.First.Span[59], session.Data.First.Span[62], session.Data.First.Span[61] });
                    dAQPoint.PointShadowData.Value = gkljll2;
                }
                byte[] responData = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x01, 0x03, 0x00, 0x00, 0x00, 0x1e,0x0a};
                DAQServerUtility.Instance.SendData(session.SessionID, responData);
            }
            return rtn;
        }
        public string ControlBehavior(List<DriverBehavior> driverBehaviors)
        {
            throw new NotImplementedException();
        }

        public string ControlDevice(string deviceOID, byte[] data)
        {
            throw new NotImplementedException();
        }

        public string ControlDevicePoints(List<PointValue> pointValues)
        {
            throw new NotImplementedException();
        }

        public string ControlDriver(byte[] data)
        {
            throw new NotImplementedException();
        }

        public string ControlPoint(string deviceOID, string pointOID, double value)
        {
            throw new NotImplementedException();
        }

        public DefaultProductInfo GetDefaultProductInfo()
        {
            throw new NotImplementedException();
        }

        public byte[] GetDevicePatrolData(string devOID)
        {
            throw new NotImplementedException();
        }


        public int NewSessionConnected(BaizeSession session)
        {
            throw new NotImplementedException();
        }

        public int ReceiveDeviceData(string sourceDevOID, string destDevOID, byte[] data, params object[] args)
        {
            throw new NotImplementedException();
        }

        public byte[] SendDevicePatrolCmd(string devOID)
        {
            throw new NotImplementedException();
        }

        public int SessionClosed(BaizeSession session, CloseReason reason)
        {
            throw new NotImplementedException();
        }

     

        public int Stop()
        {
            throw new NotImplementedException();
        }
    }
}
