namespace Ankh.Pusher
{
    partial class ChPanelExt
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkReview = new System.Windows.Forms.CheckBox();
            this.picReview = new System.Windows.Forms.PictureBox();
            this.btStop = new System.Windows.Forms.Button();
            this.btStart = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpProperty = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tSrvPort = new System.Windows.Forms.TextBox();
            this.tSrvIp = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.gbProperty = new System.Windows.Forms.GroupBox();
            this.tChDesc = new System.Windows.Forms.TextBox();
            this.tChName = new System.Windows.Forms.TextBox();
            this.tChId = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tpSource = new System.Windows.Forms.TabPage();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.cmFileSource = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmAddFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmRemoveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDown = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmRunThis = new System.Windows.Forms.ToolStripMenuItem();
            this.tpSetting = new System.Windows.Forms.TabPage();
            this.gbResizeAndCropping = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.gbProfile = new System.Windows.Forms.GroupBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbProfiles = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tpLog = new System.Windows.Forms.TabPage();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblOnlineUser = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblSampleCount = new System.Windows.Forms.Label();
            this.lblByteCount = new System.Windows.Forms.Label();
            this.lblCurrentSampleRate = new System.Windows.Forms.Label();
            this.lblCurrentBitrate = new System.Windows.Forms.Label();
            this.lblAverageSampleRate = new System.Windows.Forms.Label();
            this.lblAverageBitrate = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tmrStatus = new System.Windows.Forms.Timer(this.components);
            this.tmrAuth = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picReview)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tpProperty.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbProperty.SuspendLayout();
            this.tpSource.SuspendLayout();
            this.cmFileSource.SuspendLayout();
            this.tpSetting.SuspendLayout();
            this.gbResizeAndCropping.SuspendLayout();
            this.gbProfile.SuspendLayout();
            this.tpLog.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkReview);
            this.panel1.Controls.Add(this.picReview);
            this.panel1.Controls.Add(this.btStop);
            this.panel1.Controls.Add(this.btStart);
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(547, 63);
            this.panel1.TabIndex = 1;
            // 
            // chkReview
            // 
            this.chkReview.AutoSize = true;
            this.chkReview.Location = new System.Drawing.Point(283, 38);
            this.chkReview.Name = "chkReview";
            this.chkReview.Size = new System.Drawing.Size(69, 19);
            this.chkReview.TabIndex = 9;
            this.chkReview.Text = "Preview";
            this.chkReview.UseVisualStyleBackColor = true;
            this.chkReview.Visible = false;
            this.chkReview.CheckedChanged += new System.EventHandler(this.chkReview_CheckedChanged);
            // 
            // picReview
            // 
            this.picReview.BackColor = System.Drawing.SystemColors.Control;
            this.picReview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picReview.Location = new System.Drawing.Point(197, 1);
            this.picReview.Name = "picReview";
            this.picReview.Size = new System.Drawing.Size(80, 60);
            this.picReview.TabIndex = 8;
            this.picReview.TabStop = false;
            this.picReview.Visible = false;
            // 
            // btStop
            // 
            this.btStop.AutoSize = true;
            this.btStop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btStop.FlatAppearance.BorderSize = 0;
            this.btStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btStop.Image = global::Ankh.Pusher.Properties.Resources.stop_off;
            this.btStop.Location = new System.Drawing.Point(413, 16);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(37, 37);
            this.btStop.TabIndex = 6;
            this.btStop.UseVisualStyleBackColor = true;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // btStart
            // 
            this.btStart.AutoSize = true;
            this.btStart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btStart.FlatAppearance.BorderSize = 0;
            this.btStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btStart.Image = global::Ankh.Pusher.Properties.Resources.play_off;
            this.btStart.Location = new System.Drawing.Point(361, 16);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(37, 37);
            this.btStart.TabIndex = 5;
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(69, 34);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(54, 15);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Running";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Status:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblName.Location = new System.Drawing.Point(19, 12);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(0, 16);
            this.lblName.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpProperty);
            this.tabControl1.Controls.Add(this.tpSource);
            this.tabControl1.Controls.Add(this.tpSetting);
            this.tabControl1.Controls.Add(this.tpLog);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 63);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(547, 357);
            this.tabControl1.TabIndex = 2;
            // 
            // tpProperty
            // 
            this.tpProperty.Controls.Add(this.groupBox1);
            this.tpProperty.Controls.Add(this.gbProperty);
            this.tpProperty.Location = new System.Drawing.Point(4, 24);
            this.tpProperty.Name = "tpProperty";
            this.tpProperty.Padding = new System.Windows.Forms.Padding(3);
            this.tpProperty.Size = new System.Drawing.Size(539, 329);
            this.tpProperty.TabIndex = 3;
            this.tpProperty.Text = "Property";
            this.tpProperty.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tSrvPort);
            this.groupBox1.Controls.Add(this.tSrvIp);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(6, 125);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(456, 74);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Publish Server\'s Property";
            // 
            // tSrvPort
            // 
            this.tSrvPort.Location = new System.Drawing.Point(319, 29);
            this.tSrvPort.Name = "tSrvPort";
            this.tSrvPort.Size = new System.Drawing.Size(118, 21);
            this.tSrvPort.TabIndex = 8;
            this.tSrvPort.TextChanged += new System.EventHandler(this.tSrvPort_TextChanged);
            // 
            // tSrvIp
            // 
            this.tSrvIp.Location = new System.Drawing.Point(93, 29);
            this.tSrvIp.Name = "tSrvIp";
            this.tSrvIp.Size = new System.Drawing.Size(118, 21);
            this.tSrvIp.TabIndex = 7;
            this.tSrvIp.TextChanged += new System.EventHandler(this.tSrvIp_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(237, 29);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 15);
            this.label9.TabIndex = 6;
            this.label9.Text = "Server Port:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 15);
            this.label8.TabIndex = 5;
            this.label8.Text = "Server IP:";
            // 
            // gbProperty
            // 
            this.gbProperty.Controls.Add(this.tChDesc);
            this.gbProperty.Controls.Add(this.tChName);
            this.gbProperty.Controls.Add(this.tChId);
            this.gbProperty.Controls.Add(this.label7);
            this.gbProperty.Controls.Add(this.label6);
            this.gbProperty.Controls.Add(this.label5);
            this.gbProperty.Location = new System.Drawing.Point(6, 5);
            this.gbProperty.Name = "gbProperty";
            this.gbProperty.Size = new System.Drawing.Size(456, 115);
            this.gbProperty.TabIndex = 0;
            this.gbProperty.TabStop = false;
            this.gbProperty.Text = "Channel\'s Property";
            // 
            // tChDesc
            // 
            this.tChDesc.Location = new System.Drawing.Point(93, 60);
            this.tChDesc.Name = "tChDesc";
            this.tChDesc.Size = new System.Drawing.Size(344, 21);
            this.tChDesc.TabIndex = 5;
            this.tChDesc.TextChanged += new System.EventHandler(this.tChDesc_TextChanged);
            // 
            // tChName
            // 
            this.tChName.Location = new System.Drawing.Point(319, 30);
            this.tChName.Name = "tChName";
            this.tChName.Size = new System.Drawing.Size(118, 21);
            this.tChName.TabIndex = 4;
            this.tChName.TextChanged += new System.EventHandler(this.tChName_TextChanged);
            // 
            // tChId
            // 
            this.tChId.Location = new System.Drawing.Point(93, 30);
            this.tChId.Name = "tChId";
            this.tChId.Size = new System.Drawing.Size(118, 21);
            this.tChId.TabIndex = 3;
            this.tChId.TextChanged += new System.EventHandler(this.tChId_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 15);
            this.label7.TabIndex = 2;
            this.label7.Text = "Description:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(217, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 15);
            this.label6.TabIndex = 1;
            this.label6.Text = "Channel Name:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "Channel ID:";
            // 
            // tpSource
            // 
            this.tpSource.Controls.Add(this.lvFiles);
            this.tpSource.Location = new System.Drawing.Point(4, 24);
            this.tpSource.Name = "tpSource";
            this.tpSource.Padding = new System.Windows.Forms.Padding(3);
            this.tpSource.Size = new System.Drawing.Size(539, 329);
            this.tpSource.TabIndex = 0;
            this.tpSource.Text = "Source";
            this.tpSource.UseVisualStyleBackColor = true;
            // 
            // lvFiles
            // 
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lvFiles.ContextMenuStrip = this.cmFileSource;
            this.lvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFiles.FullRowSelect = true;
            this.lvFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvFiles.HideSelection = false;
            this.lvFiles.Location = new System.Drawing.Point(3, 3);
            this.lvFiles.MultiSelect = false;
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(533, 323);
            this.lvFiles.TabIndex = 6;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            this.columnHeader1.Width = 30;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "File Name";
            this.columnHeader2.Width = 298;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Duration";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Status";
            // 
            // cmFileSource
            // 
            this.cmFileSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmFileSource.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmAddFile,
            this.tsmRemoveFile,
            this.tsmUp,
            this.tsmDown,
            this.tsmRunThis});
            this.cmFileSource.Name = "cmFileSource";
            this.cmFileSource.Size = new System.Drawing.Size(144, 114);
            this.cmFileSource.Opening += new System.ComponentModel.CancelEventHandler(this.cmFileSource_Opening);
            // 
            // tsmAddFile
            // 
            this.tsmAddFile.Image = global::Ankh.Pusher.Properties.Resources.DocumentHS;
            this.tsmAddFile.Name = "tsmAddFile";
            this.tsmAddFile.Size = new System.Drawing.Size(143, 22);
            this.tsmAddFile.Text = "Add File";
            this.tsmAddFile.Click += new System.EventHandler(this.tsmAddFile_Click);
            // 
            // tsmRemoveFile
            // 
            this.tsmRemoveFile.Image = global::Ankh.Pusher.Properties.Resources.DeleteHS;
            this.tsmRemoveFile.Name = "tsmRemoveFile";
            this.tsmRemoveFile.Size = new System.Drawing.Size(143, 22);
            this.tsmRemoveFile.Text = "Remove File";
            this.tsmRemoveFile.Click += new System.EventHandler(this.tsmRemoveFile_Click);
            // 
            // tsmUp
            // 
            this.tsmUp.Image = global::Ankh.Pusher.Properties.Resources.GoToPreviousMessage;
            this.tsmUp.Name = "tsmUp";
            this.tsmUp.Size = new System.Drawing.Size(143, 22);
            this.tsmUp.Text = "Up";
            this.tsmUp.Click += new System.EventHandler(this.tsmUp_Click);
            // 
            // tsmDown
            // 
            this.tsmDown.Image = global::Ankh.Pusher.Properties.Resources.GoToNextMessage;
            this.tsmDown.Name = "tsmDown";
            this.tsmDown.Size = new System.Drawing.Size(143, 22);
            this.tsmDown.Text = "Down";
            this.tsmDown.Click += new System.EventHandler(this.tsmDown_Click);
            // 
            // tsmRunThis
            // 
            this.tsmRunThis.Image = global::Ankh.Pusher.Properties.Resources.FormRunHS;
            this.tsmRunThis.Name = "tsmRunThis";
            this.tsmRunThis.Size = new System.Drawing.Size(143, 22);
            this.tsmRunThis.Text = "Run this!";
            this.tsmRunThis.Click += new System.EventHandler(this.tsmRunThis_Click);
            // 
            // tpSetting
            // 
            this.tpSetting.Controls.Add(this.gbResizeAndCropping);
            this.tpSetting.Controls.Add(this.gbProfile);
            this.tpSetting.Location = new System.Drawing.Point(4, 24);
            this.tpSetting.Name = "tpSetting";
            this.tpSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tpSetting.Size = new System.Drawing.Size(539, 329);
            this.tpSetting.TabIndex = 1;
            this.tpSetting.Text = "Setting";
            this.tpSetting.UseVisualStyleBackColor = true;
            // 
            // gbResizeAndCropping
            // 
            this.gbResizeAndCropping.Controls.Add(this.label18);
            this.gbResizeAndCropping.Location = new System.Drawing.Point(7, 100);
            this.gbResizeAndCropping.Name = "gbResizeAndCropping";
            this.gbResizeAndCropping.Size = new System.Drawing.Size(526, 155);
            this.gbResizeAndCropping.TabIndex = 3;
            this.gbResizeAndCropping.TabStop = false;
            this.gbResizeAndCropping.Text = "Resize and Cropping";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(30, 38);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(102, 15);
            this.label18.TabIndex = 0;
            this.label18.Text = "Not Implemented";
            // 
            // gbProfile
            // 
            this.gbProfile.Controls.Add(this.lblDescription);
            this.gbProfile.Controls.Add(this.label4);
            this.gbProfile.Controls.Add(this.cbProfiles);
            this.gbProfile.Controls.Add(this.label3);
            this.gbProfile.Location = new System.Drawing.Point(6, 5);
            this.gbProfile.Name = "gbProfile";
            this.gbProfile.Size = new System.Drawing.Size(527, 88);
            this.gbProfile.TabIndex = 2;
            this.gbProfile.TabStop = false;
            this.gbProfile.Text = "Profile";
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(86, 52);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(424, 34);
            this.lblDescription.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Description:";
            // 
            // cbProfiles
            // 
            this.cbProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProfiles.FormattingEnabled = true;
            this.cbProfiles.Location = new System.Drawing.Point(85, 19);
            this.cbProfiles.Name = "cbProfiles";
            this.cbProfiles.Size = new System.Drawing.Size(425, 23);
            this.cbProfiles.TabIndex = 3;
            this.cbProfiles.SelectedIndexChanged += new System.EventHandler(this.cbProfiles_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Profile:";
            // 
            // tpLog
            // 
            this.tpLog.Controls.Add(this.lbLog);
            this.tpLog.Controls.Add(this.panel4);
            this.tpLog.Location = new System.Drawing.Point(4, 24);
            this.tpLog.Name = "tpLog";
            this.tpLog.Padding = new System.Windows.Forms.Padding(3);
            this.tpLog.Size = new System.Drawing.Size(539, 329);
            this.tpLog.TabIndex = 2;
            this.tpLog.Text = "Log";
            this.tpLog.UseVisualStyleBackColor = true;
            // 
            // lbLog
            // 
            this.lbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLog.FormattingEnabled = true;
            this.lbLog.ItemHeight = 15;
            this.lbLog.Location = new System.Drawing.Point(3, 152);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(533, 169);
            this.lbLog.TabIndex = 4;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.groupBox2);
            this.panel4.Controls.Add(this.label12);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(3, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(533, 149);
            this.panel4.TabIndex = 5;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblOnlineUser);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.lblDuration);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.lblSampleCount);
            this.groupBox2.Controls.Add(this.lblByteCount);
            this.groupBox2.Controls.Add(this.lblCurrentSampleRate);
            this.groupBox2.Controls.Add(this.lblCurrentBitrate);
            this.groupBox2.Controls.Add(this.lblAverageSampleRate);
            this.groupBox2.Controls.Add(this.lblAverageBitrate);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(514, 125);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Statistics";
            // 
            // lblOnlineUser
            // 
            this.lblOnlineUser.AutoSize = true;
            this.lblOnlineUser.Location = new System.Drawing.Point(359, 21);
            this.lblOnlineUser.Name = "lblOnlineUser";
            this.lblOnlineUser.Size = new System.Drawing.Size(0, 15);
            this.lblOnlineUser.TabIndex = 15;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(279, 20);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(73, 15);
            this.label17.TabIndex = 14;
            this.label17.Text = "OnlineUser:";
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(108, 20);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(0, 15);
            this.lblDuration.TabIndex = 13;
            this.lblDuration.Click += new System.EventHandler(this.lblDuration_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(45, 20);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(57, 15);
            this.label16.TabIndex = 12;
            this.label16.Text = "Duration:";
            // 
            // lblSampleCount
            // 
            this.lblSampleCount.AutoSize = true;
            this.lblSampleCount.Location = new System.Drawing.Point(362, 101);
            this.lblSampleCount.Name = "lblSampleCount";
            this.lblSampleCount.Size = new System.Drawing.Size(0, 15);
            this.lblSampleCount.TabIndex = 11;
            // 
            // lblByteCount
            // 
            this.lblByteCount.AutoSize = true;
            this.lblByteCount.Location = new System.Drawing.Point(109, 101);
            this.lblByteCount.Name = "lblByteCount";
            this.lblByteCount.Size = new System.Drawing.Size(0, 15);
            this.lblByteCount.TabIndex = 10;
            // 
            // lblCurrentSampleRate
            // 
            this.lblCurrentSampleRate.AutoSize = true;
            this.lblCurrentSampleRate.Location = new System.Drawing.Point(362, 73);
            this.lblCurrentSampleRate.Name = "lblCurrentSampleRate";
            this.lblCurrentSampleRate.Size = new System.Drawing.Size(0, 15);
            this.lblCurrentSampleRate.TabIndex = 9;
            // 
            // lblCurrentBitrate
            // 
            this.lblCurrentBitrate.AutoSize = true;
            this.lblCurrentBitrate.Location = new System.Drawing.Point(109, 73);
            this.lblCurrentBitrate.Name = "lblCurrentBitrate";
            this.lblCurrentBitrate.Size = new System.Drawing.Size(0, 15);
            this.lblCurrentBitrate.TabIndex = 8;
            // 
            // lblAverageSampleRate
            // 
            this.lblAverageSampleRate.AutoSize = true;
            this.lblAverageSampleRate.Location = new System.Drawing.Point(362, 47);
            this.lblAverageSampleRate.Name = "lblAverageSampleRate";
            this.lblAverageSampleRate.Size = new System.Drawing.Size(0, 15);
            this.lblAverageSampleRate.TabIndex = 7;
            // 
            // lblAverageBitrate
            // 
            this.lblAverageBitrate.AutoSize = true;
            this.lblAverageBitrate.Location = new System.Drawing.Point(109, 47);
            this.lblAverageBitrate.Name = "lblAverageBitrate";
            this.lblAverageBitrate.Size = new System.Drawing.Size(0, 15);
            this.lblAverageBitrate.TabIndex = 6;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(266, 101);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(86, 15);
            this.label15.TabIndex = 5;
            this.label15.Text = "SampleCount:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(36, 101);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(66, 15);
            this.label14.TabIndex = 4;
            this.label14.Text = "ByteCount:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(232, 73);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(120, 15);
            this.label13.TabIndex = 3;
            this.label13.Text = "CurrentSampleRate:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 73);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(86, 15);
            this.label11.TabIndex = 2;
            this.label11.Text = "CurrentBitrate:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(229, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(123, 15);
            this.label10.TabIndex = 1;
            this.label10.Text = "AverageSampleRate:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "AverageBitrate:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 131);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 15);
            this.label12.TabIndex = 2;
            this.label12.Text = "Channel Log:";
            // 
            // dlgOpen
            // 
            this.dlgOpen.Filter = "All files|*.*";
            this.dlgOpen.Multiselect = true;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tmrStatus
            // 
            this.tmrStatus.Interval = 1000;
            this.tmrStatus.Tick += new System.EventHandler(this.tmrStatus_Tick);
            // 
            // tmrAuth
            // 
            this.tmrAuth.Interval = 1;
            this.tmrAuth.Tick += new System.EventHandler(this.tmrAuth_Tick);
            // 
            // ChPanelExt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ChPanelExt";
            this.Size = new System.Drawing.Size(547, 420);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picReview)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tpProperty.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbProperty.ResumeLayout(false);
            this.gbProperty.PerformLayout();
            this.tpSource.ResumeLayout(false);
            this.cmFileSource.ResumeLayout(false);
            this.tpSetting.ResumeLayout(false);
            this.gbResizeAndCropping.ResumeLayout(false);
            this.gbResizeAndCropping.PerformLayout();
            this.gbProfile.ResumeLayout(false);
            this.gbProfile.PerformLayout();
            this.tpLog.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpSource;
        private System.Windows.Forms.TabPage tpSetting;
        private System.Windows.Forms.TabPage tpLog;
        private System.Windows.Forms.ListBox lbLog;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ContextMenuStrip cmFileSource;
        private System.Windows.Forms.ToolStripMenuItem tsmAddFile;
        private System.Windows.Forms.ToolStripMenuItem tsmRemoveFile;
        private System.Windows.Forms.ToolStripMenuItem tsmUp;
        private System.Windows.Forms.ToolStripMenuItem tsmDown;
        private System.Windows.Forms.ToolStripMenuItem tsmRunThis;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.GroupBox gbProfile;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbProfiles;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox gbResizeAndCropping;
        private System.Windows.Forms.TabPage tpProperty;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.GroupBox gbProperty;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.PictureBox picReview;
        private System.Windows.Forms.CheckBox chkReview;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSampleCount;
        private System.Windows.Forms.Label lblByteCount;
        private System.Windows.Forms.Label lblCurrentSampleRate;
        private System.Windows.Forms.Label lblCurrentBitrate;
        private System.Windows.Forms.Label lblAverageSampleRate;
        private System.Windows.Forms.Label lblAverageBitrate;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Timer tmrStatus;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblOnlineUser;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        internal System.Windows.Forms.TextBox tSrvIp;
        internal System.Windows.Forms.TextBox tChDesc;
        internal System.Windows.Forms.TextBox tChName;
        internal System.Windows.Forms.TextBox tChId;
        internal System.Windows.Forms.TextBox tSrvPort;
        private System.Windows.Forms.Timer tmrAuth;
    }
}
