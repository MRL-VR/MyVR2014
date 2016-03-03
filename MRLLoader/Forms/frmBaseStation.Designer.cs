using MRL.Loader.Components;
namespace MRL.Components
{
    partial class frmBaseStation
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtTime = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblFPS = new System.Windows.Forms.Label();
            this.labelX7 = new System.Windows.Forms.Label();
            this.cmbImageSender_Robots = new System.Windows.Forms.ComboBox();
            this.labelX8 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtResultRemainingTime = new System.Windows.Forms.Label();
            this.btnResultMode = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblLastCommands = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.labelX6 = new System.Windows.Forms.Label();
            this.labelX5 = new System.Windows.Forms.Label();
            this.labelX4 = new System.Windows.Forms.Label();
            this.labelX3 = new System.Windows.Forms.Label();
            this.slider4 = new System.Windows.Forms.TrackBar();
            this.slider3 = new System.Windows.Forms.TrackBar();
            this.slider2 = new System.Windows.Forms.TrackBar();
            this.slider1 = new System.Windows.Forms.TrackBar();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.labelX2 = new System.Windows.Forms.Label();
            this.labelX1 = new System.Windows.Forms.Label();
            this.sld4WheeledRotation = new System.Windows.Forms.TrackBar();
            this.sld4WheeldSpeed = new System.Windows.Forms.TrackBar();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.SimulationTime = new System.Windows.Forms.Timer(this.components);
            this.DataTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.lstDebugger = new System.Windows.Forms.ListView();
            this.chConsoleType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chConsoleMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.geoViewer = new MRL.Components.GeoreferenceImageViewer();
            this.geoVeiwerProperties = new System.Windows.Forms.PropertyGrid();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slider4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider1)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sld4WheeledRotation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sld4WheeldSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1025, 155);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1017, 129);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main Controller";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.Image = global::MRL.Loader.Properties.Resources.mrlLogo;
            this.pictureBox1.Location = new System.Drawing.Point(806, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(203, 117);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtTime);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox2.Location = new System.Drawing.Point(141, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(117, 123);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "System Time";
            // 
            // txtTime
            // 
            this.txtTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTime.ForeColor = System.Drawing.Color.Black;
            this.txtTime.Location = new System.Drawing.Point(27, 22);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(104, 45);
            this.txtTime.TabIndex = 2;
            this.txtTime.Text = "20:00";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblFPS);
            this.groupBox1.Controls.Add(this.labelX7);
            this.groupBox1.Controls.Add(this.cmbImageSender_Robots);
            this.groupBox1.Controls.Add(this.labelX8);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(138, 123);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera Manager";
            // 
            // lblFPS
            // 
            this.lblFPS.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFPS.ForeColor = System.Drawing.Color.Black;
            this.lblFPS.Location = new System.Drawing.Point(60, 73);
            this.lblFPS.Name = "lblFPS";
            this.lblFPS.Size = new System.Drawing.Size(98, 24);
            this.lblFPS.TabIndex = 4;
            this.lblFPS.Text = "0";
            // 
            // labelX7
            // 
            this.labelX7.AutoSize = true;
            this.labelX7.Location = new System.Drawing.Point(6, 22);
            this.labelX7.Name = "labelX7";
            this.labelX7.Size = new System.Drawing.Size(76, 13);
            this.labelX7.TabIndex = 1;
            this.labelX7.Text = "Image Sender:";
            // 
            // cmbImageSender_Robots
            // 
            this.cmbImageSender_Robots.DisplayMember = "Name";
            this.cmbImageSender_Robots.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImageSender_Robots.FormattingEnabled = true;
            this.cmbImageSender_Robots.ItemHeight = 13;
            this.cmbImageSender_Robots.Location = new System.Drawing.Point(9, 44);
            this.cmbImageSender_Robots.Name = "cmbImageSender_Robots";
            this.cmbImageSender_Robots.Size = new System.Drawing.Size(103, 21);
            this.cmbImageSender_Robots.TabIndex = 2;
            // 
            // labelX8
            // 
            this.labelX8.AutoSize = true;
            this.labelX8.Location = new System.Drawing.Point(11, 75);
            this.labelX8.Name = "labelX8";
            this.labelX8.Size = new System.Drawing.Size(33, 13);
            this.labelX8.TabIndex = 3;
            this.labelX8.Text = "FPS :";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1017, 129);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Generate Result";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtResultRemainingTime);
            this.groupBox3.Controls.Add(this.btnResultMode);
            this.groupBox3.Location = new System.Drawing.Point(6, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 107);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Result Controler";
            // 
            // txtResultRemainingTime
            // 
            this.txtResultRemainingTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResultRemainingTime.ForeColor = System.Drawing.Color.Black;
            this.txtResultRemainingTime.Location = new System.Drawing.Point(41, 48);
            this.txtResultRemainingTime.Name = "txtResultRemainingTime";
            this.txtResultRemainingTime.Size = new System.Drawing.Size(104, 42);
            this.txtResultRemainingTime.TabIndex = 3;
            this.txtResultRemainingTime.Text = "15:00";
            // 
            // btnResultMode
            // 
            this.btnResultMode.Location = new System.Drawing.Point(14, 15);
            this.btnResultMode.Name = "btnResultMode";
            this.btnResultMode.Size = new System.Drawing.Size(161, 31);
            this.btnResultMode.TabIndex = 0;
            this.btnResultMode.Text = "Begin result mode";
            this.btnResultMode.Click += new System.EventHandler(this.btnResultMode_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox6);
            this.tabPage3.Controls.Add(this.groupBox5);
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1017, 129);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Drive Controller";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lblLastCommands);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox6.Location = new System.Drawing.Point(580, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(256, 123);
            this.groupBox6.TabIndex = 8;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "5 Last Commands";
            // 
            // lblLastCommands
            // 
            this.lblLastCommands.Location = new System.Drawing.Point(6, 20);
            this.lblLastCommands.Name = "lblLastCommands";
            this.lblLastCommands.Size = new System.Drawing.Size(188, 97);
            this.lblLastCommands.TabIndex = 1;
            this.lblLastCommands.Text = "Last Commands";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.labelX6);
            this.groupBox5.Controls.Add(this.labelX5);
            this.groupBox5.Controls.Add(this.labelX4);
            this.groupBox5.Controls.Add(this.labelX3);
            this.groupBox5.Controls.Add(this.slider4);
            this.groupBox5.Controls.Add(this.slider3);
            this.groupBox5.Controls.Add(this.slider2);
            this.groupBox5.Controls.Add(this.slider1);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox5.Location = new System.Drawing.Point(213, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(367, 123);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Kenaf Manager";
            // 
            // labelX6
            // 
            this.labelX6.Location = new System.Drawing.Point(98, 84);
            this.labelX6.Name = "labelX6";
            this.labelX6.Size = new System.Drawing.Size(67, 15);
            this.labelX6.TabIndex = 8;
            this.labelX6.Text = "Rear Right :";
            // 
            // labelX5
            // 
            this.labelX5.Location = new System.Drawing.Point(98, 61);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(58, 15);
            this.labelX5.TabIndex = 7;
            this.labelX5.Text = "Rear Left :";
            // 
            // labelX4
            // 
            this.labelX4.Location = new System.Drawing.Point(98, 44);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(67, 15);
            this.labelX4.TabIndex = 6;
            this.labelX4.Text = "Front Right :";
            // 
            // labelX3
            // 
            this.labelX3.Location = new System.Drawing.Point(98, 22);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(58, 15);
            this.labelX3.TabIndex = 5;
            this.labelX3.Text = "Front Left :";
            // 
            // slider4
            // 
            this.slider4.Location = new System.Drawing.Point(171, 39);
            this.slider4.Maximum = 180;
            this.slider4.Minimum = -180;
            this.slider4.Name = "slider4";
            this.slider4.Size = new System.Drawing.Size(127, 45);
            this.slider4.TabIndex = 4;
            this.slider4.Text = "100";
            this.slider4.Value = 20;
            // 
            // slider3
            // 
            this.slider3.Location = new System.Drawing.Point(171, 84);
            this.slider3.Maximum = 180;
            this.slider3.Minimum = -180;
            this.slider3.Name = "slider3";
            this.slider3.Size = new System.Drawing.Size(127, 45);
            this.slider3.TabIndex = 3;
            this.slider3.Text = "100";
            this.slider3.Value = 20;
            // 
            // slider2
            // 
            this.slider2.Location = new System.Drawing.Point(171, 61);
            this.slider2.Maximum = 180;
            this.slider2.Minimum = -180;
            this.slider2.Name = "slider2";
            this.slider2.Size = new System.Drawing.Size(127, 45);
            this.slider2.TabIndex = 2;
            this.slider2.Text = "100";
            this.slider2.Value = 20;
            // 
            // slider1
            // 
            this.slider1.Location = new System.Drawing.Point(171, 17);
            this.slider1.Maximum = 180;
            this.slider1.Minimum = -180;
            this.slider1.Name = "slider1";
            this.slider1.Size = new System.Drawing.Size(127, 45);
            this.slider1.TabIndex = 1;
            this.slider1.Text = "100";
            this.slider1.Value = 20;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.labelX2);
            this.groupBox4.Controls.Add(this.labelX1);
            this.groupBox4.Controls.Add(this.sld4WheeledRotation);
            this.groupBox4.Controls.Add(this.sld4WheeldSpeed);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(210, 123);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "4 Wheeled Manager";
            // 
            // labelX2
            // 
            this.labelX2.Location = new System.Drawing.Point(16, 23);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(46, 15);
            this.labelX2.TabIndex = 7;
            this.labelX2.Text = "Speed:";
            // 
            // labelX1
            // 
            this.labelX1.Location = new System.Drawing.Point(16, 51);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(46, 15);
            this.labelX1.TabIndex = 6;
            this.labelX1.Text = "Rotation:";
            // 
            // sld4WheeledRotation
            // 
            this.sld4WheeledRotation.Location = new System.Drawing.Point(68, 50);
            this.sld4WheeledRotation.Maximum = 100;
            this.sld4WheeledRotation.Name = "sld4WheeledRotation";
            this.sld4WheeledRotation.Size = new System.Drawing.Size(127, 45);
            this.sld4WheeledRotation.TabIndex = 5;
            this.sld4WheeledRotation.Text = "100";
            this.sld4WheeledRotation.Value = 20;
            // 
            // sld4WheeldSpeed
            // 
            this.sld4WheeldSpeed.Location = new System.Drawing.Point(68, 21);
            this.sld4WheeldSpeed.Maximum = 100;
            this.sld4WheeldSpeed.Name = "sld4WheeldSpeed";
            this.sld4WheeldSpeed.Size = new System.Drawing.Size(127, 45);
            this.sld4WheeldSpeed.TabIndex = 4;
            this.sld4WheeldSpeed.Text = "100";
            this.sld4WheeldSpeed.Value = 20;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 155);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox7);
            this.splitContainer1.Size = new System.Drawing.Size(1025, 556);
            this.splitContainer1.SplitterDistance = 375;
            this.splitContainer1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox9);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1025, 375);
            this.panel1.TabIndex = 1;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.splitContainer3);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox9.Location = new System.Drawing.Point(0, 0);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(1025, 375);
            this.groupBox9.TabIndex = 2;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Georeference Viewer";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 16);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.geoViewer);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.geoVeiwerProperties);
            this.splitContainer3.Size = new System.Drawing.Size(1019, 356);
            this.splitContainer3.SplitterDistance = 802;
            this.splitContainer3.TabIndex = 0;
            // 
            // SimulationTime
            // 
            this.SimulationTime.Enabled = true;
            this.SimulationTime.Interval = 1000;
            this.SimulationTime.Tick += new System.EventHandler(this.SimulationTime_Tick);
            // 
            // DataTimer
            // 
            this.DataTimer.Enabled = true;
            this.DataTimer.Tick += new System.EventHandler(this.DataTimer_Tick);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.lstDebugger);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(0, 0);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(1025, 177);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Debug";
            // 
            // lstDebugger
            // 
            this.lstDebugger.BackColor = System.Drawing.SystemColors.Window;
            this.lstDebugger.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chConsoleType,
            this.chConsoleMessage});
            this.lstDebugger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstDebugger.FullRowSelect = true;
            this.lstDebugger.GridLines = true;
            this.lstDebugger.HideSelection = false;
            this.lstDebugger.Location = new System.Drawing.Point(3, 16);
            this.lstDebugger.Name = "lstDebugger";
            this.lstDebugger.Size = new System.Drawing.Size(1019, 158);
            this.lstDebugger.TabIndex = 6;
            this.lstDebugger.UseCompatibleStateImageBehavior = false;
            this.lstDebugger.View = System.Windows.Forms.View.Details;
            // 
            // chConsoleType
            // 
            this.chConsoleType.Text = "!";
            this.chConsoleType.Width = 18;
            // 
            // chConsoleMessage
            // 
            this.chConsoleMessage.Text = "Description";
            this.chConsoleMessage.Width = 800;
            // 
            // geoViewer
            // 
            this.geoViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.geoViewer.Location = new System.Drawing.Point(0, 0);
            this.geoViewer.Name = "geoViewer";
            this.geoViewer.Size = new System.Drawing.Size(802, 356);
            this.geoViewer.TabIndex = 2;
            // 
            // geoVeiwerProperties
            // 
            this.geoVeiwerProperties.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.geoVeiwerProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.geoVeiwerProperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.geoVeiwerProperties.Location = new System.Drawing.Point(0, 0);
            this.geoVeiwerProperties.Name = "geoVeiwerProperties";
            this.geoVeiwerProperties.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.geoVeiwerProperties.SelectedObject = this.geoViewer;
            this.geoVeiwerProperties.Size = new System.Drawing.Size(213, 356);
            this.geoVeiwerProperties.TabIndex = 4;
            // 
            // frmBaseStation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1025, 711);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.tabControl1);
            this.KeyPreview = true;
            this.Name = "frmBaseStation";
            this.Text = "MRL Virtual Robot - Tiger Version 3.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmBaseStation_FormClosed);
            this.Load += new System.EventHandler(this.frmBaseStation_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmBaseStation_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmBaseStation_KeyUp);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slider4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider1)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sld4WheeledRotation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sld4WheeldSpeed)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label txtTime;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblFPS;
        private System.Windows.Forms.Label labelX7;
        private System.Windows.Forms.ComboBox cmbImageSender_Robots;
        private System.Windows.Forms.Label labelX8;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label txtResultRemainingTime;
        private System.Windows.Forms.Button btnResultMode;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label lblLastCommands;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label labelX6;
        private System.Windows.Forms.Label labelX5;
        private System.Windows.Forms.Label labelX4;
        private System.Windows.Forms.Label labelX3;
        private System.Windows.Forms.TrackBar slider4;
        private System.Windows.Forms.TrackBar slider3;
        private System.Windows.Forms.TrackBar slider2;
        private System.Windows.Forms.TrackBar slider1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label labelX2;
        private System.Windows.Forms.Label labelX1;
        private System.Windows.Forms.TrackBar sld4WheeledRotation;
        private System.Windows.Forms.TrackBar sld4WheeldSpeed;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Timer SimulationTime;
        private System.Windows.Forms.Timer DataTimer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private GeoreferenceImageViewer geoViewer;
        private System.Windows.Forms.PropertyGrid geoVeiwerProperties;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.ListView lstDebugger;
        private System.Windows.Forms.ColumnHeader chConsoleType;
        private System.Windows.Forms.ColumnHeader chConsoleMessage;
    }
}