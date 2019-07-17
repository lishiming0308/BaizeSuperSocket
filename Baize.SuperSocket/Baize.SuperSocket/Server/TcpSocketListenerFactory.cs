using System.Collections.Concurrent;
using Baize.SuperSocket.Channel;
using Baize.SuperSocket.Primitives;
using Microsoft.Extensions.Logging;
namespace Baize.SuperSocket.Server
{
    /// <summary>
    /// TCP协议监听创建工厂
    /// </summary>
    public class TcpSocketListenerFactory : IListenerFactory
    {
        public IListener CreateListener(ConcurrentDictionary<string, IAppSession> appSessions, ListenOptions options, ILoggerFactory loggerFactory)
        {
            return new TcpSocketListener(options,
                (s) => {
                    return new TcpPipeChannel(s, options.ChannelOptions, loggerFactory.CreateLogger(nameof(IChannel)));
                },
                loggerFactory.CreateLogger(nameof(TcpSocketListener)));
        }
    }
}
