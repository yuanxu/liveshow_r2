using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Ankh.Pusher.Core
{
    /// <summary>
    /// 消息反馈事件委托
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="Message"></param>
    public delegate void MessageHandler(object Sender,string Message);

    /// <summary>
    /// 频道类型
    /// </summary>
    public enum ChannelType
    {
        /// <summary>
        /// 直播流
        /// </summary>
        Live,
        /// <summary>
        /// 文件轮播
        /// </summary>
        File
    }

    public enum Status
    {
        /// <summary>
        /// 停止状态
        /// </summary>
        Stoped,
        /// <summary>
        /// 运行状态
        /// </summary>
        Running,
        /// <summary>
        /// 发生致命错误
        /// </summary>
        Error
    }

    /// <summary>
    ///　频道，主控类
    /// </summary>
    public class Channel
    {
        
        #region 常量定义

        /// <summary>
        /// 频道名长度
        /// </summary>
        const int NAME_LEN = 40;
        /// <summary>
        /// 发布者长度
        /// </summary>
        const int PUBLISHER_LEN = 40;
        /// <summary>
        /// 频道描述长度
        /// </summary>
        const int INFO_LEN = 100;

        #endregion

        #region 属性

        
        private uint m_GameBps;
     
        private Status m_Status;

        public Status Status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }

        /// <summary>
        /// 频道ID
        /// </summary>
        public ushort ChannelID
        {
            get { return m_Profile.ChannelID; }
            set { m_Profile.ChannelID = value; }
        }

        /// <summary>
        /// 节目类型，是否隐藏。
        /// </summary>
        private ushort m_GameType = 0;

        /// <summary>
        /// 节目类型，是否隐藏。
        /// </summary>
        public ushort GameType
        {
            get { return m_GameType; }
            set { m_GameType = value; }
        }
        
        /// <summary>
        /// 码率
        /// </summary>
        public uint GameBps
        {
            get { return 300;// m_GameBps; 
            }
            set { m_GameBps = value; }
        }
        
        
        #endregion

        #region 事件

        /// <summary>
        /// 有新消息时产生的事件。
        /// </summary>
        public event MessageHandler ShowMessage;
        public event NewFileEventHandler NewFile;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="Message"></param>
        private void OnShowMessage(string Message)
        {
            if (ShowMessage != null)
                ShowMessage(this,Message);
        }

        protected virtual void OnNewFile(NewFileArgs e)
        {
            NewFile(this, e);
        }

        #endregion

        MediaSource m_Source;
        Profile m_Profile = new Profile();

        public Profile Profile
        {
            get { return m_Profile; }
            set { m_Profile = value; }
        }
        TcpClient m_Skt;
        /// <summary>
        /// 启动推送
        /// </summary>
        public void Start()
        {
            if (Status == Status.Running)
            {
                return;
            }
            if (m_Profile.ChannelID == 0)
            {
                Status = Status.Error;
               // throw new NoConfigFileException();
                return;
            }
            
           // m_Profile.LoadFromFile(); //从文件初始化

          //  OnShowMessage("Loading Profile"); //TODO:

            if (m_Profile.ChannelType == ChannelType.File)
            {
                m_Source = new AsfFileSource();
            }
            else
            {
                m_Source = new AsfLiveSource();
            }
            m_PushedHeader = false;
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(WorkThread));
            thread.Start();
            Status = Status.Running;
        }
        private void WorkThread()
        {
            m_Source.Profile = m_Profile;
            m_Source.GetHeader += new MediaEventHandler(m_Source_GetHeader);
            m_Source.GetOneSecondData += new MediaEventHandler(m_Source_GetOneSecondData);
            m_Source.NewFile += new NewFileEventHandler(m_Source_NewFile);
            
            while (m_Source != null)
            {
                try
                {
                    //初始化TcpClient
                    IPEndPoint ip = new IPEndPoint(IPAddress.Parse(m_Profile.CacheSrvIP), m_Profile.CacheSrvPort);
                    m_Skt = new TcpClient();
                    OnShowMessage("Connecting CacheServer...."); //TODO:
                    m_Skt.Connect(ip);
                    OnShowMessage("Connecting CacheServer Successed."); //TODO:

                    //注册
//                    RegisterToCache();

                }
                catch (System.Net.Sockets.SocketException err)
                {
                    OnShowMessage("SOCKET Error:"+err.Message );
                    Status = Status.Error;
                    closeSocket();
                    System.Threading.Thread.Sleep(500);
                    m_PushedHeader = false;
                    continue;
                }
                catch (Exception err)
                {
                    OnShowMessage("register to cache server has failed:"+err.Message);
                    Status = Status.Error;
                    System.Threading.Thread.Sleep(1000);
                    m_PushedHeader = false;
                    continue;
                }
                try
                {
                    Status = Status.Running ;
                    m_Source.Start();
                }
                catch 
                {
                    Status = Status.Error;
                    m_PushedHeader = false;
                    if (m_Source == null)
                    {
                        closeSocket();
                    }
                }
                System.Threading.Thread.Sleep(3000);
            }
        }

        void m_Source_NewFile(object Sender, NewFileArgs e)
        {
            OnNewFile(e);
        }
        void m_Source_GetOneSecondData(object sender, MediaDataArgs e)
        {
            MediaBody data = (MediaBody)e.MediaData;
            uint len = 20;
            foreach (AsfChunk chunk in data.AsfChunks)
            {
                len += (uint)(chunk.Length);
            }
            try
            {
                PushDataToCache(data);  
                OnShowMessage(string.Format("Successed Send Data:FileID:{2} Sequece:{0} Chunks:{1} size:{3}B ", data.SequenceID, data.AsfChunks.Count, data.FileID,len ));
            }
            catch
            {
                
                OnShowMessage(string.Format("Failed Send Data:FileID:{2} Sequece:{0} Chunks:{1} size{3}B", data.SequenceID, data.AsfChunks.Count, data.FileID,len));
                Stop(true);
             
            }
        }

        private void closeSocket()
        {
            try
            {
                m_Skt.Client.Close(2000);
                //m_Skt.GetStream().Close(5000);
            }
            catch (Exception)
            {

            }
            try
            {
                m_Skt.Close();
            }
            catch { }
        }
        bool m_PushedHeader = false;
        void m_Source_GetHeader(object Sender, MediaDataArgs e)
        {
            
            MediaHeader header = (MediaHeader)e.MediaData;
            try
            {
                if (!m_PushedHeader)
                {
                    PushDataToCache(header);
                    m_PushedHeader = true;
                }
                OnShowMessage(string.Format("Successed Send Header: FileID:{0} PacketsPerSec:{4} Min Packet Size:{1} Max Packet Size:{2} Bitrate:{3}", header.FileID, header.AsfHeader.MinPacketSize, header.AsfHeader.MaxPacketSize, header.AsfHeader.MaxBitrate, header.AsfHeader.PacketsPerSec));
            
            }
            catch 
            {
                Stop(true);
                OnShowMessage(string.Format("Failed Send Header: FileID:{0} PacketsPerSec:{4} Min Packet Size:{1} Max Packet Size:{2} Bitrate:{3}", header.FileID, header.AsfHeader.MinPacketSize, header.AsfHeader.MaxPacketSize, header.AsfHeader.MaxBitrate, header.AsfHeader.PacketsPerSec));
                

            }
        }

        /// <summary>
        /// 停止推送
        /// </summary>
        public void Stop()
        {
            Stop(false);
        }
        private void Stop(bool isRestart)
        {
            m_PushedHeader = false;
            Status = Status.Stoped;
            if (m_Source == null)
                return;
            m_Source.Stop();
            m_Source.NewFile -= new NewFileEventHandler(m_Source_NewFile);
            m_Source.GetHeader -= new MediaEventHandler(m_Source_GetHeader);
            m_Source.GetOneSecondData -= new MediaEventHandler(m_Source_GetOneSecondData);
            m_Source = null;
            
            closeSocket();
            m_Skt = null;
            if (isRestart)
            {
                System.Threading.Thread.Sleep(3000);
                Start();
            }
            else
            {
                
            }
        }
        #region CacheServer Message

        byte[] Reverse(byte[] src)
        {
            Array.Reverse(src);
            return src;
        }

        
        private void PushDataToCache(MediaData data)
        {
            System.IO.BinaryWriter writer = new System.IO.BinaryWriter(m_Skt.GetStream());
            
            // whole message length - 24
            //计算长度
            uint len =20;
            MediaHeader mHeader=null;
            MediaBody mBody = null;
            switch (data.DataType)
            {
                case DataType.Header:
                    mHeader = (MediaHeader)data;
                    len += (uint)(  mHeader.AsfHeader.Data.Length);
                    break;
                case DataType.Data:
                    mBody = (MediaBody)data;
                    if (m_Profile.ChannelType == ChannelType.File)
                        len += (uint)(mBody.AsfChunks.Count * (mBody.AsfHeader.MaxPacketSize));
                    else
                    {                        
                        foreach (AsfChunk chunk in mBody.AsfChunks)
                        {
                            len += (uint)(chunk.Length);
                        }
                    }
                    break;
                case DataType.End:
                    mBody = (MediaBody)data;
                    if (m_Profile.ChannelType == ChannelType.File)
                        len += (uint)( mBody.AsfChunks.Count * (mBody.AsfHeader.MaxPacketSize ));
                    else
                    {
                        foreach (AsfChunk chunk in mBody.AsfChunks)
                        {
                            len += (uint)(chunk.Length );
                        }
                    }
                    break;
            }

            //writer.Write((byte)0x01); //Push Data
            //writer.Write(BitConverter.GetBytes(data.SequenceID)); //sequenceid
            //writer.Write(BitConverter.GetBytes(len)); //len

            //writer.Write(BitConverter.GetBytes((uint)ChannelID));           
           
            switch (data.DataType)
            {
                case DataType.Header:
                    writer.Write(0x00010001);//push datas
                    break;
                case DataType.Data:
                    writer.Write(0x00010002);//push datas
                    break;
                case DataType.End:
                    writer.Write(0x00010003);//push datas
                    break;
            }

            writer.Write((uint)data.SequenceID);
            writer.Write((uint)ChannelID);
            writer.Write((uint)0);//peerid
            writer.Write((uint)len);

            
            switch (data.DataType)
            {
                case DataType.Header:
                    // writer.Write((ushort)0x4824);
                    //writer.Write((ushort)(mHeader.AsfHeader.Data.Length + 8));
                    //writer.Write((uint)0);
                    //writer.Write((ushort)0x0c00); // it's important
                    //writer.Write((ushort)(mHeader.AsfHeader.Data.Length + 8));
                    writer.Write(mHeader.AsfHeader.Data);
                    break;
                case DataType.Data:
                case DataType.End:
                    
                    System.IO.BinaryWriter bufWriter = new System.IO.BinaryWriter(new System.IO.MemoryStream());
                    foreach (AsfChunk chunk in mBody.AsfChunks)
                    {
                        //bufWriter.Write((ushort)0x4424);
                        //bufWriter.Write((ushort)(chunk.Length + 8));
                        //bufWriter.Write((uint)chunk.SeqNumber);
                        //bufWriter.Write((ushort)chunk.Unknown);
                        //bufWriter.Write((ushort)(chunk.Length + 8));
                        bufWriter.Write(chunk.Data);
                    }
                    byte[] buf_t = new byte[bufWriter.BaseStream.Length];
                    bufWriter.BaseStream.Position = 0;
                    bufWriter.BaseStream.Read(buf_t, 0, buf_t.Length);
                    bufWriter.Close();
                    writer.Write(buf_t);
                    break;
            }
            
        }
        #endregion
    }

    
}
