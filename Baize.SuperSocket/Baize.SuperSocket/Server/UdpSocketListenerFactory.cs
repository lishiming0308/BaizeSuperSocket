using Baize.SuperSocket.Channel;
using Baize.SuperSocket.Primitives;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Baize.SuperSocket.Server
{
    /// <summary>
    /// UDP协议监听器创建工厂
    /// </summary>
    class UdpSocketListenerFactory: IListenerFactory
    {
        public IListener CreateListener(ConcurrentDictionary<string, IAppSession> appSessions, ListenOptions options, ILoggerFactory loggerFactory)
        {
            return new UdpSocketListener(options,
               (s,remotePoint) => {
                   if(!appSessions.ContainsKey(remotePoint.ToString()))
                   {
                       return new UdpPipeChannel(s, remotePoint,options.ChannelOptions, loggerFactory.CreateLogger(nameof(IChannel)));
                   }
                   else
                   {
                       return null;
                   }
  
               },
               loggerFactory.CreateLogger(nameof(UdpSocketListener)));
        }
    }
}
