# BaizeSuperSocket
#.netCore2.2版本的baize平台socket服务
#下边是使用的一段代码摘要，具体可以看看测试用例

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
