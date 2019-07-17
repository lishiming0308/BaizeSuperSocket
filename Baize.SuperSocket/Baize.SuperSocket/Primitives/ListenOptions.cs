using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;

namespace Baize.SuperSocket.Primitives
{
    /// <summary>
    /// 监听选项
    /// </summary>
    public class ListenOptions
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip
        {
            get;
            set;
        }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port
        {
            get;
            set;
        }
        /// <summary>
        /// 协议类型
        /// </summary>
        public ProtocolType ProtocolType
        {
            get;
            set;
        } = ProtocolType.TCP;

        /// <summary>
        /// 操作系统堆栈中，潜在可以接收的连接数量
        /// </summary>
        public int BackLog
        {
            get;
            set;
        } = 100;
        /// <summary>
        /// TPC网络包是否使用延时算法发送，已解决小数据包消耗过大问题，但是会产生延时。
        /// </summary>
        public bool NoDelay
        {
            get;
            set;
        }
        /// <summary>
        /// 通道选项
        /// </summary>
        public ChannelOptions ChannelOptions
        {
            get;
            set;
        } = new ChannelOptions();

    }
}
