using System;
using System.Collections.Generic;
using System.Text;
using LiveShow.Message;

namespace LiveShow
{
    public struct UdpCmd
    {
        public ProxyCommand Cmd;
        public uint PeerID;
        public uint IP;
        public ushort Port;
        public uint ChId;
    }
}
