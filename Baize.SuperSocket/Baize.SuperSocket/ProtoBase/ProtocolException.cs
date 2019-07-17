using System;
using System.Collections.Generic;
using System.Text;

namespace Baize.SuperSocket.ProtoBase
{
    public class ProtocolException : Exception
    {
        public ProtocolException(string message, Exception exception)
            : base(message, exception)
        {

        }

        public ProtocolException(string message)
            : base(message)
        {

        }
    }
}
