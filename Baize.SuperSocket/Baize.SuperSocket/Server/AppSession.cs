using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.Channel;
using Baize.SuperSocket.Primitives;
using System;
using System.Net;

namespace Baize.SuperSocket.Server
{
    /// <summary>
    /// 通道会话
    /// </summary>
    class AppSession : IAppSession
    {
        internal AppSession( IChannel channel)
        {
            Channel = channel;
            string sessionID = "";
            string remoteIpEndPoint = "";

            if (channel.Socket.ProtocolType == System.Net.Sockets.ProtocolType.Udp)
            {
                sessionID = channel.RemoteIPEndPoint.ToString();
                remoteIpEndPoint =sessionID;
            }
            else
            {
                sessionID = Guid.NewGuid().ToString();
                remoteIpEndPoint = channel.RemoteIPEndPoint.ToString();
            }
            Session = new BaizeSession(sessionID);
            Session.RemoteIpEndPoint = remoteIpEndPoint;
            IPEndPoint iPEndPoint = (IPEndPoint)channel.Socket.LocalEndPoint;
            Session.LocalIp = iPEndPoint.Address.ToString();
            Session.LocalPort = iPEndPoint.Port;
            Session.ProtocolType = (ProtocolType)channel.Socket.ProtocolType;
        }
        public IChannel Channel { get; internal set; }

        public BaizeSession Session { get; internal set; }
        /// <summary>
        /// 关闭会话
        /// </summary>
        public void Close()
        {
            Channel.Close();
            Channel = null;
        }
    }
}
