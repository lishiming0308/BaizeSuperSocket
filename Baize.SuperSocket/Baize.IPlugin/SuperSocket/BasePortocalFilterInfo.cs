using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Baize.IPlugin.SuperSocket
{
    /// <summary>
    /// 基础协议过滤信息
    /// </summary>
    public class BasePortocalFilterInfo
    {
        /// <summary>
        /// 协议类型
        /// </summary>
        public ProtoType ProtoType
        {
            get;
            set;
        } = ProtoType.ShortMessage;
        /// <summary>
        /// 结束定位符
        /// </summary>
        public byte[] Terminator
        {
            get;
            set;
        }
        /// <summary>
        /// 起始符
        /// </summary>
        public byte[] BeginMark
        {
            get;
            set;
        }
        /// <summary>
        /// 结束符
        /// </summary>
        public byte[] EndMark
        {
            get;
            set;
        }
        /// <summary>
        /// 长度
        /// </summary>
        public int Size
        {
            get;
            set;
        }
        /// <summary>
        /// 根据头部数据内容获得整体报文长度
        /// </summary>
        public Func<ReadOnlySequence<byte>, int> GetBodyLengthFromHeader
        {
            get;
            set;
        }
    }
}
