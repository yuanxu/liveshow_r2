using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace Ankh.Pusher
{
    public partial class frmMain : Form
    {
        private IList<ChPanel> m_Chs = new List<ChPanel>();
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(".", "*.xml");
            foreach (string file in files)
            {
                FileInfo fInfo = new FileInfo(file);
                string sTmp = fInfo.Name.Substring(0, fInfo.Name.Length - 4);
                ChPanel ChObj = new ChPanel();
                ChObj.ChannelID = ushort.Parse(sTmp);
                m_Chs.Add(ChObj);
                TreeNode node = new TreeNode();
                node.Text = ChObj.ChannelID.ToString() + "[" + ChObj.Channel.Profile.Name + "]";
                node.Tag = ChObj;
                ChObj.Dock = DockStyle.Fill;
                tvChannels.Nodes.Add(node);
            }
        }

        private void tvChannels_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //splitContainer1.Panel2.Controls.Clear();
            //splitContainer1.Panel2.Controls.Add((ChPanel)tvChannels.SelectedNode.Tag );
        }

        private void tvChannels_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(splitContainer1.Panel2.Controls.Count != 0)
                ((ChPanel)splitContainer1.Panel2.Controls[0]).ProfileChanged -= new ChannelProfileChangedEventHandler(frmMain_ProfileChanged);
            splitContainer1.Panel2.Controls.Clear();
            splitContainer1.Panel2.Controls.Add((ChPanel)tvChannels.SelectedNode.Tag);
            ((ChPanel)splitContainer1.Panel2.Controls[0]).ProfileChanged+=new ChannelProfileChangedEventHandler(frmMain_ProfileChanged);
        }

        void frmMain_ProfileChanged(object Sender, ChannelArgs e)
        {
            tvChannels.SelectedNode.Text = string.Format("{0}[{1}]",e.Channel.ChannelID,e.Channel.Profile.Name );
        }

        private void newChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doNew();
        }

        private void StopAll()
        {
            foreach (ChPanel ch in m_Chs)
            {
                if (ch.Channel.Status == Ankh.Pusher.Core.Status.Running)
                {
                    //ch.Stop();
                }
                ch.Channel.Profile.WriteToFile();
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                StopAll();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            doNew();
        }

        #region ²Ëµ¥ÃüÁî
        
        private void doNew()
        {
            TreeNode node = new TreeNode();
            ChPanel ChObj = new ChPanel();
            ChObj.ChannelID = 0;
            m_Chs.Add(ChObj);
            node.Text = ChObj.ChannelID.ToString() + "[" + ChObj.Channel.Profile.Name + "]";
            node.Tag = ChObj;
            ChObj.Dock = DockStyle.Fill;
            tvChannels.Nodes.Add(node);
        }

        private void doDelete()
        {
            if (tvChannels.SelectedNode == null)
                return;
            if (MessageBox.Show("Are you sure to delete the selected channel?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;
            
            ChPanel ch = (ChPanel)tvChannels.SelectedNode.Tag;
            ch.Channel.Stop();
            m_Chs.Remove(ch);
            File.Delete(string.Format("{0}{1}.xml",AppDomain.CurrentDomain.BaseDirectory ,ch.Channel.Profile.ChannelID ));
            tvChannels.Nodes.Remove(tvChannels.SelectedNode);

        }

        private void doPreviouse()
        {
            tvChannels.SelectedNode = tvChannels.SelectedNode.PrevNode;
            
        }

        private void doNext()
        {
            tvChannels.SelectedNode = tvChannels.SelectedNode.NextNode ;
        }

        private void doStartAll()
        {
            foreach (ChPanel ch in m_Chs)
            {
                if (ch.Channel.Status != Ankh.Pusher.Core.Status.Running) ;
                    //ch.Start();
            }
        }

        private void doStopAll()
        {
            StopAll();
        }

        private string m_ErrorChs="";
        private void doSetToolBar()
        {
            
            if (m_Chs.Count == 0)
            {
                toolStripButton2.Enabled = false;
                toolStripButton3.Enabled = false;
                toolStripButton4.Enabled = false;
                toolStripButton5.Enabled = false;
                toolStripButton6.Enabled = false;
            }
            else
            {
                if (tvChannels.SelectedNode == null)
                {
                    toolStripButton2.Enabled = false;
                    toolStripButton3.Enabled = false;
                    toolStripButton4.Enabled = false;
                }
                else
                {
                    toolStripButton2.Enabled = true;//delete
                    toolStripButton3.Enabled = tvChannels.SelectedNode.PrevNode != null;
                    toolStripButton4.Enabled = tvChannels.SelectedNode.NextNode != null;
                }
                toolStripButton5.Enabled = true;//start all
                toolStripButton6.Enabled = true;//stop all
            }
            //×´Ì¬À¸
            int iRun = 0,iError = 0, iStop = 0;
            System.Text.StringBuilder objBuilder = new StringBuilder();
            foreach (ChPanel ch in m_Chs)
            {
                switch (ch.Channel.Status)
                {
                    case Ankh.Pusher.Core.Status.Running:
                        iRun++;
                        break;
                    case Ankh.Pusher.Core.Status.Error:
                        objBuilder.Append("\r\n" + ch.Channel.Profile.Name);
                        iError++;
                        break;
                    case Ankh.Pusher.Core.Status.Stoped :
                        iStop++;
                        break;
                }
            }
            if (objBuilder.Length == 0)
                m_ErrorChs = "";
            else
                m_ErrorChs = "Failed Channels:"+objBuilder.ToString();
            toolStripStatusLabel1.Text  = string.Format("Running:{0},Stoped:{1},Error:{2}",iRun,iStop,iError);
        }

        
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            doDelete();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            doPreviouse();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            doNext();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            doStartAll();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            doStopAll();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            doSetToolBar();
        }
        #endregion

        private void statusStrip1_DoubleClick(object sender, EventArgs e)
        {
            if (m_ErrorChs.Length == 0)
                return;
            MessageBox.Show(m_ErrorChs, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doNew();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doDelete();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ChPanel)tvChannels.SelectedNode.Tag).Channel.Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ChPanel)tvChannels.SelectedNode.Tag).Channel.Stop();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (tvChannels.SelectedNode == null)
            {
                deleteToolStripMenuItem.Enabled = startToolStripMenuItem.Enabled = stopToolStripMenuItem.Enabled = false;
            }
            else
            {
                deleteToolStripMenuItem.Enabled = startToolStripMenuItem.Enabled = stopToolStripMenuItem.Enabled = true;
                switch(((ChPanel)tvChannels.SelectedNode.Tag).Channel.Status )
                {
                    case Ankh.Pusher.Core.Status.Running:
                        startToolStripMenuItem.Enabled = false;
                        break;
                    case Ankh.Pusher.Core.Status.Stoped:
                    case  Ankh.Pusher.Core.Status.Error:
                        stopToolStripMenuItem.Enabled = false;
                        break;
                }
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            //if (ConfigClass.AuthResult == null)
            //{
            //    frmSignIn frm = new frmSignIn();
            //    if (frm.ShowDialog() == DialogResult.Cancel)
            //    {
            //        ConfigClass.AuthResult = new Ankh.Pusher.Srv.AuthResult();
            //        ConfigClass.AuthResult.IsAuthenticated = false;
            //        FormClosing -= this.frmMain_FormClosing;
            //        Close();
            //        Application.Exit();
            //    }
            //    else
            //    {
            //        if (ConfigClass.AuthResult.IsAdmin)
            //        {
            //            string[] files = Directory.GetFiles(".", "*.xml");
            //            foreach (string file in files)
            //            {
            //                FileInfo fInfo = new FileInfo(file);
            //                string sTmp = fInfo.Name.Substring(0, fInfo.Name.Length - 4);
            //                ChPanelExt ChObj = new ChPanelExt();
            //                ChObj.ChannelID = ushort.Parse(sTmp);
            //                m_Chs.Add(ChObj);
            //                TreeNode node = new TreeNode();
            //                node.Text = ChObj.ChannelID.ToString() + "[" + ChObj.Channel.Profile.Name + "]";
            //                node.Tag = ChObj;
            //                ChObj.Dock = DockStyle.Fill;
            //                tvChannels.Nodes.Add(node);
            //            }
            //        }
            //        else
            //        {
            //            foreach (Srv.Channel ch in ConfigClass.AuthResult.Channels)
            //            {
            //                ChPanelExt ChObj = new ChPanelExt();
            //                ChObj.ChannelID = ushort.Parse(ch.ChId);
            //                m_Chs.Add(ChObj);
            //                ChObj.tChName.Text = ch.ChName;
            //                ChObj.tChDesc.Text = ch.Description;
            //                ChObj.tSrvIp.Text = ch.SrvIP;
            //                ChObj.tSrvPort.Text = ch.SrvPort;
            //                TreeNode node = new TreeNode(); 
            //                ChObj.Channel.Profile.Name = ch.ChName;
            //                node.Text = ChObj.ChannelID.ToString() + "[" + ChObj.Channel.Profile.Name + "]";
            //                node.Tag = ChObj;
            //                ChObj.tChId.Enabled = ChObj.tChDesc.Enabled = ChObj.tChName.Enabled = ChObj.tSrvIp.Enabled = ChObj.tSrvPort.Enabled = false;
            //                ChObj.Dock = DockStyle.Fill;
            //                tvChannels.Nodes.Add(node);
            //            }
            //            newChannelToolStripMenuItem.Visible = false;
            //            newToolStripMenuItem.Visible = deleteToolStripMenuItem.Visible = toolStripButton1.Visible = toolStripButton2.Visible = toolStripSeparator1.Visible = false;
            //        }
            //    }
            //}
        }

       
    }
}