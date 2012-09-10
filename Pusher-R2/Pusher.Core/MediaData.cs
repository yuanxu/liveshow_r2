using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Pusher.Core
{
    /// <remarks>媒体数据</remarks>
    class MediaData
    {
        private DataType m_DataType = DataType.Data;
        private uint m_SequenceID;
        private ushort m_FileID;

        public ushort FileID
        {
            get { return m_FileID; }
            set { m_FileID = value; }
        }

        /// <value>DataType.Data</value>
        public DataType DataType
        {
            get
            {
                return m_DataType;
            }
            set
            {
                m_DataType = value;
            }
        }

        public uint SequenceID
        {
            get
            {
                return m_SequenceID;
            }
            set
            {
                m_SequenceID = value;
            }
        }
    }

    enum DataType
    {
        /// <summary>
        /// ASF 头
        /// </summary>
        Header = 1,
        /// <summary>
        /// ASF 数据
        /// </summary>
        Data = 2,
        /// <summary>
        /// ASF 尾
        /// </summary>
        End = 3,
    }

    /// <summary>
    /// 媒体头
    /// </summary>
    class MediaHeader:MediaData
    {

        public MediaHeader()
            : base()
        {
            DataType = DataType.Header;
            m_AsfHeader = new AsfHeader();
        }

        private AsfHeader m_AsfHeader;
        
        public  AsfHeader AsfHeader
        {
            get
            {
                if (m_AsfHeader == null)
                    m_AsfHeader = new AsfHeader();
                return m_AsfHeader; 
            }
            set { m_AsfHeader = value; }
        }
    }

    /// <summary>
    /// 保存一秒钟的数据
    /// </summary>
    class MediaBody : MediaData
    {
        public MediaBody()
            : base()
        {
            m_AsfChunks = new List<AsfChunk>();
            DataType = DataType.Data;
        }

        private IList<AsfChunk> m_AsfChunks;
        private AsfHeader m_AsfHeader;

        public AsfHeader AsfHeader
        {
            get { return m_AsfHeader; }
            set { m_AsfHeader = value; }
        }
        public IList<AsfChunk> AsfChunks
        {
            get
            {
                return m_AsfChunks;
            }
        }
    }
}
