using System;
using System.Collections.Generic;
using System.Text;

namespace LiveShow.Message
{
    public class UdpCallBackReq : UdpMessageBase
    {
        public UdpCallBackReq():base()
        {
            Cmd = ProxyCommand.Callback;
        }
        private UdpCmd _Body;
        public UdpCmd Body
        {
            get { return _Body; }
            set { _Body = value; }
        }

        protected override void RestoreBody(System.IO.BinaryReader reader)
        {

            _Body = new UdpCmd();
            _Body.Cmd = (ProxyCommand)reader.ReadUInt16();
            _Body.PeerID = reader.ReadUInt32();
            _Body.IP = reader.ReadUInt32();
            _Body.Port = reader.ReadUInt16();
            _Body.ChId = reader.ReadUInt32();
        }

        protected override void StoreBody(System.IO.BinaryWriter writer)
        {
            
            writer.Write((ushort)_Body.Cmd);
            writer.Write(_Body.PeerID);
            writer.Write(_Body.IP);
            writer.Write(_Body.Port);
            writer.Write(_Body.ChId);
        }

        
    }
}
