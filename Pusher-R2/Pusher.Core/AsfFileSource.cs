using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ankh.Pusher.Core
{
    class AsfFileSource : MediaSource
    {
        private bool bIsRunning;
        const int OBJECT_ID_LEN = 16;
        const int OBJECT_SIZE_LEN = 64;
        const int DATA_HEADER_LEN = 0x32;

        private long  m_filePostion = 0; //文件指针
        private ushort iFileID = 1;
        
        private string m_fileId = "";//出现错误时，现在播放的文件

        public override void Start()
        {
            bIsRunning = true;
            bool bRecover = false;
            m_fileId = Profile.Custom;

            if(m_filePostion >0 || m_fileId.Length >0)
                bRecover =true ;
            

            while (bIsRunning)
            {
                string file = Profile.Files[0].FileName; //正在播放的文件名
                int iLoc = 0;
                if (bRecover && m_fileId.Length >0)
                {
                    for (int i = 0; i < Profile.Files.Count; i++)
                    {
                        if (Profile.Files[i].ID == m_fileId)
                        {
                            iLoc = i;
                            file = Profile.Files[i].FileName;
                            break;
                        }
                    }
                }
                while (bIsRunning)
                {
                    //Play
                    
                    m_fileId = Profile.Files[iLoc].ID; //保存正在播放之文件名
                    Profile.Custom = m_fileId;
                    Profile.WriteToFile();

                    OnNewFile(new NewFileArgs(Profile.Files[iLoc]));

                    ulong iChunkSeq = 0;

                    FileStream fileStream = null;
                    try
                    {
                        fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    }
                    catch
                    {
                        goto lbNext;
                    }
                    BinaryReader reader = new BinaryReader(fileStream);

                    //处理头
                    reader.ReadBytes(OBJECT_ID_LEN); //过虑掉ASF Header Object ID
                    int len = (int)reader.ReadUInt64();//asf header size

                    MediaHeader header = new MediaHeader();
                    header.FileID = iFileID;
                    reader.BaseStream.Position = 0;
                    header.AsfHeader.Data = reader.ReadBytes(len + DATA_HEADER_LEN); //读取头数据
                    if (header.AsfHeader.MaxBitrate > 650 * 1024)//大于最大支持的比特率
                    {
                        goto lbNext;
                    }
                    header.SequenceID = GetCurrentSequenceID();

                    OnGetHeader(new MediaDataArgs(header)); //触发得到头事件

                    //if (bRecover && m_filePostion != 0)
                    //   reader.BaseStream.Position = m_filePostion;

                    //读取一秒钟数据，直到文件尾
                    while (bIsRunning)
                    {
                        MediaBody body = new MediaBody();
                        body.AsfHeader = header.AsfHeader;
                        body.FileID = iFileID;
                        body.SequenceID = GetNextSequenceID();
                        int iSize = 0;
                        for (int i = 0; i < header.AsfHeader.PacketsPerSec
                            && reader.BaseStream.Position < reader.BaseStream.Length
                            && (header.AsfHeader.PacketCount > 0 ? ((ulong)iChunkSeq < header.AsfHeader.PacketCount) : true)
                            ; i++)
                        {
                            AsfChunk chunk = new AsfChunk();

                            //读取一个Packet
                            //假定是恒定的packetsize
                            chunk.Length = (ushort)header.AsfHeader.MinPacketSize;// reader.ReadUInt16(); //可能要倒字节 BitConverter.ToUInt16(Array.Reverse(reader.ReadChars(2)));
                            iSize += chunk.Length;
                            chunk.Data = reader.ReadBytes(chunk.Length);

                            chunk.SeqNumber = (uint)iChunkSeq++;
                            m_filePostion = reader.BaseStream.Position;
                            body.AsfChunks.Add(chunk);
                        }

                        if (reader.BaseStream.Position == reader.BaseStream.Length || (ulong)iChunkSeq == header.AsfHeader.PacketCount) //已经读完一个文件
                        {
                            body.DataType = DataType.End;
                            OnGetOneSecondData(new MediaDataArgs(body));
                            break;
                        }
                        else
                        {
                            OnGetOneSecondData(new MediaDataArgs(body));
                        }
                        System.Threading.Thread.Sleep(990);//995ms

                    }
                lbNext:
                    if(fileStream !=null)
                        fileStream.Close();
                
                    if (iFileID == 0xffff)
                        iFileID = 0;
                    iFileID++;

                    bRecover = false;

                    //goto next file
                    for (int i_t = 0; i_t < Profile.Files.Count; i_t++)
                    {
                        if (m_fileId == Profile.Files[i_t].ID)
                        {
                            if (i_t == Profile.Files.Count - 1) //最后一个
                                i_t = 0;
                            else
                                i_t++;
                            iLoc = i_t;
                            file = Profile.Files[i_t].FileName;
                            break;
                        }
                    }//for
                }//play
                
            }//while
            
        }

        public override void Stop()
        {
            bIsRunning = false;
        }
    }
}
