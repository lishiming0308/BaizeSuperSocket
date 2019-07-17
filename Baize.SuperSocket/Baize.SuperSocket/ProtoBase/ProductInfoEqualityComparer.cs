using Baize.IPlugin.SuperSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baize.SuperSocket.ProtoBase
{
    /// <summary>
    /// 产品信息分组比较器
    /// </summary>
    public class ProductInfoEqualityComparer : IEqualityComparer<BasePortocalFilterInfo>
    {
        public bool Equals(BasePortocalFilterInfo x, BasePortocalFilterInfo y)
        {
            bool rtn = false;
            if (x!=null && y!=null && x.ProtoType == y.ProtoType)
            {
                switch (x.ProtoType)
                {
                    case ProtoType.ShortMessage:
                        rtn = true;
                        break;
                    case ProtoType.FixHeaderSize:
                        rtn = false;
                        break;
                    case ProtoType.FixSize:
                        if ( x.Size == y.Size)
                        {
                            rtn = true;
                        }
                        break;
                    case ProtoType.Terminator:
                        if (x.Terminator !=null && y.Terminator !=null &&
                           Enumerable.SequenceEqual(x.Terminator,y.Terminator))
                        {
                            rtn = true;
                        }
                        break;
                    case ProtoType.BeginEndMark:
                        if (x.BeginMark !=null && y.BeginMark !=null &&
                              x.EndMark != null && y.EndMark != null &&
                            Enumerable.SequenceEqual(x.BeginMark,y.BeginMark) &&
                            Enumerable.SequenceEqual(x.EndMark,y.EndMark))
                        {
                            rtn = true;
                        }
                        break;
                }
            }
            return rtn;
        }

        public int GetHashCode(BasePortocalFilterInfo obj)
        {
            return obj.ProtoType.GetHashCode();
        }
    }
}
