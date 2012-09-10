using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Ankh.Pusher.Core
{
    /// <summary>
    /// 频道配置文件
    /// 文件示例:
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-16"?>
    /// <Channel >
    ///   <!--频道信息-->
    ///   <ChannelInfo
    ///     channelD="10001"
    ///     type="live"
    ///     name="频道名称"
    ///     info="频道描述"
    ///     publisher="发布者"
    ///   />
    ///  <!--Cache Server信息-->
    ///   <CacheSrv
    ///     ip="192.168.21.135"
    ///     port="7799"
    ///   />
    ///   <!--Live源信息-->
    ///   <LiveSrv
    ///    ip="192.168.21.40"
    ///     port="8089"
    ///   />
    ///   <!--文件源信息-->
    ///   <Files>
    ///     <File id="" fileName="" duration="" />
    ///   </Files>
    /// </Channel>
    /// ]]>
    /// </summary>
    public class Profile
    {
        #region 属性

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
        /// 发布者
        /// </summary>
        public string Publisher
        {
            get { return m_Publisher; }
            set { m_Publisher = value; }
        }

        /// <summary>
        /// 频道描述
        /// </summary>
        public string Info
        {
            get { return m_Info; }
            set { m_Info = value; }
        }

        /// <summary>
        /// 频道名称
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// 频道ID
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
        /// 频道类型
        /// </summary>
        public ChannelType ChannelType
        {
            get { return m_ChannelType; }
            set { m_ChannelType = value; }
        }

        #endregion

        /// <summary>
        /// 从文件中读取
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
        /// 写入文件
        /// </summary>
        public void WriteToFile()
        {
            if (m_ChannelID == 0)
                return;
            XmlWriter writer = XmlWriter.Create(AppDomain.CurrentDomain.BaseDirectory  +m_ChannelID.ToString() + ".xml");
            writer.WriteStartDocument();

            writer.WriteStartElement("Channel"); //root element

            writer.WriteComment("频道信息。type可以取值file或live");
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

            writer.WriteComment("Live源");
            writer.WriteStartElement("LiveSrv");
            writer.WriteAttributeString("ip", m_LiveSrvIP);
            writer.WriteAttributeString("port", m_LiveSrvPort.ToString());
            writer.WriteEndElement();

            writer.WriteComment("文件源信息");
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
