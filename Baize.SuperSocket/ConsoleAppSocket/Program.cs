using Baize.IPlugin;
using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.Primitives;
using Baize.SuperSocket.ProtoBase;
using Baize.SuperSocket.Server;
using Driver.WriteModbus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSocket
{
    class Program
    {
        /// <summary>
        /// socket服务
        /// </summary>
        private static IServer server;
        /// <summary>
        /// 统一列表
        /// </summary>
        private static List<Statistics> _statistics = new List<Statistics>();
        /// <summary>
        /// 统计信息定时器
        /// </summary>
        private static System.Threading.Timer timer = new System.Threading.Timer(new System.Threading.TimerCallback(Statictis));
        static void Main(string[] args)
        {
            var productInfos = CreateRequestProductInfos();
            var drives = CreateDrive(productInfos);
            var devices = CreateDevices(50000);
            var daqService = CreateDAQService();
            DAQServerUtility.Instance.Init(drives);
            foreach (DriverInfo driveInfo in drives.Values)
            {
                driveInfo.DAQDrive.Startup(driveInfo.ProductConfig, devices, daqService);
            }
            DAQServerUtility.Instance.SendDataEvent += Instance_SendDataEvent;
            var builder = CreateSocketServerBuilder(daqService);
            server = builder
                .ConfigurePackageHandler(async (superServer, products,session) =>
                {
                    await Task.Factory.StartNew(() =>
                    {
                        foreach (string productOID in products)
                        {
                          Task.Factory.StartNew(()=>  DAQServerUtility.Instance.NewRequestReceived(productOID, session));
                        }
                    });

                })
                .ConfigureSuperSocket(p => {
                    p.ProductInfos = productInfos;
                })
                .BuildAsServer() as IServer;
            Console.WriteLine("启动监听30005!,输入exit退出,输入exp导出数据.");
            server.StartAsync();
            timer.Change(0, 1000);
            string cmd = "";
            while ( (cmd = Console.ReadLine()) !="exit")
            {
                if (cmd == "exp")
                {
                    ExportFileToCsv();
                }
            }
            server.StopAsync();
        }
        /// <summary>
        /// 每分钟统计一次信息
        /// </summary>
        /// <param name="obj"></param>
        private static void Statictis( object obj)
        {
            DateTime now = DateTime.Now;
            if (now.Second == 0)
            {
                _statistics.Add(new Statistics()
                {
                    ReceiveCount = DAQServerUtility.Instance.ReceiveDataCount,
                    SendCount = DAQServerUtility.Instance.SendDataCount,
                    Time = now
                });
            }
            if(now.Minute==0 && now.Second==0)
            {
                ExportFileToCsv();
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sessionID">会话ID</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        private static ValueTask<int> Instance_SendDataEvent(string sessionID, ReadOnlyMemory<byte> data)
        {
            return server.SendDataAsync(sessionID, data);
        }
        /// <summary>
        /// 创建主机
        /// </summary>
        /// <param name="dAQService"></param>
        /// <returns></returns>
        protected static IHostBuilder CreateSocketServerBuilder(DAQService dAQService)
        {
            return CreateSocketServerBuilderBase(dAQService).UseSuperSocket();
        }
        /// <summary>
        /// 创建产品列表
        /// </summary>
        /// <returns></returns>
        private static List<ProductConfig> CreateRequestProductInfos()
        {
            List<ProductConfig> productInfos = new List<ProductConfig>();
            ProductConfig productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = new byte[] { },
                    EndMark = new byte[] { },
                    ProtoType = ProtoType.ShortMessage,
                    Size = 0,
                    Terminator = new byte[] { }
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfos.Add(productInfo);
            return productInfos;
        }
        /// <summary>
        /// 创建插件列表
        /// </summary>
        /// <param name="productConfigs">产品配置列表</param>
        /// <returns></returns>
        private static ConcurrentDictionary<string, Baize.IPlugin.Model.DriverInfo> CreateDrive(List<ProductConfig> productConfigs)
        {
            ConcurrentDictionary<string, DriverInfo> rtn = new ConcurrentDictionary<string, DriverInfo>();
            foreach(ProductConfig productConfig in productConfigs)
            {
                DriverInfo driveInfo = new DriverInfo();
                driveInfo.ProductConfig = productConfig;
                driveInfo.DAQDrive = new DAQDrive();
                rtn.TryAdd(driveInfo.ProductConfig.ProductOID, driveInfo);
            }
            return rtn;
        }
        /// <summary>
        /// 创建设备
        /// </summary>
        /// <param name="devNumber"></param>
        /// <returns></returns>
        private static ConcurrentDictionary<string,DAQDevice> CreateDevices(int devNumber)
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
        /// <summary>
        /// 创建采集服务
        /// </summary>
        /// <returns></returns>
        private static DAQService CreateDAQService()
        {
            DAQService dAQService = new DAQService();
            dAQService.Port = 30005;
            return dAQService;
        }
        /// <summary>
        /// 创建采集点
        /// </summary>
        /// <returns></returns>
        private static List<DAQPoint> CreatePoints()
        {
            List<DAQPoint> rtn = new List<DAQPoint>();
            DAQPoint dAQPoint = new DAQPoint();
            dAQPoint.Code = "online";
            dAQPoint.Name = "在线状态";
            dAQPoint.SN = "0";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "LastTime";
            dAQPoint.Name = "最新时间";
            dAQPoint.SN = "1";
            //第一路流量计
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "BKSS1";
            dAQPoint.Name = "标况瞬时1";
            dAQPoint.SN = "2";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "GKSS1";
            dAQPoint.Name = "工况瞬时1";
            dAQPoint.SN = "3";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "YL1";
            dAQPoint.Name = "压力1";
            dAQPoint.SN = "4";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "WD1";
            dAQPoint.Name = "温度1";
            dAQPoint.SN = "5";
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "BKLJ1";
            dAQPoint.Name = "标况累计1";
            dAQPoint.SN = "6";
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "GKLJ1";
            dAQPoint.Name = "工况累计1";
            dAQPoint.SN = "7";
            rtn.Add(dAQPoint);
            //第二路流量计
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "BKSS2";
            dAQPoint.Name = "标况瞬时2";
            dAQPoint.SN = "8";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "GKSS2";
            dAQPoint.Name = "工况瞬时2";
            dAQPoint.SN = "9";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "YL2";
            dAQPoint.Name = "压力2";
            dAQPoint.SN = "10";
            rtn.Add(dAQPoint);
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "WD2";
            dAQPoint.Name = "温度2";
            dAQPoint.SN = "11";
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "BKLJ2";
            dAQPoint.Name = "标况累计2";
            dAQPoint.SN = "12";
            dAQPoint = new DAQPoint();
            dAQPoint.Code = "GKLJ2";
            dAQPoint.Name = "工况累计2";
            dAQPoint.SN = "13";
            rtn.Add(dAQPoint);
            return rtn;
        }
        /// <summary>
        /// 创建Socket服务
        /// </summary>
        /// <param name="dAQService"></param>
        /// <returns></returns>
        protected static IHostBuilder CreateSocketServerBuilderBase(DAQService dAQService)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((hostCtx, configApp) =>
                {
                    configApp.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "serverOptions:name", "TestServer" },
                        { "serverOptions:listeners:0:ip", "Any" },
                        { "serverOptions:listeners:0:port", dAQService.Port.ToString() },
                        { "serverOptions:listeners:0:BackLog","9048"},
                        { "serverOptions:ProtoType","0"}
                    });
                })
                .ConfigureLogging((hostCtx, loggingBuilder) =>
                {
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                })
                .ConfigureServices((hostCtx, services) =>
                {
                    services.AddOptions();
                    services.Configure<ServerOptions>(hostCtx.Configuration.GetSection("serverOptions"));

                });
            return hostBuilder;
        }
        /// <summary>
        /// 导出数据到csv文件
        /// </summary>
        private static void ExportFileToCsv()
        {

            ExportData(DateTime.Now.ToString("yyyyMMddHHmm") + ".csv");
        }
        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="fileName">导出文件名称</param>
        private static void ExportData(string fileName)
        {

            using (StreamWriter streamWriter = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" 时间 ").Append(","); //没一个字段后面都加逗号，表示是一列，因为这是第一行    因此也是列标题
                sb.Append(" 发送次数 ").Append(",");
                sb.Append(" 接收次数 ").Append(",");
                streamWriter.WriteLine(sb.ToString());
                //要写的数据源
                foreach (Statistics model in _statistics)
                {
                    sb = new StringBuilder();
                    sb.Append(model.Time + "").Append(",");
                    sb.Append(model.SendCount + "").Append(",");
                    sb.Append(model.ReceiveCount + "").Append(",");
                    streamWriter.WriteLine(sb.ToString());
                }
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

    }
    public static class Extensions
    {
        public static string GetString(this ReadOnlySequence<byte> buffer, Encoding encoding)
        {
            if (buffer.IsSingleSegment)
            {
                return encoding.GetString(buffer.First.Span);
            }

            return string.Create((int)buffer.Length, buffer, (span, sequence) =>
            {
                foreach (var segment in sequence)
                {
                    var count = encoding.GetChars(segment.Span, span);
                    span = span.Slice(count);
                }
            });
        }
    }
}
