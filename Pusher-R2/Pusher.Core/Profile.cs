using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Ankh.Pusher.Core
{
    /// <summary>
    /// Ƶ�������ļ�
    /// �ļ�ʾ��:
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-16"?>
    /// <Channel >
    ///   <!--Ƶ����Ϣ-->
    ///   <ChannelInfo
    ///     channelD="10001"
    ///     type="live"
    ///     name="Ƶ������"
    ///     info="Ƶ������"
    ///     publisher="������"
    ///   />
    ///  <!--Cache Server��Ϣ-->
    ///   <CacheSrv
    ///     ip="192.168.21.135"
    ///     port="7799"
    ///   />
    ///   <!--LiveԴ��Ϣ-->
    ///   <LiveSrv
    ///    ip="192.168.21.40"
    ///     port="8089"
    ///   />
    ///   <!--�ļ�Դ��Ϣ-->
    ///   <Files>
    ///     <File id="" fileName="" duration="" />
    ///   </Files>
    /// </Channel>
    /// ]]>
    /// </summary>
    public class Profile
    {
        #region ����

        private ChannelType m_ChannelType;
        private ushort m_ChannelID =0;
        private string m_CacheSrvIP ="";
        private string m_Name = "", m_Info = "", m_Publisher = "";
        private string m_Custom = "";
        private string m_WMEProfileName;

        public string WMEProfileName
        {
            get { return m_WMEProfileName; }
            set { m_WMEProfileName = value; }
        }

        public string Custom
        {
            get { return m_Custom; }
            set { m_Custom = value; }
        }

        public string CacheSrvIP
        {
            get { return m_CacheSrvIP; }
            set { m_CacheSrvIP = value; }
        }
        private ushort m_CacheSrvPort =7799;

        public ushort CacheSrvPort
        {
            get { return m_CacheSrvPort; }
            set { m_CacheSrvPort = value; }
        }
        private string m_LiveSrvIP = "";

        public string LiveSrvIP
        {
            get { return m_LiveSrvIP; }
            set { m_LiveSrvIP = value; }
        }
        private ushort m_LiveSrvPort =0;

        public ushort LiveSrvPort
        {
            get { return m_LiveSrvPort; }
            set { m_LiveSrvPort = value; }
        }
        private IList<MediaFile> m_Files;

        public IList<MediaFile> Files
        {
            get {
                if (m_Files == null)
                    m_Files = new List<MediaFile>();
                return m_Files; 
            }
            set { m_Files = value; }
        }

        /// <summary>
        /// ������
        /// </summary>
        public string Publisher
        {
            get { return m_Publisher; }
            set { m_Publisher = value; }
        }

        /// <summary>
        /// Ƶ������
        /// </summary>
        public string Info
        {
            get { return m_Info; }
            set { m_Info = value; }
        }

        /// <summary>
        /// Ƶ������
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// Ƶ��ID
        /// </summary>
        public ushort ChannelID
        {
            get { return m_ChannelID; }
            set 
            {
                m_ChannelID = value;
                try
                {
                    if(value != 0)
                        LoadFromFile();
                }
                catch { }
            }
        }

        /// <summary>
        /// Ƶ������
        /// </summary>
        public ChannelType ChannelType
        {
            get { return m_ChannelType; }
            set { m_ChannelType = value; }
        }

        #endregion

        /// <summary>
        /// ���ļ��ж�ȡ
        /// </summary>
        public void LoadFromFile()
        {
            XmlDocument m_Doc = new XmlDocument();

            string fileName = AppDomain.CurrentDomain.BaseDirectory  + ChannelID.ToString() + ".xml";

            if (!System.IO.File.Exists(fileName))
                throw new NoConfigFileException();
            m_Doc.Load(fileName);

            XmlNode node = m_Doc.SelectSingleNode("/Channel/ChannelInfo");
            if (node == null)
                throw new IncorrectConfigFileException("ChannelInfo.type");

            m_Name = node.Attributes["name"].Value;
            m_Info = node.Attributes["info"].Value;
            m_Publisher = node.Attributes["publisher"].Value;
            try
            {
                m_WMEProfileName = node.Attributes["WMEProfileName"].Value;
            }
            catch { }
            string sTmp = node.Attributes["type"].Value.ToLower();
            if (sTmp == "live")
            {
                m_ChannelType = ChannelType.Live;
            }
            else
            {
                m_ChannelType = ChannelType.File;
            }

            node = m_Doc.SelectSingleNode("/Channel/LiveSrv");
            if (node == null)
                throw new  IncorrectConfigFileException("LiveSrv");
            m_LiveSrvIP = node.Attributes["ip"].Value;
            m_LiveSrvPort = ushort.Parse(node.Attributes["port"].Value );
         
       
            XmlNodeList nlist = m_Doc.GetElementsByTagName("File");
            if (m_Files == null)
                m_Files = new List<MediaFile>();
            else
                m_Files.Clear();
            foreach(XmlNode item in nlist)
            {
                MediaFile mfile = new MediaFile();
                mfile.ID = item.Attributes["id"].Value;
                mfile.Duration = TimeSpan.Parse(item.Attributes["duration"].Value);
                mfile.FileName = item.Attributes["fileName"].Value;

                m_Files.Add(mfile);
            }
                
           
            node = m_Doc.SelectSingleNode("/Channel/CacheSrv");
            if (node == null)
                throw new  IncorrectConfigFileException("CacheSrv");
            m_CacheSrvIP = node.Attributes["ip"].Value;
            m_CacheSrvPort = ushort.Parse(node.Attributes["port"].Value );

            node = m_Doc.SelectSingleNode("/Channel/Custom");
            if (node != null)
                m_Custom = ((XmlCDataSection)node.FirstChild).Data ;
            
        }

        /// <summary>
        /// д���ļ�
        /// </summary>
        public void WriteToFile()
        {
            if (m_ChannelID == 0)
                return;
            XmlWriter writer = XmlWriter.Create(AppDomain.CurrentDomain.BaseDirectory  +m_ChannelID.ToString() + ".xml");
            writer.WriteStartDocument();

            writer.WriteStartElement("Channel"); //root element

            writer.WriteComment("Ƶ����Ϣ��type����ȡֵfile��live");
            writer.WriteStartElement("ChannelInfo");
            writer.WriteAttributeString("channelID", m_ChannelID.ToString());
            writer.WriteAttributeString("type", m_ChannelType.ToString().ToLower());
            writer.WriteAttributeString("name", m_Name);
            writer.WriteAttributeString("info", m_Info);
            writer.WriteAttributeString("publisher", m_Publisher);
            writer.WriteAttributeString("WMEProfileName", m_WMEProfileName);
            writer.WriteEndElement();

            writer.WriteComment("Cache Server Info");
            writer.WriteStartElement("CacheSrv");
            writer.WriteAttributeString("ip", m_CacheSrvIP);
            writer.WriteAttributeString("port", m_CacheSrvPort.ToString());
            writer.WriteEndElement();

            writer.WriteComment("LiveԴ");
            writer.WriteStartElement("LiveSrv");
            writer.WriteAttributeString("ip", m_LiveSrvIP);
            writer.WriteAttributeString("port", m_LiveSrvPort.ToString());
            writer.WriteEndElement();

            writer.WriteComment("�ļ�Դ��Ϣ");
            writer.WriteStartElement("Files");
            foreach (MediaFile mfile in Files)
            {
                writer.WriteStartElement("File");
                writer.WriteAttributeString("id", mfile.ID);
                writer.WriteAttributeString("fileName", mfile.FileName);
                writer.WriteAttributeString("duration",mfile.Duration.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();//files

            //custome
            writer.WriteStartElement("Custom");
            writer.WriteCData(m_Custom);
            writer.WriteEndElement();

            writer.WriteEndElement();//channel

            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();
        }
    }

    public struct MediaFile
    {
        private string m_ID, m_FileName;
        private TimeSpan  m_Duration;

        public TimeSpan  Duration
        {
            get { return m_Duration; }
            set { m_Duration = value; }
        }

        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public static string ComputeID(string fileName)
        {
            return fileName.GetHashCode().ToString();
        }
    }
}
