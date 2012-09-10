using System;
using System.Collections.Generic;
using System.Text;

namespace LiveShow.Message
{
    public class UdpLeaveReq:UdpMessageNoBody
    {
        public UdpLeaveReq():base()
        {
            Cmd = ProxyCommand.Leave;
        }
        
    }
}
