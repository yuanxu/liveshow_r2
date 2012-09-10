using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LiveShow.Message
{
    /// <summary>
    /// Udp Hello Message
    /// </summary>
    public class UdpHelloReq:UdpMessageBase
    {
        public UdpHelloReq():base()
        {
            Cmd = ProxyCommand.Hello;
        }

        protected override void RestoreBody(BinaryReader reader)
        {

        }
        
        protected override void StoreBody(BinaryWriter writer)
        {
            return;
        }
    }

    /********************************************************/
    
    public class UdpHelloResp : UdpMessageBase
    {
        public UdpHelloResp():base()
        {
            Cmd = ProxyCommand.Hello_R;
            //Cmds = new List<UdpCmd>();
        }

       
        public IDictionary<int,UdpCmd> Cmds ;

        protected override void RestoreBody(System.IO.BinaryReader reader)
        {
            throw new NotImplementedException();
            //try
            //{
            //    m_SID = reader.ReadUInt32();
            //    while (reader.BaseStream.Position < reader.BaseStream.Length)
            //    {
            //        UdpCmd _UdpCmd = new UdpCmd();
            //        _UdpCmd.Cmd = (ProxyCommand)reader.ReadUInt16();
            //        _UdpCmd.SID = reader.ReadUInt32();
            //        _UdpCmd.IP = reader.ReadUInt32();
            //        _UdpCmd.Port = reader.ReadUInt16();
            //        _UdpCmd.CHID = reader.ReadUInt32();
            //        Cmds.Add(_UdpCmd);
            //    }
            //}
            //catch (EndOfStreamException)
            //{
            //}
        }

        protected override void StoreBody(System.IO.BinaryWriter writer)
        {
            writer.Write(PeerId);
            foreach(UdpCmd item in Cmds.Values)
            {
                writer.Write((uint)item.Cmd);
                writer.Write(item.PeerID);
                writer.Write(item.IP);
                writer.Write(item.Port);
                writer.Write(item.ChId);
            }
        }
        
    }
}
