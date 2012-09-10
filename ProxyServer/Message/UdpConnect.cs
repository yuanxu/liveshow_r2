using System;
using System.Collections.Generic;
using System.Text;

namespace LiveShow.Message
{
    public class UdpConnect:Message.MessageBase
    {
       
        public IList<uint> Ips;
        protected override void RestoreBody(System.IO.BinaryReader reader)
        {
            Ips = new List<uint>();
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                uint info = reader.ReadUInt32();
                //info.Port = reader.ReadUInt16();
                Ips.Add(info);
            }
        }

        protected override void StoreBody(System.IO.BinaryWriter writer)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        
    }

    public class UdpConnectR : Message.MessageBase
    {
        public uint RemoteIp;
        public uint peer_id_;
        protected override void RestoreBody(System.IO.BinaryReader reader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void StoreBody(System.IO.BinaryWriter writer)
        {
            writer.Write(RemoteIp);
            writer.Write(peer_id_);
        }
    }
}
