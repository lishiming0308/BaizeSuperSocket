using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppSocket
{
    class Statistics
    {
        public DateTime Time
        {
            get;
            set;
        }
        public long SendCount
        {
            get;
            set;
        }
        public long ReceiveCount
        {
            get;
            set;
        }
    }
}
