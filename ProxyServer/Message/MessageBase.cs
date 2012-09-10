using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LiveShow.Message
{
    public enum ProxyCommand:uint 
    {
        Unknown = 0x00000000,//未知
        Connect = 0x00020002,
        Hello = 0x00020003,
        Callback = 0x00020004,	//call back request
        Leave = 0x00020005,
        Hit = 0x00020006,//心跳记录.

        Connect_R = 0x80020002,
        Hello_R = 0x80020003,	//Shakehands
        Callback_R = 0x80020004,	//call back request
        
    }
    public abstract class MessageBase
    {

        public MessageBase()
        {
            _Cmd = ProxyCommand.Unknown;
            _Buffer = null;
        }

        #region Properties

        protected  ProxyCommand _Cmd;
        protected uint _SeqID;
        protected uint _ChID;
        private uint _PeerId;

        public  uint PeerId
        {
            get { return _PeerId; }
            set { _PeerId = value; }
        }
        protected uint _Len;
        protected byte[] _Buffer;
        

        protected uint ChID
        {
            get { return _ChID; }
            set { _ChID = value; }
        }

        public ProxyCommand Cmd
        {
            get { return _Cmd; }
            set { _Cmd = value; }
        }

        public uint SeqID
        {
            get { return _SeqID; }
            set { _SeqID = value; }
        }

        public uint Len
        {
            get { return _Len; }
            set { _Len = value; }
        }
        public byte[] Buffer
        {
            get { return _Buffer; }
        }
        #endregion

        /// <summary>
        /// 从流中恢复消息体
        /// </summary>
        /// <param name="reader"></param>
     
        protected abstract void RestoreBody(BinaryReader reader);

        /// <summary>
        /// 将消息体存入流中
        /// </summary>
        /// <param name="writer"></param>
        protected abstract void StoreBody(BinaryWriter writer);
        

        /// <summary>
        /// 从字节数组恢复
        /// </summary>
        /// <param name="Buf"></param>
        public void FromBytes(byte[] Buf)
        {
            _Buffer = new byte[Buf.Length];
            Array.Copy(Buf, _Buffer, Buf.Length);

            MemoryStream _Stream = new MemoryStream(_Buffer);
            BinaryReader _Reader = new BinaryReader(_Stream);

            _Cmd = (ProxyCommand)_Reader.ReadUInt32();
            _SeqID = _Reader.ReadUInt32();
            _ChID = _Reader.ReadUInt32();
            _PeerId = _Reader.ReadUInt32();
            _Len = _Reader.ReadUInt32();
            

            RestoreBody(_Reader);//此函数已经由派生类提供

            _Reader.Close();
            //System.Diagnostics.Debug.Assert(_Len= Buf.Length);
        }
      
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        public  void ToBytes()
        {
            MemoryStream _Stream = new MemoryStream();
        
            
            BinaryWriter _Writer = new BinaryWriter(_Stream);

            _Writer.Write((uint)_Cmd);
            _Writer.Write(_SeqID);
            
            _Writer.Write(_ChID);
            _Writer.Write(_PeerId);
            _Writer.Write(_Len);
            StoreBody(_Writer); //此函数已经由派生类提供
            _Buffer = _Stream.ToArray();
            Array.Copy(BitConverter.GetBytes((ushort)_Buffer.Length), 0, _Buffer, 4, 2); //修复Len
        }

    }

    public abstract class UdpMessageBase : MessageBase
    {

    }

    public abstract class MessageNoBody:MessageBase
    {
        protected override void RestoreBody(BinaryReader reader)
        {
            return;
        }

        protected override void StoreBody(BinaryWriter writer)
        {
            return;
        }
    }

    public abstract class UdpMessageNoBody : UdpMessageBase
    {
        protected override void RestoreBody(BinaryReader reader)
        {
            return;
        }

        protected override void StoreBody(BinaryWriter writer)
        {
            return;
        }
       
    }
 
}
