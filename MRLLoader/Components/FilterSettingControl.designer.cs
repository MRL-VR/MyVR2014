namespace MRL.Components
{
    partial class FilterSettingControl
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
            this.ThickHaze = new System.Windows.Forms.RadioButton();
            this.ThinHaze = new System.Windows.Forms.RadioButton();
            this.BlackSmoke = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.BlakeSecond = new System.Windows.Forms.RadioButton();
            this.ThinSmoke = new System.Windows.Forms.RadioButton();
            this.All = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.brightTxt = new System.Windows.Forms.TextBox();
            this.ContrastTxt = new System.Windows.Forms.TextBox();
            this.GammaTxt = new System.Windows.Forms.TextBox();
            this.BrightTxt2 = new System.Windows.Forms.TextBox();
            this.gammaTrack = new System.Windows.Forms.TrackBar();
            this.ContrastTrack = new System.Windows.Forms.TrackBar();
            this.BrightTick = new System.Windows.Forms.TrackBar();
            this.BrightnessThin = new System.Windows.Forms.TrackBar();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gammaTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ContrastTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightTick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessThin)).BeginInit();
            this.SuspendLayout();
            // 
            // ThickHaze
            // 
            this.ThickHaze.AutoSize = true;
            this.ThickHaze.Location = new System.Drawing.Point(73, 43);
            this.ThickHaze.Name = "ThickHaze";
            this.ThickHaze.Size = new System.Drawing.Size(77, 17);
            this.ThickHaze.TabIndex = 4;
            this.ThickHaze.TabStop = true;
            this.ThickHaze.Text = "ThickHaze";
            this.ThickHaze.UseVisualStyleBackColor = true;
            this.ThickHaze.CheckedChanged += new System.EventHandler(this.ThickHaze_CheckedChanged);
            // 
            // ThinHaze
            // 
            this.ThinHaze.AutoSize = true;
            this.ThinHaze.Location = new System.Drawing.Point(179, 14);
            this.ThinHaze.Name = "ThinHaze";
            this.ThinHaze.Size = new System.Drawing.Size(71, 17);
            this.ThinHaze.TabIndex = 5;
            this.ThinHaze.TabStop = true;
            this.ThinHaze.Text = "ThinHaze";
            this.ThinHaze.UseVisualStyleBackColor = true;
            this.ThinHaze.CheckedChanged += new System.EventHandler(this.ThinHaze_CheckedChanged);
            // 
            // BlackSmoke
            // 
            this.BlackSmoke.AutoSize = true;
            this.BlackSmoke.Location = new System.Drawing.Point(73, 14);
            this.BlackSmoke.Name = "BlackSmoke";
            this.BlackSmoke.Size = new System.Drawing.Size(85, 17);
            this.BlackSmoke.TabIndex = 6;
            this.BlackSmoke.TabStop = true;
            this.BlackSmoke.Text = "BlackSmoke";
            this.BlackSmoke.UseVisualStyleBackColor = true;
            this.BlackSmoke.CheckedChanged += new System.EventHandler(this.BlackSmoke_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Maroon;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "first";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.BlakeSecond);
            this.groupBox1.Controls.Add(this.ThinSmoke);
            this.groupBox1.Controls.Add(this.All);
            this.groupBox1.Controls.Add(this.ThickHaze);
            this.groupBox1.Controls.Add(this.ThinHaze);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.BlackSmoke);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 94);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Methods";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Maroon;
            this.label4.Location = new System.Drawing.Point(10, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Second";
            // 
            // BlakeSecond
            // 
            this.BlakeSecond.AutoSize = true;
            this.BlakeSecond.Location = new System.Drawing.Point(179, 70);
            this.BlakeSecond.Name = "BlakeSecond";
            this.BlakeSecond.Size = new System.Drawing.Size(85, 17);
            this.BlakeSecond.TabIndex = 10;
            this.BlakeSecond.TabStop = true;
            this.BlakeSecond.Text = "BlackSmoke";
            this.BlakeSecond.UseVisualStyleBackColor = true;
            this.BlakeSecond.CheckedChanged += new System.EventHandler(this.BlakeSecond_CheckedChanged);
            // 
            // ThinSmoke
            // 
            this.ThinSmoke.AutoSize = true;
            this.ThinSmoke.Location = new System.Drawing.Point(73, 70);
            this.ThinSmoke.Name = "ThinSmoke";
            this.ThinSmoke.Size = new System.Drawing.Size(79, 17);
            this.ThinSmoke.TabIndex = 9;
            this.ThinSmoke.TabStop = true;
            this.ThinSmoke.Text = "ThinSmoke";
            this.ThinSmoke.UseVisualStyleBackColor = true;
            this.ThinSmoke.CheckedChanged += new System.EventHandler(this.ThinSmoke_CheckedChanged);
            // 
            // All
            // 
            this.All.AutoSize = true;
            this.All.Location = new System.Drawing.Point(179, 43);
            this.All.Name = "All";
            this.All.Size = new System.Drawing.Size(36, 17);
            this.All.TabIndex = 8;
            this.All.TabStop = true;
            this.All.Text = "All";
            this.All.UseVisualStyleBackColor = true;
            this.All.CheckedChanged += new System.EventHandler(this.All_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.brightTxt);
            this.groupBox2.Controls.Add(this.ContrastTxt);
            this.groupBox2.Controls.Add(this.GammaTxt);
            this.groupBox2.Controls.Add(this.BrightTxt2);
            this.groupBox2.Controls.Add(this.gammaTrack);
            this.groupBox2.Controls.Add(this.ContrastTrack);
            this.groupBox2.Controls.Add(this.BrightTick);
            this.groupBox2.Controls.Add(this.BrightnessThin);
            this.groupBox2.Location = new System.Drawing.Point(277, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(331, 69);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ChangeValue";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 139);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 13);
            this.label8.TabIndex = 22;
            this.label8.Tag = 3;
            this.label8.Text = "GammaThick";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 21;
            this.label7.Tag = 2;
            this.label7.Text = "Contrast Thin";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 20;
            this.label6.Tag = 1;
            this.label6.Text = "BrightnessThick";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 19;
            this.label5.Tag = 0;
            this.label5.Text = "BrightnessThin";
            // 
            // brightTxt
            // 
            this.brightTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.brightTxt.Location = new System.Drawing.Point(15, 76);
            this.brightTxt.Name = "brightTxt";
            this.brightTxt.Size = new System.Drawing.Size(37, 20);
            this.brightTxt.TabIndex = 18;
            this.brightTxt.Tag = 1;
            // 
            // ContrastTxt
            // 
            this.ContrastTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ContrastTxt.Location = new System.Drawing.Point(15, 116);
            this.ContrastTxt.Name = "ContrastTxt";
            this.ContrastTxt.Size = new System.Drawing.Size(37, 20);
            this.ContrastTxt.TabIndex = 17;
            this.ContrastTxt.Tag = 2;
            // 
            // GammaTxt
            // 
            this.GammaTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GammaTxt.Location = new System.Drawing.Point(15, 156);
            this.GammaTxt.Name = "GammaTxt";
            this.GammaTxt.Size = new System.Drawing.Size(37, 20);
            this.GammaTxt.TabIndex = 16;
            this.GammaTxt.Tag = 3;
            // 
            // BrightTxt2
            // 
            this.BrightTxt2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrightTxt2.Location = new System.Drawing.Point(15, 36);
            this.BrightTxt2.Name = "BrightTxt2";
            this.BrightTxt2.Size = new System.Drawing.Size(37, 20);
            this.BrightTxt2.TabIndex = 15;
            this.BrightTxt2.Tag = 0;
            // 
            // gammaTrack
            // 
            this.gammaTrack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gammaTrack.Location = new System.Drawing.Point(96, 139);
            this.gammaTrack.Maximum = 500;
            this.gammaTrack.Name = "gammaTrack";
            this.gammaTrack.Size = new System.Drawing.Size(230, 45);
            this.gammaTrack.TabIndex = 14;
            this.gammaTrack.Tag = 3;
            this.gammaTrack.Scroll += new System.EventHandler(this.Track_Scroll);
            // 
            // ContrastTrack
            // 
            this.ContrastTrack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ContrastTrack.Location = new System.Drawing.Point(96, 99);
            this.ContrastTrack.Maximum = 500;
            this.ContrastTrack.Name = "ContrastTrack";
            this.ContrastTrack.Size = new System.Drawing.Size(230, 45);
            this.ContrastTrack.TabIndex = 13;
            this.ContrastTrack.Tag = 2;
            this.ContrastTrack.Scroll += new System.EventHandler(this.Track_Scroll);
            // 
            // BrightTick
            // 
            this.BrightTick.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.BrightTick.Location = new System.Drawing.Point(95, 59);
            this.BrightTick.Maximum = 100;
            this.BrightTick.Minimum = -100;
            this.BrightTick.Name = "BrightTick";
            this.BrightTick.Size = new System.Drawing.Size(230, 45);
            this.BrightTick.TabIndex = 12;
            this.BrightTick.Tag = 1;
            this.BrightTick.TickFrequency = 5;
            this.BrightTick.Scroll += new System.EventHandler(this.Track_Scroll);
            // 
            // BrightnessThin
            // 
            this.BrightnessThin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.BrightnessThin.Location = new System.Drawing.Point(95, 19);
            this.BrightnessThin.Maximum = 100;
            this.BrightnessThin.Minimum = -100;
            this.BrightnessThin.Name = "BrightnessThin";
            this.BrightnessThin.Size = new System.Drawing.Size(230, 45);
            this.BrightnessThin.TabIndex = 11;
            this.BrightnessThin.Tag = 0;
            this.BrightnessThin.TickFrequency = 5;
            this.BrightnessThin.Scroll += new System.EventHandler(this.Track_Scroll);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(278, 81);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(65, 17);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "Enabled";
            this.checkBox1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // FilterSettingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FilterSettingControl";
            this.Size = new System.Drawing.Size(619, 106);
            this.Load += new System.EventHandler(this.FilterSettingForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gammaTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ContrastTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightTick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessThin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton ThickHaze;
        private System.Windows.Forms.RadioButton ThinHaze;
        private System.Windows.Forms.RadioButton BlackSmoke;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton BlakeSecond;
        private System.Windows.Forms.RadioButton ThinSmoke;
        private System.Windows.Forms.RadioButton All;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar gammaTrack;
        private System.Windows.Forms.TrackBar ContrastTrack;
        private System.Windows.Forms.TrackBar BrightTick;
        private System.Windows.Forms.TrackBar BrightnessThin;
        private System.Windows.Forms.TextBox brightTxt;
        private System.Windows.Forms.TextBox ContrastTxt;
        private System.Windows.Forms.TextBox GammaTxt;
        private System.Windows.Forms.TextBox BrightTxt2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

