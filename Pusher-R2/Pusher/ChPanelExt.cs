using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.Pusher.Core;
using WMEncoderLib;
using WMPREVIEWLib;

namespace Ankh.Pusher
{

    public partial class ChPanelExt : UserControl
    {
        private Channel m_Channel;
        public event ChannelProfileChangedEventHandler ProfileChanged;
        private Srv.PusherSrv objSrv = new Ankh.Pusher.Srv.PusherSrv();
        // Create a WMEncoder object.
        WMEncoder Encoder ;//= new WMEncoder();
        
        public ChPanelExt()
        {
            ListBox.CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();

            Encoder = new WMEncoder();
            Encoder.OnSourceStateChange += new _IWMEncoderEvents_OnSourceStateChangeEventHandler(Encoder_OnSourceStateChange);
            
            EnumProfiles();
            objSrv.GetOnlineUserCountCompleted += new Ankh.Pusher.Srv.GetOnlineUserCountCompletedEventHandler(objSrv_GetOnlineUserCountCompleted);
        }

        void objSrv_GetOnlineUserCountCompleted(object sender, Ankh.Pusher.Srv.GetOnlineUserCountCompletedEventArgs e)
        {
            try
            {
                lblOnlineUser.Text = e.Result.ToString();
                if (Channel.Status == Status.Running)
                    objSrv.GetOnlineUserCountAsync(tChId.Text);
            }
            catch { }
        }

        void Encoder_OnSourceStateChange(WMENC_SOURCE_STATE enumState, WMENC_SOURCE_TYPE enumType, short iIndex, string bstrSourceGroup)
        {
            if (enumState == WMENC_SOURCE_STATE.WMENC_SOURCE_START)
            {
                bool bFound = false;
                Encoder.Flush();
                foreach (ListViewItem item in lvFiles.Items)
                {
                    if (item.Tag.ToString() == bstrSourceGroup && !bFound)
                    {
                        item.SubItems[3].Text = "Playing";
                        bFound = true;
                        //// Display the postview in a panel named PostviewPanel.
                        m_Channel.Profile.Custom = item.Tag.ToString();
                        m_Channel.Profile.WriteToFile();
                        try
                        {
                            IWMEncVideoSource SrcVid = (IWMEncVideoSource)Encoder.SourceGroupCollection.Active.get_Source(WMENC_SOURCE_TYPE.WMENC_VIDEO, 0);
                            if (chkReview.Checked && SrcVid != null)
                            {
                                object Postview;
                                int lPostviewStream;
                                lPostviewStream = SrcVid.PostviewCollection.Item(0, out Postview);
                                ((WMEncDataView)Postview).SetViewProperties(lPostviewStream, (int)picReview.Handle);
                                ((WMEncDataView)Postview).StartView(lPostviewStream);
                                picReview.Visible = true;
                            }
                            else
                            {
                                picReview.Visible = false;
                            }
                        }
                        catch { picReview.Visible = false; }
                    }
                    else
                    {
                        item.SubItems[3].Text = "";
                    }
                }
            }
        }

