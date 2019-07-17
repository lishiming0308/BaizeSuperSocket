using Baize.IPlugin;
using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using Driver.WriteModbus;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xunit;

namespace DriverWriteModbusTest
{
    public class DAQDriveTest
    {
        private  ConcurrentDictionary<string, DAQDevice> CreateDevices(int devNumber)
        {
            ConcurrentDictionary<string, DAQDevice> rtn = new ConcurrentDictionary<string, DAQDevice>();
            for (int i = 0; i < devNumber; i++)
            {
                DAQDevice dAQDevice = new DAQDevice();
                dAQDevice.SN = (i + 1).ToString();
                foreach (DAQPoint dAQPoint in CreatePoints())
                {
                    dAQDevice.DAQPoints.TryAdd(dAQPoint.OID, dAQPoint);
                }
                rtn.TryAdd(dAQDevice.OID, dAQDevice);
            }
            return rtn;
        }
        private  DAQService CreateDAQService()
        {
            DAQService dAQService = new DAQService();
            dAQService.Port = 14040;
            return dAQService;
        }
        private  List<DAQPoint> CreatePoints()
        {
            List<DAQPoint> rtn = new List<DAQPoint>();
            DAQPoint dAQPoint = new DAQPoint();
            dAQPoint.Code = "online";
            dAQPoint.Name = "����״̬";
            dAQPoint.SN = "0";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "LastTime";
            dAQPoint.Name = "����ʱ��";
            dAQPoint.SN = "1";
            //��һ·������
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "BKSS1";
            dAQPoint.Name = "���˲ʱ1";
            dAQPoint.SN = "2";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "GKSS1";
            dAQPoint.Name = "����˲ʱ1";
            dAQPoint.SN = "3";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "YL1";
            dAQPoint.Name = "ѹ��1";
            dAQPoint.SN = "4";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "WD1";
            dAQPoint.Name = "�¶�1";
            dAQPoint.SN = "5";
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "BKLJ1";
            dAQPoint.Name = "����ۼ�1";
            dAQPoint.SN = "6";
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "GKLJ1";
            dAQPoint.Name = "�����ۼ�1";
            dAQPoint.SN = "7";
            rtn.Add(dAQPoint);
            //�ڶ�·������
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "BKSS2";
            dAQPoint.Name = "���˲ʱ2";
            dAQPoint.SN = "8";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "GKSS2";
            dAQPoint.Name = "����˲ʱ2";
            dAQPoint.SN = "9";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "YL2";
            dAQPoint.Name = "ѹ��2";
            dAQPoint.SN = "10";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "WD2";
            dAQPoint.Name = "�¶�2";
            dAQPoint.SN = "11";
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "BKLJ2";
            dAQPoint.Name = "����ۼ�2";
            dAQPoint.SN = "12";
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "GKLJ2";
            dAQPoint.Name = "�����ۼ�2";
            dAQPoint.SN = "13";
            rtn.Add(dAQPoint);
            return rtn;
        }
        private ProductConfig CreateProduct()
        {
            ProductConfig productConfig = new ProductConfig();
            return productConfig;
        }
        [Fact]
        public void TestNewRequestReceived()
        {
            IDAQDrive dAQDrive = new DAQDrive();
            dAQDrive.Startup(CreateProduct(), CreateDevices(5000), CreateDAQService());
            byte[] data = new byte[] {
                0x01,0x03,0x1e,0x00,0x0C,0x00,0x00,0x47,0xAE,0x44,
                0x7A,0x5C,0xEC,0x46,0x09,0x05,0x1F,0x42,0xC8,0x99,
                0x9A,0x41,0xBB,0x00,0x00,0x42,0xC8,0x00,0x00,0x00,
                0x00,0x00,0x00,0x44,0x48,0x00,0x00,0x00,0x00,0x3D,
                0x71,0x42,0xC4,0x33,0x33,0x41,0x17,0x3D,0x71,0x42,
                0xC4,0x5C,0x29,0x41,0xC5,0x00,0x00,0x41,0x20,0x00,
                0x00,0x41,0xA0};
            ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(data);
            BaizeSession baizeSession = new BaizeSession(sequence);
            Assert.Equal(1, dAQDrive.NewRequestReceived(baizeSession));

        }
    }
}
