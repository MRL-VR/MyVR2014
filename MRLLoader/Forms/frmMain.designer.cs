namespace MRL.Components
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnSetting = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.gbbtn = new System.Windows.Forms.GroupBox();
            this.btnShowMenu = new System.Windows.Forms.Button();
            this.btnGetPose = new System.Windows.Forms.Button();
            this.gbAgents = new System.Windows.Forms.GroupBox();
            this.lstSpawn = new System.Windows.Forms.CheckedListBox();
            this.lstAgents = new System.Windows.Forms.CheckedListBox();
            this.dlgOF = new System.Windows.Forms.OpenFileDialog();
            this.cmsRun = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itmRun = new System.Windows.Forms.ToolStripMenuItem();
            this.itmRunTest = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxRobotProcess = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.gbbtn.SuspendLayout();
            this.gbAgents.SuspendLayout();
            this.cmsRun.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSetting
            // 
            this.btnSetting.BackColor = System.Drawing.Color.White;
            this.btnSetting.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSetting.ForeColor = System.Drawing.Color.Black;
            this.btnSetting.Location = new System.Drawing.Point(122, 12);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(111, 29);
            this.btnSetting.TabIndex = 1;
            this.btnSetting.Text = "Setting";
            this.btnSetting.UseVisualStyleBackColor = false;
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.White;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnStart.ForeColor = System.Drawing.Color.Black;
            this.btnStart.Location = new System.Drawing.Point(356, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(105, 29);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.White;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClose.ForeColor = System.Drawing.Color.Black;
            this.btnClose.Location = new System.Drawing.Point(6, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 29);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // gbbtn
            // 
            this.gbbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbbtn.Controls.Add(this.btnShowMenu);
            this.gbbtn.Controls.Add(this.btnGetPose);
            this.gbbtn.Controls.Add(this.btnClose);
            this.gbbtn.Controls.Add(this.btnStart);
            this.gbbtn.Controls.Add(this.btnSetting);
            this.gbbtn.Location = new System.Drawing.Point(7, 279);
            this.gbbtn.Name = "gbbtn";
            this.gbbtn.Size = new System.Drawing.Size(479, 47);
            this.gbbtn.TabIndex = 1;
            this.gbbtn.TabStop = false;
            // 
            // btnShowMenu
            // 
            this.btnShowMenu.BackColor = System.Drawing.Color.White;
            this.btnShowMenu.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnShowMenu.ForeColor = System.Drawing.Color.Black;
            this.btnShowMenu.Location = new System.Drawing.Point(461, 12);
            this.btnShowMenu.Name = "btnShowMenu";
            this.btnShowMenu.Size = new System.Drawing.Size(13, 29);
            this.btnShowMenu.TabIndex = 3;
            this.btnShowMenu.Text = ">";
            this.btnShowMenu.UseVisualStyleBackColor = false;
            this.btnShowMenu.Click += new System.EventHandler(this.btnShowMenu_Click);
            // 
            // btnGetPose
            // 
            this.btnGetPose.BackColor = System.Drawing.Color.White;
            this.btnGetPose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGetPose.ForeColor = System.Drawing.Color.Black;
            this.btnGetPose.Location = new System.Drawing.Point(239, 12);
            this.btnGetPose.Name = "btnGetPose";
            this.btnGetPose.Size = new System.Drawing.Size(111, 29);
            this.btnGetPose.TabIndex = 0;
            this.btnGetPose.Text = "Get NFO";
            this.btnGetPose.UseVisualStyleBackColor = false;
            this.btnGetPose.Click += new System.EventHandler(this.btnGetPose_Click);
            // 
            // gbAgents
            // 
            this.gbAgents.Controls.Add(this.lstSpawn);
            this.gbAgents.Controls.Add(this.lstAgents);
            this.gbAgents.Location = new System.Drawing.Point(7, 135);
            this.gbAgents.Name = "gbAgents";
            this.gbAgents.Size = new System.Drawing.Size(479, 118);
            this.gbAgents.TabIndex = 0;
            this.gbAgents.TabStop = false;
            this.gbAgents.Text = "Agents";
            // 
            // lstSpawn
            // 
            this.lstSpawn.CheckOnClick = true;
            this.lstSpawn.FormattingEnabled = true;
            this.lstSpawn.Location = new System.Drawing.Point(242, 18);
            this.lstSpawn.Name = "lstSpawn";
            this.lstSpawn.Size = new System.Drawing.Size(228, 94);
            this.lstSpawn.TabIndex = 1;
            this.lstSpawn.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstSpawn_ItemCheck);
            // 
            // lstAgents
            // 
            this.lstAgents.CheckOnClick = true;
            this.lstAgents.FormattingEnabled = true;
            this.lstAgents.Location = new System.Drawing.Point(6, 19);
            this.lstAgents.Name = "lstAgents";
            this.lstAgents.Size = new System.Drawing.Size(220, 94);
            this.lstAgents.TabIndex = 0;
            this.lstAgents.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstAgents_ItemCheck);
            // 
            // dlgOF
            // 
            this.dlgOF.FileName = "LocalConfig.ini";
            this.dlgOF.Filter = "INI File (*.ini) | *.ini";
            this.dlgOF.Title = "Open Configuration File";
            // 
            // cmsRun
            // 
            this.cmsRun.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmRun,
            this.itmRunTest});
            this.cmsRun.Name = "cmsRun";
            this.cmsRun.Size = new System.Drawing.Size(171, 48);
            // 
            // itmRun
            // 
            this.itmRun.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.itmRun.Name = "itmRun";
            this.itmRun.Size = new System.Drawing.Size(170, 22);
            this.itmRun.Text = "Run";
            this.itmRun.Click += new System.EventHandler(this.itmRun_Click);
            // 
            // itmRunTest
            // 
            this.itmRunTest.Name = "itmRunTest";
            this.itmRunTest.Size = new System.Drawing.Size(170, 22);
            this.itmRunTest.Text = "Run As Test Mode";
            this.itmRunTest.Click += new System.EventHandler(this.itmRun_Click);
            // 
            // checkBoxRobotProcess
            // 
            this.checkBoxRobotProcess.AutoSize = true;
            this.checkBoxRobotProcess.Location = new System.Drawing.Point(18, 259);
            this.checkBoxRobotProcess.Name = "checkBoxRobotProcess";
            this.checkBoxRobotProcess.Size = new System.Drawing.Size(133, 17);
            this.checkBoxRobotProcess.TabIndex = 3;
            this.checkBoxRobotProcess.Text = "Robot Internal process";
            this.checkBoxRobotProcess.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MRL.Loader.Properties.Resources.JoaoPessoaLogo;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(474, 117);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(490, 333);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkBoxRobotProcess);
            this.Controls.Add(this.gbAgents);
            this.Controls.Add(this.gbbtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MRL - Virtual Robot League";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.gbbtn.ResumeLayout(false);
            this.gbAgents.ResumeLayout(false);
            this.cmsRun.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox gbbtn;
        private System.Windows.Forms.GroupBox gbAgents;
        private System.Windows.Forms.CheckedListBox lstAgents;
        private System.Windows.Forms.OpenFileDialog dlgOF;
        private System.Windows.Forms.CheckedListBox lstSpawn;
        private System.Windows.Forms.Button btnGetPose;
        private System.Windows.Forms.ContextMenuStrip cmsRun;
        private System.Windows.Forms.ToolStripMenuItem itmRun;
        private System.Windows.Forms.ToolStripMenuItem itmRunTest;
        private System.Windows.Forms.Button btnShowMenu;
        private System.Windows.Forms.CheckBox checkBoxRobotProcess;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}