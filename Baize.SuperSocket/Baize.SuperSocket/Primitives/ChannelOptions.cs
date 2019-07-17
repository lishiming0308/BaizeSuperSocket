using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.SuperSocket.Primitives
{
    /// <summary>
    /// 通道选项
    /// </summary>
    public class ChannelOptions
    {
        // 64K by default
        public int MaxPackageLength
        {
            get;
            set;
        } = 1024 * 64;

        // 4k by default
        public int ReceiveBufferSize
        {
            get;
            set;
        } = 1024 * 4;
    }
}
