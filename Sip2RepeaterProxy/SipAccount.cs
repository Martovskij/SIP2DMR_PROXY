using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sip2RepeaterProxy
{
    public class SipAccount
    {
        public String UserName { get; set; }
        public String Password { get; set; }
        public String SipServerIp { get; set; }
        public UInt16 SipServerPort { get; set; }

    }
}
