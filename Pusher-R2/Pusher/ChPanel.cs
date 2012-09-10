using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.Pusher.Core;

namespace Ankh.Pusher
{
    public partial class ChPanel : UserControl
    {
        private Channel m_Channel;
        public event ChannelProfileChangedEventHandler ProfileChanged;


        public ChPanel()
        {
            ListBox.CheckForIllegalCrossThreadCalls = false;
           
            InitializeComponent();
        }

        public ushort ChannelID
        {
            get
            {
                if (m_Channel == null)
                    return 0;
                return m_Channel.ChannelID; 
            }
            set
            {
                if (m_Channel == null)
                {
                    m_Channel = new Channel();
                    m_Channel.ChannelID = value;
                    try
                    {
                        m_Channel.Profile.LoadFromFile();
                        InitUI();
                    }
                    catch (NoConfigFileException)
                    {
                    }
                    m_Channel.ShowMessage += new MessageHandler(m_Channel_ShowMessage);
                    m_Channel.NewFile += new NewFileEventHandler(m_Channel_NewFile);
                }
            }
        }

        void m_Channel_NewFile(object Sender, NewFileArgs e)
        {
            bool bFound = false;
            foreach(ListViewItem item in lvFiles.Items)
            {
                if (item.Tag.ToString() == e.File.ID && !bFound)
                {
                    item.SubItems[3].Text = "Playing";
                    bFound = true;
                }
                else
                {
                    item.SubItems[3].Text = "";
                }
            }
        }

        public Channel Channel
        {
            get { return m_Channel; }
        }

        #region init ui

        private void InitUI()
        {
            InitProperty();
            InitSource();
            RefreshStatus();
        }

        private void InitProperty()
        {
            tChID.Text = m_Channel.ChannelID.ToString();
            tChName.Text = lblName.Text = m_Channel.Profile.Name;
            tPubName.Text = m_Channel.Profile.Publisher;
            tInfo.Text = m_Channel.Profile.Info;
            cbType.SelectedIndex = (int)m_Channel.Profile.ChannelType;

            tCacheSrvIP.Text = m_Channel.Profile.CacheSrvIP;
            tCacheSrvPort.Text  = m_Channel.Profile.CacheSrvPort.ToString();

        }

        private void InitSource()
        {

            tLiveSrvIP.Text = m_Channel.Profile.LiveSrvIP;
            tLiveSrvPort.Text = m_Channel.Profile.LiveSrvPort.ToString();

            for (int i = 0; i < m_Channel.Profile.Files.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = (i + 1).ToString();
                item.SubItems.Add(m_Channel.Profile.Files[i].FileName);
                item.SubItems.Add(m_Channel.Profile.Files[i].Duration.ToString());
                item.SubItems.Add("");
                item.Tag = m_Channel.Profile.Files[i].ID;
                lvFiles.Items.Add(item);
            }

        }

    #endregion

        private void RefreshStatus()
        {
            if (m_Channel == null)
                return;
            lblStatus.Text = m_Channel.Status.ToString();
            lblName.Text = m_Channel.Profile.Name;
            lblType.Text = m_Channel.Profile.ChannelType.ToString();
            if (m_Channel.Status == Status.Running || m_Channel.Status == Status.Error)
            {
                btStart.Enabled = false;
                btStop.Enabled = true;
            }
            else
            {
                btStart.Enabled = true;
                btStop.Enabled = false;
            }
        }

        #region Apply

