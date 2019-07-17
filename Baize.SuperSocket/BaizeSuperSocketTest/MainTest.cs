using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.Primitives;
using Baize.SuperSocket.ProtoBase;
using Baize.SuperSocket.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BaizeSuperSocketTest
{
    public class MainTest
    {
        protected IHostBuilder CreateTcpSocketServerBuilder()
        {
            return CreateTcpSocketServerBuilderBase().UseSuperSocket();
        }
        protected IHostBuilder CreateTcpSocketServerBuilderBase()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((hostCtx, configApp) =>
                {
                    configApp.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "serverOptions:name", "BaizServerTcp" },
                        { "serverOptions:listeners:0:ip", "Any" },
                        { "serverOptions:listeners:0:port", "14040" },
                        { "serverOptions:listeners:0:protocoltype","6"}
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
        protected IHostBuilder CreateMultiSocketServerBuilderBase()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((hostCtx, configApp) =>
                {
                    configApp.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "serverOptions:name", "BaizServerTcp" },
                        { "serverOptions:listeners:0:ip", "Any" },
                        { "serverOptions:listeners:0:port", "14040" },
                        { "serverOptions:listeners:0:protocoltype","6"},
                        { "serverOptions:listeners:1:ip", "Any" },
                        { "serverOptions:listeners:1:port", "14040" },
                        { "serverOptions:listeners:1:protocoltype","17"}
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
        protected IHostBuilder CreateUdpSocketServerBuilder()
        {
            return CreateUdpSocketServerBuilderBase().UseSuperSocket();
        }
        protected IHostBuilder CreateUdpSocketServerBuilderBase()
        {
           
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((hostCtx, configApp) =>
                {
                    configApp.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "serverOptions:name", "BaizServerUdp" },
                        { "serverOptions:listener:ip", "Any" },
                        { "serverOptions:listener:port", "14040" },
                        { "serverOptions:ProtoType","1"}
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

        private List<ProductConfig> CreateRequestProductInfos()
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
            productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = new byte[] { },
                    EndMark = new byte[] { },
                    ProtoType = ProtoType.ShortMessage,
                    Size = 10,
                    Terminator = new byte[] { }
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfos.Add(productInfo);
            return productInfos;
        }
        private static ServerOptions CreateServerOptions(Baize.IPlugin.SuperSocket.ProtocolType protocolType)
        {
            return new ServerOptions()
            {
                Listeners = new List<ListenOptions>(){ new ListenOptions()
                {
                    BackLog = 100,
                    ChannelOptions = new ChannelOptions()
                    {
                        MaxPackageLength = 4096,
                        ReceiveBufferSize = 1024
                    },
                    Ip = "Any",
                    NoDelay = true,
                    Port = 18080,
                     ProtocolType=protocolType
                }
                },
                Name = "BaizeServerUdp",
                ProductInfos = new List<ProductConfig>() { new ProductConfig(){
                        BasePortocalFilterInfo=new BasePortocalFilterInfo()
                        {
                             ProtoType=ProtoType.ShortMessage
                        },
                         ProductOID="1"
                   },
                   new ProductConfig(){
                        BasePortocalFilterInfo=new BasePortocalFilterInfo()
                        {
                             ProtoType=ProtoType.ShortMessage
                        },
                         ProductOID="2"
                   },
                   }

            };
        }
        [Fact]
        public async void TestSinglePortTCP()
        {
            Debug.Print($"当前线程ID:{Thread.CurrentThread.ManagedThreadId}");
            var builder = CreateTcpSocketServerBuilder();
            var server = builder
                .ConfigureConnectedHandler( async (superServer, session) => {
                   // return await true;
                   // await superServer.SendDataAsync(session.SessionID, Encoding.Default.GetBytes($"Connected RemoteIP:{ session.RemoteIpEndPoint}\r\n"));
                })
                .ConfigurePackageHandler(async (superServer, products,session) =>
                {
                    Debug.Print($"当前线程ID:{Thread.CurrentThread.ManagedThreadId}");
                    string msg = session.Data.GetString(Encoding.Default);
                   int rtn= await superServer.SendDataAsync(session.SessionID, Encoding.Default.GetBytes("Hello World\r\n"));

                })
                .ConfigureClosedHandler(async (superServer, session, closeReason) => {
                      Console.WriteLine($"会话关闭:{session.SessionID}");
                    await Task.FromResult(true);
                })
                .BuildAsServer() as IServer;
            await server.StartAsync();
            var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            await client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14040));
            using (var stream = new NetworkStream(client))
            using (var streamReader = new StreamReader(stream, Encoding.ASCII, true))
            using (var streamWriter = new StreamWriter(stream, Encoding.ASCII, 1024 * 1024 * 4))
            {
                await streamWriter.WriteAsync("#Hello World\r\n#");
                await streamWriter.FlushAsync();
                var line = await streamReader.ReadLineAsync();
                Assert.Equal("Hello World", line);
            }
            await server.StopAsync();
        }
        [Fact]
        public void TestSinglePortUDP()
        {
            var builder = CreateUdpSocketServerBuilder();
            ServerOptions serverOptions = CreateServerOptions(Baize.IPlugin.SuperSocket.ProtocolType.UDP);
            var server = builder
                 .ConfigureSuperSocket(p =>
                 {
                     p.Listeners = serverOptions.Listeners;
                     p.Name = serverOptions.Name;
                 })
                .ConfigureConnectedHandler(async (superServer, session) =>
                {

                    await superServer.SendDataAsync(session.SessionID, Encoding.Default.GetBytes($"Connected RemoteIP:{ session.RemoteIpEndPoint}\r\n"));
                })
                .ConfigurePackageHandler(async (superServer, products,session) =>
                {
                    //string msg = session.Data.GetString(Encoding.Default);
                    await superServer.SendDataAsync(session.SessionID, session.Data.First);

                })
                .ConfigureClosedHandler(async (superServer, session, closeReason) =>
                {
                    Console.WriteLine($"会话关闭:{session.SessionID}");
                    await Task.FromResult(true);
                })

                .BuildAsServer() as IServer;
            server.StartAsync();
        }

        /// <summary>
        /// 测试多端口
        /// </summary>
        [Fact]
        public async void TestMultiPorts()
        {
            Debug.Print($"当前线程ID:{Thread.CurrentThread.ManagedThreadId}");
            var builder = CreateMultiSocketServerBuilderBase().UseSuperSocket();
            var server = builder
                .ConfigureConnectedHandler(async (superServer, session) => {

                    // await superServer.SendDataAsync(session.SessionID, Encoding.Default.GetBytes($"Connected RemoteIP:{ session.RemoteIpEndPoint}\r\n"));
                })
                .ConfigurePackageHandler(async (superServer, products,session) =>
                {
                    Debug.Print($"当前线程ID:{Thread.CurrentThread.ManagedThreadId}");
                    string msg = session.Data.GetString(Encoding.Default);
                    int rtn = await superServer.SendDataAsync(session.SessionID, Encoding.Default.GetBytes("Hello World\r\n"));

                })
                .ConfigureClosedHandler(async (superServer, session, closeReason) => {
                    Console.WriteLine($"会话关闭:{session.SessionID}");
                    await Task.FromResult(true);
                })
                .BuildAsServer() as IServer;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int k = 0; k <100; k++)
            {
                await server.StartAsync();
                var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                await client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14040));
                using (var stream = new NetworkStream(client))
                using (var streamReader = new StreamReader(stream, Encoding.ASCII, true))
                using (var streamWriter = new StreamWriter(stream, Encoding.ASCII, 1024 * 1024 * 4))
                {
                    await streamWriter.WriteAsync("#Hello World\r\n#");
                    await streamWriter.FlushAsync();
                    var line = await streamReader.ReadLineAsync();
                    Assert.Equal("Hello World", line);
                }

                var udpclient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
                EndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14040);
                udpclient.SendTo(Encoding.ASCII.GetBytes("hello world udp"), iPEndPoint);
                byte[] receiveData = new byte[1024];
                int i = udpclient.ReceiveFrom(receiveData, ref iPEndPoint);
                string msg1 = Encoding.ASCII.GetString(receiveData, 0, i);
                Assert.Equal("Hello World\r\n", msg1);
                await server.StopAsync();
            }
            Debug.Print($"耗时:{stopwatch.Elapsed.Seconds}");
        }
        /// <summary>
        /// 测试连接远程TCP服务
        /// </summary>
        [Fact]
        public void TestTcpClient()
        {
            var builder = CreateMultiSocketServerBuilderBase().UseSuperSocket();
            var server = builder
                .ConfigureConnectedHandler(async (superServer, session) => {

                     await superServer.SendDataAsync(session.SessionID, Encoding.Default.GetBytes($"Connected RemoteIP:{ session.RemoteIpEndPoint}\r\n"));
                })
                .ConfigurePackageHandler(async (superServer, products, session) =>
                {
                    Debug.Print($"当前线程ID:{Thread.CurrentThread.ManagedThreadId}");
                    string msg = session.Data.GetString(Encoding.Default);
                    int rtn = await superServer.SendDataAsync(session.SessionID, Encoding.Default.GetBytes("Hello World\r\n"));

                })
                .ConfigureClosedHandler(async (superServer, session, closeReason) => {
                    Console.WriteLine($"会话关闭:{session.SessionID}");
                    await Task.FromResult(true);
                })
                .BuildAsServer() as IServer;
            server.StartAsync();
            for (int i = 0; i < 100; i++)
            {
                var task = server.CreateClientAsync("127.0.0.1", 14040);
                BaizeSession baizeSession = task.Result;
                if (baizeSession != null)
                {
                    server.SendDataAsync(baizeSession.SessionID, Encoding.ASCII.GetBytes("hello world"));
                    server.CloseSession(baizeSession.SessionID);
                }
            }
        }
        /// <summary>
        /// 测试连接远程UdP服务
        /// </summary>
        [Fact]
        public void TestUdpClient()
        {
            var builder = CreateMultiSocketServerBuilderBase().UseSuperSocket();
            var server = builder
                .ConfigureConnectedHandler(async (superServer, session) => {

                    await superServer.SendDataAsync(session.SessionID, Encoding.Default.GetBytes($"Connected RemoteIP:{ session.RemoteIpEndPoint}\r\n"));
                })
                .ConfigurePackageHandler(async (superServer, products, session) =>
                {
                    Debug.Print($"当前线程ID:{Thread.CurrentThread.ManagedThreadId}");
                    string msg = session.Data.GetString(Encoding.Default);
                    int rtn = await superServer.SendDataAsync(session.SessionID, Encoding.Default.GetBytes("Hello World\r\n"));

                })
                .ConfigureClosedHandler(async (superServer, session, closeReason) => {
                    Console.WriteLine($"会话关闭:{session.SessionID}");
                    await Task.FromResult(true);
                })
                .BuildAsServer() as IServer;
            server.StartAsync();
            for (int i = 0; i < 100; i++)
            {
                var task = server.CreateClientAsync("127.0.0.1", 14040, Baize.IPlugin.SuperSocket.ProtocolType.UDP);
                BaizeSession baizeSession = task.Result;
                if (baizeSession != null)
                {
                    server.SendDataAsync(baizeSession.SessionID, Encoding.ASCII.GetBytes("hello world"));

                }
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
        public static int GetInt32(this ReadOnlySequence<byte> buffer,int index)
        {
            int rtn = 0;
            if (buffer.IsSingleSegment)
            {
                rtn = BitConverter.ToInt32(buffer.Slice(index, 4).First.Span);
            }
            else
            {
                if (buffer.Slice(index, 4).IsSingleSegment)
                {
                    rtn = BitConverter.ToInt32(buffer.Slice(index, 4).First.Span);
                }
                else
                {
                    rtn = BitConverter.ToInt32(buffer.Slice(index, 4).ToArray());
                }
            }
            return rtn;
        }
        public static int GetInt16(this ReadOnlySequence<byte> buffer, int index)
        {
            int rtn = 0;
            if (buffer.IsSingleSegment)
            {
                rtn = BitConverter.ToInt16(buffer.Slice(index, 2).First.Span);
            }
            else
            {
                if (buffer.Slice(index, 2).IsSingleSegment)
                {
                    rtn = BitConverter.ToInt16(buffer.Slice(index, 2).First.Span);
                }
                else
                {
                    rtn = BitConverter.ToInt16(buffer.Slice(index, 2).ToArray());
                }
            }
            return rtn;
        }
    }
}
