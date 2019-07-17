using Baize.IPlugin.SuperSocket;
using System;
using System.Threading.Tasks;

namespace Baize.SuperSocket.Primitives
{
    public interface IServer 
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        Task<bool> StartAsync();
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        Task StopAsync();
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sessionID">会话ID</param>
        /// <param name="buffer">发送数据</param>
        /// <returns></returns>
        ValueTask<int> SendDataAsync(string sessionID, ReadOnlyMemory<byte> buffer);
        /// <summary>
        /// 创建连接远程服务器的会话
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口</param>
        /// <param name="protocolType">协议</param>
        /// <returns>成功返回会话，否则返回空</returns>
        ValueTask<BaizeSession> CreateClientAsync(string ip,int port,ProtocolType protocolType=ProtocolType.TCP);
        /// <summary>
        /// 关闭会话
        /// </summary>
        /// <param name="sessionID">会话ID</param>
        /// <returns></returns>
        bool CloseSession(string sessionID);
        /// <summary>
        /// 会话数据数量
        /// </summary>
        int SessionCount { get; }

       
    }
}