        private void ApplyProperty()
        {
            try
            {
                m_Channel.ChannelID = ushort.Parse(tChID.Text);
                m_Channel.Profile.Name = tChName.Text;
                m_Channel.Profile.ChannelType = (ChannelType)cbType.SelectedIndex;
                m_Channel.Profile.Info = tInfo.Text;
                m_Channel.Profile.Publisher = tPubName.Text;
                m_Channel.Profile.LiveSrvIP = tLiveSrvIP.Text;
                m_Channel.Profile.LiveSrvPort = ushort.Parse(tLiveSrvPort.Text);

                m_Channel.Profile.CacheSrvIP = tCacheSrvIP.Text;
                m_Channel.Profile.CacheSrvPort = ushort.Parse(tCacheSrvPort.Text);
                m_Channel.Profile.WriteToFile();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplySource()
        {
            try
            {
                m_Channel.Profile.LiveSrvIP = tLiveSrvIP.Text;
                m_Channel.Profile.LiveSrvPort = ushort.Parse(tLiveSrvPort.Text);
                m_Channel.Profile.Files.Clear();
                foreach (ListViewItem item in lvFiles.Items)
                {
                    MediaFile mFile = new MediaFile();
                    mFile.ID = item.Tag.ToString();
                    mFile.FileName = item.SubItems[1].Text;
                    mFile.Duration = TimeSpan.Parse(item.SubItems[2].Text);
                    m_Channel.Profile.Files.Add(mFile );
                }
                m_Channel.Profile.WriteToFile();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void btProCancel_Click(object sender, EventArgs e)
        {
            InitProperty();
        }

        private void btMdCancel_Click(object sender, EventArgs e)
        {
            InitSource();
        }

        private void btMdApply_Click(object sender, EventArgs e)
        {
            ApplySource();
        }

        private void btProAppy_Click(object sender, EventArgs e)
        {
            ApplyProperty();
            RefreshStatus();
            OnProfileChanged();
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            if (m_Channel == null)
            {
                return;
            }
            
            m_Channel.Start();
            RefreshStatus();
            btStart.Enabled = false;
            btStop.Enabled = true;
        }

        void m_Channel_ShowMessage(object Sender, string Message)
        {
            if (lbLog.Items.Count  > 20)
                lbLog.Items.RemoveAt(0);
            lbLog.Items.Add(Message);
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            m_Channel.Stop();
            RefreshStatus();
            btStop.Enabled = false;
            btStart.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                RefreshStatus();
            }
            catch { }
        }

        private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

      
        private void OnProfileChanged()
        {
            if (ProfileChanged != null)
                ProfileChanged(this,new ChannelArgs( m_Channel));
        }

        private void cmFileSource_Opening(object sender, CancelEventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
            {
                toolStripMenuItem2.Enabled = toolStripMenuItem3.Enabled = toolStripMenuItem4.Enabled = toolStripMenuItem5.Enabled = false;
            }
            else
            {
                toolStripMenuItem2.Enabled = toolStripMenuItem3.Enabled = toolStripMenuItem4.Enabled = toolStripMenuItem5.Enabled = true;
            }
        }

        #region File Source Context menu

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            doAddFile();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            doRemoveFile();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            doUp();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            doDown();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            doRunThis();
        }


        private void btAddFile_Click(object sender, EventArgs e)
        {
            doAddFile();
        }

        private void btRemoveFile_Click(object sender, EventArgs e)
        {
            doRemoveFile();
        }

        private void btUp_Click(object sender, EventArgs e)
        {
            doUp();
        }

        private void btDown_Click(object sender, EventArgs e)
        {
            doDown();
        }

        private void ReindexFiles()
        {
            lvFiles.BeginUpdate();
            m_Channel.Profile.Files.Clear();
            foreach (ListViewItem item in lvFiles.Items)
            {
                MediaFile mFile = new MediaFile();
                mFile.FileName = item.SubItems[1].Text;
                mFile.ID = item.Tag.ToString();
                mFile.Duration = TimeSpan.Parse(item.SubItems[2].Text);
                m_Channel.Profile.Files.Add(mFile);

                item.Text = (item.Index + 1).ToString();
            }
            lvFiles.EndUpdate();
        }

        private void doAddFile()
        {
            if (dlgOpen.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string f in dlgOpen.FileNames)
                {
                    try
                    {
                        MediaFile mFile = new MediaFile();
                        mFile.FileName = f;
                        mFile.ID = MediaFile.ComputeID(f);
                        AsfHeader header = new AsfHeader();
                        header.InitFromFile(f);
                        mFile.Duration = header.SendDuration;
                        if (header.MaxBitrate > 660 * 1024) //650Kbps
                        {
                            MessageBox.Show(string.Format("{0}'s Bitrate is too large!", f), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            continue;
                        }
                        lvFiles.BeginUpdate();
                        ListViewItem item = new ListViewItem();
                        item.Text = (lvFiles.Items.Count + 1).ToString();
                        item.SubItems.Add(mFile.FileName);
                        item.SubItems.Add(mFile.Duration.ToString());
                        item.SubItems.Add("");
                        item.Tag = mFile.ID;
                        lvFiles.Items.Add(item);

                        // m_Channel.Profile.Files.Add(mFile);
                        ReindexFiles();

                        lvFiles.EndUpdate();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message,Application.ProductName,MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
                
            }
        }
        
        private void doRemoveFile()
        {
            if (lvFiles.SelectedItems.Count == 0)
                return;
            ListViewItem item = lvFiles.SelectedItems[0];
            if (item.SubItems[3].Text != "")
            {
                MessageBox.Show("Can't remove file while pushing!");
                return;
            }
            
            item.Remove();
            ReindexFiles();
        }

        private void doUp()
        {
            if (lvFiles.SelectedItems.Count == 0 || lvFiles.SelectedIndices[0] == 0)
                return;
            int index = lvFiles.SelectedItems[0].Index;
            ListViewItem item = lvFiles.SelectedItems[0];
            item.Remove();
            lvFiles.Items.Insert(index - 1, item);
            ReindexFiles();
        }

        private void doDown()
        {
            if (lvFiles.SelectedItems.Count == 0 || lvFiles.SelectedIndices[0] == lvFiles.Items.Count - 1)
                return;
            int index = lvFiles.SelectedItems[0].Index;
            ListViewItem item = lvFiles.SelectedItems[0];
            item.Remove();
            lvFiles.Items.Insert(index + 1, item);
            ReindexFiles();
        }

        private void doRunThis()
        {
            if (Channel == null || lvFiles.SelectedItems.Count == 0 || (Channel.Profile.Custom == lvFiles.SelectedItems[0].Tag.ToString() && Channel.Status == Status.Running))
                return;
            
            Channel.Profile.Custom = lvFiles.SelectedItems[0].Tag.ToString();
            Channel.Stop();
            System.Threading.Thread.Sleep(1000);
            Channel.Start();
        }
        #endregion

        private void tLiveSrvIP_TextChanged(object sender, EventArgs e)
        {
            ApplyProperty();
        }

        private void tLiveSrvPort_TextChanged(object sender, EventArgs e)
        {
            ApplyProperty();
        }

        private void tChID_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }

    public class ChannelArgs : EventArgs
    {
        private Channel m_Channel;

        public Channel Channel
        {
            get { return m_Channel; }
            set { m_Channel = value; }
        }
        public ChannelArgs(Channel ch)
        {
            m_Channel = ch;
        }
    }

    public delegate void ChannelProfileChangedEventHandler(object Sender,ChannelArgs e);
}
