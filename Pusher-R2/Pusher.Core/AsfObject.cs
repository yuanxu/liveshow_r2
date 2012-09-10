using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ankh.Pusher.Core
{
    public class AsfHeader
    {
        private uint m_MinPacketSize, m_MaxPacketSize, m_MaxBitrate, m_SeqNumber;
        private TimeSpan m_Duration, m_SendDuration;

        public TimeSpan SendDuration
        {
            get { return m_SendDuration; }
            
        }

        public TimeSpan Duration
        {
            get { return m_Duration; }
            
        }
        public uint SeqNumber
        {
            get { return m_SeqNumber; }
            set { m_SeqNumber = value; }
        }
        private ulong m_PacketCount;
        private byte[] m_Data;

        const int  HDR_TOTAL_SIZE_8    =           	0x28;
        const int  HDR_NUM_PACKETS_8    =          	0x38;
        const int HDR_FINE_TOTALTIME_8   =        	0x40;
        const int HDR_FINE_PLAYTIME_8     =       	0x48;
        const int HDR_PLAYTIME_OFFSET_4    =      	0x50;
        const int HDR_FLAGS_4               =     	0x58;
        const int HDR_ASF_CHUNKLENGTH_4      =    	0x5c;
        const int HDR_ASF_CHUNKLENGTH_CONFIRM_4 = 	0x60;
        
        const int DATSEG_HDR_SIZE 	=	0x32;
        const int DATSEG_NUMCHUNKS_4 =		0x28;
        const int HDR_MIN_PACKET_SIZE_4 = 0x5c;
        const int HDR_MAX_PACKET_SIZE_4 = 0x60;
        const int HDR_ASF_BITRATE = 0x64;

        const int HDR_IDX_LENGTH = 16;

        public byte[] Data
        {
            get { return m_Data; }
            set 
            { 
                
                //分析asf头
                int offset = 30;//过虑掉header object
                
                
                Guid fileID=new Guid(new byte[]{0xA1 ,0xDC ,0xAB ,0x8C ,0x47 ,0xA9 ,0xCF ,0x11,0x8E ,0xE4 ,0x00 ,0xC0 ,0x0C ,0x20 ,0x53 ,0x65});
                offset = Find(value, fileID);

                m_MinPacketSize = BitConverter.ToUInt32(value, offset + HDR_MIN_PACKET_SIZE_4);
                m_MaxPacketSize = BitConverter.ToUInt32(value, offset + HDR_MAX_PACKET_SIZE_4);
                m_MaxBitrate = BitConverter.ToUInt32(value, offset + HDR_ASF_BITRATE);
                m_PacketCount = BitConverter.ToUInt64(value, offset + HDR_NUM_PACKETS_8);
                m_Duration = new TimeSpan(BitConverter.ToInt64(value, offset+HDR_FINE_TOTALTIME_8));
                m_SendDuration = new TimeSpan(BitConverter.ToInt64(value,offset+HDR_FINE_PLAYTIME_8));
                value[offset + HDR_FLAGS_4] = (byte)0x08;
                //Array.Copy(BitConverter.GetBytes((ulong)0),0,m_Data,offset+HDR_TOTAL_SIZE_8,8);
                Array.Copy(BitConverter.GetBytes((ulong)0), 0, value, offset + HDR_FINE_PLAYTIME_8, 8);
                Array.Copy(BitConverter.GetBytes((ulong)0), 0, value, offset + HDR_FINE_TOTALTIME_8, 8);
                //Array.Copy(BitConverter.GetBytes((ulong)0), 0, m_Data, offset + HDR_NUM_PACKETS_8, 8);
                //Array.Copy(BitConverter.GetBytes((uint)0), 0, m_Data, offset + HDR_PLAYTIME_OFFSET_4,4);

                //去掉Index Object
                Guid IndexID = new Guid(new byte[] { 0x20, 0xDE, 0xAA, 0xD9, 0x17, 0x7C, 0x9C, 0x4F, 0xBC, 0x28, 0x85, 0x55, 0xDD, 0x98, 0xE2, 0xA2 });
                offset = Find(value, IndexID);

                int IdxLen = offset == 0 ? 0 : (int)BitConverter.ToUInt64(value, offset + HDR_IDX_LENGTH);

                m_Data = new byte[value.Length - IdxLen];
                if (offset == 0) //not find index object
                    Buffer.BlockCopy(value, 0, m_Data, 0, value.Length);
                else
                {
                    Buffer.BlockCopy(value, 0, m_Data, 0, offset);
                    Buffer.BlockCopy(value, offset + IdxLen, m_Data, offset, value.Length - offset - IdxLen);
                }
            }
        }
        private int Find(byte[] data, Guid guid)
        {
            int offset = 30;
            byte[] buf_t = new byte[16];
            while (data.Length >= offset+16)
            {
                Array.Copy(data, offset, buf_t, 0, 16);
                Guid g = new Guid(buf_t);
                if (guid == g)
                    break;
                offset += (int)BitConverter.ToUInt64(data, offset + 16);
            }
            return data.Length <= offset  ? 0 : offset;
        }
        public ulong PacketCount
        {
            get { return m_PacketCount; }
            set { m_PacketCount = value; }
        }
        public uint MaxBitrate
        {
            get { return m_MaxBitrate; }
            set { m_MaxBitrate = value; }
        }

        public uint MaxPacketSize
        {
            get { return m_MaxPacketSize; }
            set { m_MaxPacketSize = value; }
        }

        public uint MinPacketSize
        {
            get { return m_MinPacketSize; }
            set { m_MinPacketSize = value; }
        }

        public int PacketsPerSec
        {
            get { return (int)(m_MaxBitrate / (8 * m_MaxPacketSize)); }
        }

        /// <summary>
        /// 从文件初始化头属性
        /// </summary>
        /// <param name="file"></param>
        public void InitFromFile(string file)
        {
            const int OBJECT_ID_LEN = 16;
            const int OBJECT_SIZE_LEN = 64;
            const int DATA_HEADER_LEN = 0x32;

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(fileStream);

            //处理头
            reader.ReadBytes(OBJECT_ID_LEN); //过虑掉ASF Header Object ID
            int len = (int)reader.ReadUInt64();//asf header size
            reader.BaseStream.Position = 0;
            Data = reader.ReadBytes(len + DATA_HEADER_LEN); //读取头数据

            if (m_Duration.TotalSeconds == 0) // 无索引时
            {
                int  iCnt = (int)((reader.BaseStream.Length - len - OBJECT_ID_LEN - DATA_HEADER_LEN) /m_MaxPacketSize);
                m_Duration = new TimeSpan(0,0, iCnt / PacketsPerSec );
                m_SendDuration = m_Duration;
            }
            reader.Close();
        }
    }

    class AsfChunk
    {
        private ushort m_Cmd, m_Length;

        public ushort Length
        {
            get { return m_Length; }
            set { m_Length = value; }
        }

        public ushort Cmd
        {
            get { return m_Cmd; }
            set { m_Cmd = value; }
        }
        private uint m_SeqNumber;

        public uint SeqNumber
        {
            get { return m_SeqNumber; }
            set { m_SeqNumber = value; }
        }
        private ushort m_Unknown, m_ConfirmLength;

        public ushort ConfirmLength
        {
            get { return m_ConfirmLength; }
            set { m_ConfirmLength = value; }
        }

        public ushort Unknown
        {
            get { return m_Unknown; }
            set { m_Unknown = value; }
        }

        private byte[] m_Data;

        public byte[] Data
        {
            get { return m_Data; }
            set { m_Data = value; }
        }
    }
}
