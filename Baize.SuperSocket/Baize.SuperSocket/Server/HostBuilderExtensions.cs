using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.Channel;
using Baize.SuperSocket.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baize.SuperSocket.Server
{
    /// <summary>
    /// SuperScoket服务主机扩展类
    /// </summary>
    public  static class HostBuilderExtensions
    {
        /// <summary>
        /// 向主机中添加SuperSocket服务
        /// </summary>
        /// <param name="hostBuilder">主机服务</param>
        /// <returns></returns>
        public static IHostBuilder UseSuperSocket(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(
                (hostCtx, services) =>
                {
                    services.AddSingleton<IServer, SuperSocketService>();
                    
                }
            );
            return hostBuilder;
        }
        /// <summary>
        /// 接收数据处理
        /// </summary>
        /// <param name="hostBuilder">扩展类</param>
        /// <param name="packageHandler">处理函数</param>
        /// <returns></returns>
        public static IHostBuilder ConfigurePackageHandler(this IHostBuilder hostBuilder, Func<IServer,List<string>, BaizeSession, Task> packageHandler)
        {
            if (packageHandler == null)
            {
                return hostBuilder;
            }
            return hostBuilder.ConfigureServices(
                (hostCtx, services) =>
                {
                    services.AddSingleton<Func<IServer, List<string>, BaizeSession, Task>>(packageHandler);
                  
                }
            );
        }
        /// <summary>
        /// 建立连接
        /// </summary>
        /// <param name="hostBuilder">扩展类</param>
        /// <param name="connectedHandler">处理函数</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureConnectedHandler(this IHostBuilder hostBuilder, Func<IServer,BaizeSession, ValueTask> connectedHandler)
        {
            if (connectedHandler == null)
            {
                return hostBuilder;
            }
            return hostBuilder.ConfigureServices(
                (hostCtx, services) =>
                {
                    services.AddSingleton<Func<IServer,BaizeSession,ValueTask>>(connectedHandler);

                }
            );
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="hostBuilder">扩展类</param>
        /// <param name="closedHandler">处理函数</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureClosedHandler(this IHostBuilder hostBuilder, Func<IServer,BaizeSession, CloseReason, ValueTask> closedHandler)
        {
            if (closedHandler == null)
            {
                return hostBuilder;
            }
            return hostBuilder.ConfigureServices(
                (hostCtx, services) =>
                {
                    services.AddSingleton<Func<IServer,BaizeSession, CloseReason,ValueTask>>(closedHandler);
                }
            );
        }
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="hostBuilder">主机</param>
        /// <param name="configurator">配置选项委托</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureSuperSocket(this IHostBuilder hostBuilder, Action<ServerOptions> configurator)
        {
            return hostBuilder.ConfigureServices(
                (hostCtx, services) =>
                {
                    services.Configure<ServerOptions>(configurator);
                }
            );
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IServer BuildAsServer(this IHostBuilder hostBuilder)
        {
            var host = hostBuilder.Build();
            return  host.Services.GetRequiredService<IServer>();
         
        }
    }
}
