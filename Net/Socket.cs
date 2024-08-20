using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SML.Net
{
    public abstract class Socket
    {
        public RemoteAddress RemoteAddress { get;protected set; }
    }
}
