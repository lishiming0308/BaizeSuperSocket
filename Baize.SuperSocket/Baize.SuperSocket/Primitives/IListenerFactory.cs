using Baize.SuperSocket.Channel;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Baize.SuperSocket.Primitives
{
    /// <summary>
    /// 监听端口工厂
    /// </summary>
    public interface IListenerFactory
    {
        /// <summary>
        /// 创建端口监听
        /// </summary>
        /// <param name="appSessions">会话字典</param>
        /// <param name="options">监听选项</param>
        /// <param name="loggerFactory">日志</param>
        /// <returns></returns>
        IListener CreateListener(ConcurrentDictionary<string, IAppSession> appSessions, ListenOptions options, ILoggerFactory loggerFactory);
    }
}