        private int m_ProOffset = -1;
        void EnumProfiles()
        {
         
            // Configure the sources.

            // Choose a profile from the collection.
            // Replace the following profile name with the profile that you want.
            
            IWMEncProfileCollection ProColl = Encoder.ProfileCollection;
            
            ProColl.ProfileDirectory = "Profiles";
            ProColl.Refresh();
            IWMEncProfile Pro;
            for (int i = 0; i < ProColl.Count; i++)
            {
                Pro = ProColl.Item(i);
                if (Pro.Name.IndexOf("Windows Media") < 0)
                {
                    if (m_ProOffset == -1)
                        m_ProOffset = i;
                    cbProfiles.Items.Add(Pro.Name);
                }
            }

        }

        
        private void OnProfileChanged()
        {
            if (ProfileChanged != null)
                ProfileChanged(this, new ChannelArgs(m_Channel));
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
        public Channel Channel
        {
            get { return m_Channel; }
        }
        void m_Channel_NewFile(object Sender, NewFileArgs e)
        {
            bool bFound = false;
            foreach (ListViewItem item in lvFiles.Items)
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
        void m_Channel_ShowMessage(object Sender, string Message)
        {
            if (lbLog.Items.Count > 15)
                lbLog.Items.RemoveAt(0);
            lbLog.Items.Add(Message);
            
        }

        #region init ui

        private void InitUI()
        {
            InitProperty();
            InitSource();
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            if (m_Channel == null)
                return;
            lblStatus.Text = m_Channel.Status.ToString();
            lblName.Text = m_Channel.Profile.Name;
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


        private void InitProperty()
        {
            tChId.TextChanged -= tChId_TextChanged;
            tChName.TextChanged -= tChName_TextChanged;
            tChDesc.TextChanged -= tChDesc_TextChanged;
            tSrvIp.TextChanged -= tSrvIp_TextChanged;
            tSrvPort.TextChanged -= tSrvPort_TextChanged;
            cbProfiles.SelectedIndexChanged -= cbProfiles_SelectedIndexChanged;

            tChId.Text = m_Channel.ChannelID.ToString();
            tChName.Text = lblName.Text = m_Channel.Profile.Name;
            
            tChDesc.Text = m_Channel.Profile.Info;
            
            tSrvIp.Text = m_Channel.Profile.CacheSrvIP;
            tSrvPort.Text = m_Channel.Profile.CacheSrvPort.ToString();

            for(int i=0;i<cbProfiles.Items.Count;i++)
            {
                if(cbProfiles.Items[i].ToString() == m_Channel.Profile.WMEProfileName)
                {
                    cbProfiles.SelectedIndex = i;
                    break;
                }
            }
            if (cbProfiles.SelectedIndex == -1)
            {
                //TODO:ÉèÖÃÄ¬ÈÏÖµ
            }
            tChId.TextChanged += tChId_TextChanged;
            tChName.TextChanged += tChName_TextChanged;
            tChDesc.TextChanged += tChDesc_TextChanged;
            tSrvIp.TextChanged += tSrvIp_TextChanged;
            tSrvPort.TextChanged += tSrvPort_TextChanged;
            cbProfiles.SelectedIndexChanged += cbProfiles_SelectedIndexChanged;
        }

        private void InitSource()
        {
                      
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
        
        #region Apply

        private void ApplyProperty()
        {
            try
            {
                m_Channel.ChannelID = ushort.Parse(tChId.Text);
                m_Channel.Profile.Name = tChName.Text;
                //m_Channel.Profile.ChannelType = (ChannelType)cbType.SelectedIndex;
                m_Channel.Profile.Info = tChDesc.Text;
                //m_Channel.Profile.Publisher = tPubName.Text;
                //m_Channel.Profile.LiveSrvIP = tLiveSrvIP.Text;
                //m_Channel.Profile.LiveSrvPort = ushort.Parse(tLiveSrvPort.Text);
                m_Channel.Profile.WMEProfileName = cbProfiles.Text ;

                m_Channel.Profile.CacheSrvIP = tSrvIp.Text;
                m_Channel.Profile.CacheSrvPort = ushort.Parse(tSrvPort.Text);
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
                //m_Channel.Profile.LiveSrvIP = tLiveSrvIP.Text;
                //m_Channel.Profile.LiveSrvPort = ushort.Parse(tLiveSrvPort.Text);
                m_Channel.Profile.Files.Clear();
                foreach (ListViewItem item in lvFiles.Items)
                {
                    MediaFile mFile = new MediaFile();
                    mFile.ID = item.Tag.ToString();
                    mFile.FileName = item.SubItems[1].Text;
                    mFile.Duration = TimeSpan.Parse(item.SubItems[2].Text);
                    m_Channel.Profile.Files.Add(mFile);
                }
                m_Channel.Profile.WriteToFile();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region UI changed

        private void tChId_TextChanged(object sender, EventArgs e)
        {
            ApplyProperty();
            OnProfileChanged();
        }

        private void tChName_TextChanged(object sender, EventArgs e)
        {
            ApplyProperty();
            OnProfileChanged();
        }

        private void tChDesc_TextChanged(object sender, EventArgs e)
        {
            ApplyProperty();
        }

        private void tSrvIp_TextChanged(object sender, EventArgs e)
        {
            ApplyProperty();
        }

        private void tSrvPort_TextChanged(object sender, EventArgs e)
        {
            ApplyProperty();
        }

        private void cbProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            IWMEncProfile Pro = Encoder.ProfileCollection.Item(cbProfiles.SelectedIndex+m_ProOffset);
            lblDescription.Text = Pro.Description;
            ApplyProperty();
        }

        #endregion

        #region Start & Stop

        public void Start()
        {
            btStart_Click(null, null);
        }
        public void Stop()
        {
            btStop_Click(null,null);
        }
        DateTime dtStartTime;
        private void btStart_Click(object sender, EventArgs e)
        {
            if (m_Channel == null)
            {
                return;
            }
            ushort iPort = StartEnc();
            if (iPort == 0)
                return;
            m_Channel.Profile.ChannelType = ChannelType.Live;
            m_Channel.Profile.LiveSrvIP = "127.0.0.1";
            m_Channel.Profile.LiveSrvPort = iPort;
            dtStartTime = DateTime.Now;
            m_Channel.Start();
            RefreshStatus();
            btStart.Enabled = false;
            btStop.Enabled = true;
            cbProfiles.Enabled = false;
            tmrAuth.Enabled = true;
        }


        private void btStop_Click(object sender, EventArgs e)
        {
            StopEnc();
            m_Channel.Stop();
            RefreshStatus();
            btStop.Enabled = false;
            btStart.Enabled = true;
            picReview.Visible = false;
            cbProfiles.Enabled = true;
            tmrAuth.Enabled = false;
        }

        private ushort StartEnc()
        {
            ushort iPort;
            if (cbProfiles.SelectedIndex == -1)
            {
                MessageBox.Show("Not supported Profile!\r\nPlease choice another one.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return 0;
            }
            Random rnd = new Random(DateTime.Now.Millisecond);
            iPort= (ushort)rnd.Next(8000, 9000);
            // Retrieve an IWMEncBroadcast object.
            IWMEncBroadcast BrdCst = Encoder.Broadcast;

            // Set the port number.
            BrdCst.set_PortNumber(WMENC_BROADCAST_PROTOCOL.WMENC_PROTOCOL_HTTP, iPort);

            // clear all predefind list

            // Retrieve the source group collection.
            IWMEncSourceGroupCollection SrcGrpColl = Encoder.SourceGroupCollection;
            while (SrcGrpColl.Count != 0)
            {
                SrcGrpColl.Remove(0);
            }
            //add source
            int iCounter = 0;
            foreach (MediaFile file in m_Channel.Profile.Files)
            {
                // Add a source group to the collection.

                    IWMEncSourceGroup2 SrcGrp;
                    SrcGrp = (IWMEncSourceGroup2)SrcGrpColl.Add(file.ID);//string.Format("SG_{0}",iCounter)
                    SrcGrp.SetAutoRollover(-1, "WMENC_SOURCEGROUP_AUTOROLLOVER_TO_NEXT");
                    try
                    {
                        SrcGrp.AutoSetFileSource(file.FileName);
                    }
                    catch
                    {
                        SrcGrpColl.Remove(SrcGrp);
                        
                        continue;
                    }
                    //  SrcGrp.get_Source(WMENC_SOURCE_TYPE.WMENC_VIDEO , SrcGrp.get_SourceCount(WMENC_SOURCE_TYPE.WMENC_VIDEO)).Repeat=true;
                    SrcGrp.set_Profile(Encoder.ProfileCollection.Item(cbProfiles.SelectedIndex+m_ProOffset));
                    if (m_Channel.Profile.Custom == file.ID)
                        Encoder.SourceGroupCollection.Active = SrcGrp;

                    IWMEncDataViewCollection DVColl_Postview;
                    try
                    {
                        IWMEncVideoSource SrcVid = (IWMEncVideoSource)SrcGrp.get_Source(WMENC_SOURCE_TYPE.WMENC_VIDEO, 0);
                        if (SrcVid != null)
                        {
                            DVColl_Postview = SrcVid.PostviewCollection;

                            // Create a WMEncDataView object.
                            WMEncDataView Postview;
                            Postview = new WMEncDataView();

                            // Add the WMEncDataView object to the collection.
                            int lPostviewStream;
                            lPostviewStream = DVColl_Postview.Add(Postview);
                        }
                    }
                    catch
                    {
                    }
                    iCounter++;
               
            }

            Encoder.AutoStop = false;
            try
            {
                Encoder.PrepareToEncode(true);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,Application.ProductName,MessageBoxButtons.OK,MessageBoxIcon.Error);
                return 0;
            }
            tmrStatus.Enabled = true;
            Encoder.Start();
            
            if (lbLog.Items.Count > 15)
                lbLog.Items.RemoveAt(0);
            string Message = string.Format("Encoder Started @ {0}" ,iPort);
            lbLog.Items.Add(Message);
            objSrv.GetOnlineUserCountAsync(tChId.Text);
            return iPort;
        }

        private void StopEnc()
        {
            Encoder.Stop();
            tmrStatus.Enabled = false;
            if (lbLog.Items.Count > 15)
                lbLog.Items.RemoveAt(0);
            string Message = string.Format("Encoder Stoped");
            lbLog.Items.Add(Message);
        }
        #endregion

        #region Menu Action


        private void tsmAddFile_Click(object sender, EventArgs e)
        {
            doAddFile();
        }

        private void tsmRemoveFile_Click(object sender, EventArgs e)
        {
            doRemoveFile();
        }

        private void tsmUp_Click(object sender, EventArgs e)
        {
            doUp();
        }

        private void tsmDown_Click(object sender, EventArgs e)
        {
            doDown();
        }

        private void tsmRunThis_Click(object sender, EventArgs e)
        {
            doRunThis();
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
            m_Channel.Profile.WriteToFile();
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
                        //AsfHeader header = new AsfHeader();
                        //header.InitFromFile(f);
                        //mFile.Duration = header.SendDuration;
                        //if (header.MaxBitrate > 660 * 1024) //650Kbps
                        //{
                        //    MessageBox.Show(string.Format("{0}'s Bitrate is too large!", f), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //    continue;
                        //}
                        lvFiles.BeginUpdate();
                        ListViewItem item = new ListViewItem();
                        item.Text = (lvFiles.Items.Count + 1).ToString();
                        item.SubItems.Add(mFile.FileName);
                        item.SubItems.Add(mFile.Duration.ToString());
                        item.SubItems.Add("");
                        item.Tag = mFile.ID;
                        lvFiles.Items.Add(item);

                        //m_Channel.Profile.Files.Add(mFile);
                        ReindexFiles();
                        if (Encoder.RunState == WMENC_ENCODER_STATE.WMENC_ENCODER_RUNNING)
                        {
                            IWMEncSourceGroup2 SrcGrp=null;
                            try
                            {
                                
                                SrcGrp = (IWMEncSourceGroup2)Encoder.SourceGroupCollection.Add(mFile.ID);//string.Format("SG_{0}",iCounter)
                                SrcGrp.SetAutoRollover(-1, "WMENC_SOURCEGROUP_AUTOROLLOVER_TO_NEXT");
                                SrcGrp.AutoSetFileSource(mFile.FileName);
                                //  SrcGrp.get_Source(WMENC_SOURCE_TYPE.WMENC_VIDEO , SrcGrp.get_SourceCount(WMENC_SOURCE_TYPE.WMENC_VIDEO)).Repeat=true;
                                SrcGrp.set_Profile(Encoder.ProfileCollection.Item(cbProfiles.SelectedIndex));
                            }
                            catch
                            {
                                if(SrcGrp!=null)
                                    Encoder.SourceGroupCollection.Remove(SrcGrp);
                                m_Channel.Profile.Files.Remove(mFile);
                                item.Remove();
                            }
                        }
                        lvFiles.EndUpdate();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        
            if (Encoder.RunState == WMENC_ENCODER_STATE.WMENC_ENCODER_RUNNING)
            {
                Encoder.SourceGroupCollection.Remove(item.Index);
            }

            item.Remove();
            ReindexFiles();
        }

        private void doUp()
        {
            if (lvFiles.SelectedItems.Count == 0 || lvFiles.SelectedIndices[0] == 0)
                return;
            
            int index = lvFiles.SelectedItems[0].Index;
            string SrcId = lvFiles.SelectedItems[0].Tag.ToString();
            string TgtId = lvFiles.Items[index - 1].Tag.ToString();

            ListViewItem item = lvFiles.SelectedItems[0];
            item.Remove();
            lvFiles.Items.Insert(index - 1, item);
            ReindexFiles();

            if (Encoder.RunState == WMENC_ENCODER_STATE.WMENC_ENCODER_RUNNING)
            {
                Encoder.SourceGroupCollection.Move(Encoder.SourceGroupCollection.Item(SrcId), Encoder.SourceGroupCollection.Item(TgtId));    
            }
        }

        private void doDown()
        {
            if (lvFiles.SelectedItems.Count == 0 || lvFiles.SelectedIndices[0] == lvFiles.Items.Count - 1)
                return;
            int index = lvFiles.SelectedItems[0].Index;
            ListViewItem item = lvFiles.SelectedItems[0];

            string SrcId = lvFiles.SelectedItems[0].Tag.ToString();
            string TgtId = lvFiles.Items[index + 1].Tag.ToString();
            item.Remove();
            lvFiles.Items.Insert(index + 1, item);
            ReindexFiles();
            if (Encoder.RunState == WMENC_ENCODER_STATE.WMENC_ENCODER_RUNNING)
            {
                Encoder.SourceGroupCollection.Move(Encoder.SourceGroupCollection.Item(SrcId), Encoder.SourceGroupCollection.Item(TgtId));
            }
        }

        private void doRunThis()
        {
            try
            {
                if (Channel == null || lvFiles.SelectedItems.Count == 0 || (Channel.Profile.Custom == lvFiles.SelectedItems[0].Tag.ToString() && Channel.Status == Status.Running))
                    return;
                if (Encoder.RunState != WMENC_ENCODER_STATE.WMENC_ENCODER_RUNNING)
                {
                    return;
                }
                Channel.Profile.Custom = lvFiles.SelectedItems[0].Tag.ToString();
                Encoder.SourceGroupCollection.Active = Encoder.SourceGroupCollection.Item(lvFiles.SelectedItems[0].Tag.ToString());
            }
            catch { }
            //Channel.Stop();
            //System.Threading.Thread.Sleep(1000);
            //Channel.Start();
        }
        #endregion  

        private void cmFileSource_Opening(object sender, CancelEventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
            {
                tsmUp.Enabled = tsmDown.Enabled = tsmRemoveFile.Enabled = tsmRunThis.Enabled = false;
            }
            else
            {
                 tsmUp.Enabled = tsmDown.Enabled = tsmRemoveFile.Enabled = tsmRunThis.Enabled = true;
            }
            if (Encoder.RunState == WMENC_ENCODER_STATE.WMENC_ENCODER_RUNNING && lvFiles.SelectedItems.Count > 0)
                tsmRunThis.Enabled = true;
            else
                tsmRunThis.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                RefreshStatus();
            }
            catch { }
        }

        private void chkReview_CheckedChanged(object sender, EventArgs e)
        {
            picReview.Visible = chkReview.Checked;
            if (Encoder.RunState != WMENC_ENCODER_STATE.WMENC_ENCODER_RUNNING)
                return;
            try
            {
                IWMEncVideoSource SrcVid = (IWMEncVideoSource)Encoder.SourceGroupCollection.Active.get_Source(WMENC_SOURCE_TYPE.WMENC_VIDEO, 0);
                if (chkReview.Checked && SrcVid != null)
                {
                    WMEncDataView Postview;
                    object obj;
                    int lPostviewStream;
                    lPostviewStream = SrcVid.PostviewCollection.Item(0, out obj);
                    Postview = (WMEncDataView)obj;
                    Postview.SetViewProperties(lPostviewStream, (int)picReview.Handle);
                    if (chkReview.Checked)
                        Postview.StartView(lPostviewStream);
                        //Postview.StartAllViews();
                    else
                        Postview.StopView(lPostviewStream);
                }
            }
            catch { }
        }



        private void tmrStatus_Tick(object sender, EventArgs e)
        {
            IWMEncStatistics Stats = Encoder.Statistics;
                      
            // Retrieve an IWMEncOutputStats object and display the current bit rate.
            IWMEncOutputStats OutputStats = (IWMEncOutputStats)Stats.WMFOutputStats;
            decimal BRate = OutputStats.CurrentBitrate / 1000;
            
            lblAverageBitrate.Text = string.Format("{0} Kbps", OutputStats.AverageBitrate / 1000);
            lblAverageSampleRate.Text = string.Format("{0} fps", OutputStats.AverageSampleRate/1000);

            lblCurrentBitrate.Text = BRate.ToString() + " Kbps";
            lblCurrentSampleRate.Text = string.Format("{0} fps", OutputStats.CurrentSampleRate/1000);

            lblByteCount.Text = string.Format("{0:F} MB", OutputStats.ByteCount * 10000 / 1024 / 1024);
            lblSampleCount.Text = string.Format("{0} ", (int)(OutputStats.SampleCount * 10000));
            TimeSpan ts = DateTime.Now - dtStartTime;
            if(ts.Days>0)
                lblDuration.Text = string.Format("{3}.{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds,ts.Days);
            else
                lblDuration.Text = string.Format("{0}:{1}:{2}",ts.Hours,ts.Minutes,ts.Seconds);
           
            
        }

        private void lblDuration_Click(object sender, EventArgs e)
        {

        }

        private void tmrAuth_Tick(object sender, EventArgs e)
        {
            if (!ConfigClass.AuthResult.IsAdmin)
            {
                foreach (Srv.Channel ch in ConfigClass.AuthResult.Channels)
                {
                    if (ch.ChId == tChId.Text)
                    {
                        if (DateTime.Now > ch.ExpireDate)
                        {
                            btStop_Click(null, null);
                        }
                        break;
                    }
                }
            }
        }
    }
}
