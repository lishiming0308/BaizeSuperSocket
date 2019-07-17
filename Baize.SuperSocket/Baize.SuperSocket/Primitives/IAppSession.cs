using Baize.IPlugin;
using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.Channel;
using Baize.SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.SuperSocket.Primitives
{
    /// <summary>
    /// 通道会话
    /// </summary>
    public interface IAppSession
    {
        /// <summary>
        /// 通道
        /// </summary>
        IChannel Channel { get; }
        /// <summary>
        /// 会话数据
        /// </summary>
        BaizeSession Session { get; }
        void Close();
        
    }
}
