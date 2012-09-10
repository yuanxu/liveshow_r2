using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Pusher.Core
{
    /// <summary>
    /// 媒体事件委托
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    delegate void MediaEventHandler(Object Sender,MediaDataArgs e);
    public delegate void NewFileEventHandler(object Sender,NewFileArgs e);
    /// <summary>
    /// 媒体事件参数
    /// </summary>
    class MediaDataArgs : EventArgs
    {
        public MediaDataArgs()
        {
        }
        public MediaDataArgs(MediaData data)
        {
            m_Data = data;
        }
        private MediaData m_Data;
        public  MediaData MediaData
        {
            get
            {
                return m_Data;
            }
            set
            {
                m_Data = value;
            }
        }
    }

    public class NewFileArgs : EventArgs
    {
        private MediaFile m_File;

        public MediaFile File
        {
            get { return m_File; }
            set { m_File = value; }
        }
        public NewFileArgs(MediaFile file)
        {
            m_File = file;
        }
    }

    /// <summary>
    /// 媒体源抽象基类
    /// </summary>
    abstract class MediaSource 
    {
        private uint m_SequenceID = 0;

        private Profile m_Profile;

        public Profile Profile
        {
            get { return m_Profile; }
            set { m_Profile = value; }
        }

        public event MediaEventHandler GetHeader;

        public event MediaEventHandler GetOneSecondData;
        public event NewFileEventHandler NewFile;
    
        public abstract void Start();

        public abstract void Stop();

        protected uint GetNextSequenceID()
        {
            return m_SequenceID++;
        }

        protected uint GetCurrentSequenceID()
        {
            return m_SequenceID;
        }

        protected virtual void OnGetHeader(MediaDataArgs e)
        {
            GetHeader(this, e);
        }

        protected virtual void OnGetOneSecondData(MediaDataArgs e)
        {
            GetOneSecondData(this, e);
        }

        protected virtual void OnNewFile(NewFileArgs e)
        {
            NewFile(this, e);
        }
    }

    
}
