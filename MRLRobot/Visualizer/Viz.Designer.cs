namespace MRLRobot
{
    partial class Viz
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Viz));
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.SlimControl1 = new VisualizerLibrary.NewSlimControl();
            this.RecordBtn = new System.Windows.Forms.Button();
            this.sliderLevel = new System.Windows.Forms.TrackBar();
            this.recordLbl = new System.Windows.Forms.Label();
            this.OpenBtn = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sliderLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "-";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(162, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "-";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(481, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "-";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(328, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(11, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "-";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.ForeColor = System.Drawing.Color.Yellow;
            this.checkBox1.Location = new System.Drawing.Point(633, 12);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(74, 17);
            this.checkBox1.TabIndex = 16;
            this.checkBox1.Text = "ShowMap";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.flowLayoutPanel1.Controls.Add(this.RecordBtn);
            this.flowLayoutPanel1.Controls.Add(this.OpenBtn);
            this.flowLayoutPanel1.Controls.Add(this.sliderLevel);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 536);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(728, 46);
            this.flowLayoutPanel1.TabIndex = 17;
            // 
            // SlimControl1
            // 
            this.SlimControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SlimControl1.Location = new System.Drawing.Point(0, 40);
            this.SlimControl1.Name = "SlimControl1";
            this.SlimControl1.Size = new System.Drawing.Size(728, 507);
            this.SlimControl1.TabIndex = 15;
            this.SlimControl1.Transform = ((System.Nullable<SlimDX.Matrix3x2>)(resources.GetObject("SlimControl1.Transform")));
            this.SlimControl1.WheelActive = true;
            // 
            // RecordBtn
            // 
            this.RecordBtn.ForeColor = System.Drawing.Color.Red;
            this.RecordBtn.Location = new System.Drawing.Point(3, 3);
            this.RecordBtn.Name = "RecordBtn";
            this.RecordBtn.Size = new System.Drawing.Size(55, 35);
            this.RecordBtn.TabIndex = 0;
            this.RecordBtn.Text = "Record";
            this.RecordBtn.UseVisualStyleBackColor = true;
            this.RecordBtn.Click += new System.EventHandler(this.RecordBtn_Click);
            // 
            // sliderLevel
            // 
            this.sliderLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderLevel.Location = new System.Drawing.Point(125, 3);
            this.sliderLevel.Maximum = 1000;
            this.sliderLevel.Name = "sliderLevel";
            this.sliderLevel.Size = new System.Drawing.Size(588, 45);
            this.sliderLevel.TabIndex = 1;
            // 
            // recordLbl
            // 
            this.recordLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.recordLbl.AutoSize = true;
            this.recordLbl.BackColor = System.Drawing.Color.Red;
            this.recordLbl.ForeColor = System.Drawing.Color.White;
            this.recordLbl.Location = new System.Drawing.Point(676, 50);
            this.recordLbl.Name = "recordLbl";
            this.recordLbl.Size = new System.Drawing.Size(37, 13);
            this.recordLbl.TabIndex = 2;
            this.recordLbl.Text = "record";
            // 
            // OpenBtn
            // 
            this.OpenBtn.ForeColor = System.Drawing.Color.Green;
            this.OpenBtn.Location = new System.Drawing.Point(64, 3);
            this.OpenBtn.Name = "OpenBtn";
            this.OpenBtn.Size = new System.Drawing.Size(55, 35);
            this.OpenBtn.TabIndex = 2;
            this.OpenBtn.Text = "Open";
            this.OpenBtn.UseVisualStyleBackColor = true;
            this.OpenBtn.Click += new System.EventHandler(this.OpenBtn_Click);
            // 
            // Viz
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(725, 579);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.recordLbl);
            this.Controls.Add(this.SlimControl1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Name = "Viz";
            this.Text = "Viz";
            this.Load += new System.EventHandler(this.Viz_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sliderLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private VisualizerLibrary.NewSlimControl SlimControl1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button RecordBtn;
        private System.Windows.Forms.TrackBar sliderLevel;
        private System.Windows.Forms.Label recordLbl;
        private System.Windows.Forms.Button OpenBtn;
    }
}