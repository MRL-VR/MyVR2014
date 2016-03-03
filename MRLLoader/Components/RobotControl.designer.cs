using System.Windows.Forms;
namespace MRL.Components
{
    partial class RobotControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageTimer = new System.Windows.Forms.Timer(this.components);
            this.lblDisconnect = new System.Windows.Forms.Label();
            this.gbArm = new System.Windows.Forms.Panel();
            this.chkArm_rr = new System.Windows.Forms.CheckBox();
            this.chkArm_rl = new System.Windows.Forms.CheckBox();
            this.chkArm_fr = new System.Windows.Forms.CheckBox();
            this.labelX4 = new System.Windows.Forms.Label();
            this.labelX3 = new System.Windows.Forms.Label();
            this.labelX2 = new System.Windows.Forms.Label();
            this.labelX1 = new System.Windows.Forms.Label();
            this.chkArm_fl = new System.Windows.Forms.CheckBox();
            this.prgBattery = new System.Windows.Forms.ProgressBar();
            this.lblBattery = new System.Windows.Forms.Label();
            this.chkRobotLight = new System.Windows.Forms.CheckBox();
            this.gbMain = new System.Windows.Forms.Label();
            this.signalImg = new System.Windows.Forms.PictureBox();
            this.picCameraImage = new System.Windows.Forms.PictureBox();
            this.lblPercent = new System.Windows.Forms.Label();
            this.routeLbl = new System.Windows.Forms.Label();
            this.signalPercentageLbl = new System.Windows.Forms.Label();
            this.gbArm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.signalImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCameraImage)).BeginInit();
            this.SuspendLayout();
            // 
            // imageTimer
            // 
            this.imageTimer.Enabled = true;
            // 
            // lblDisconnect
            // 
            this.lblDisconnect.AutoSize = true;
            this.lblDisconnect.ForeColor = System.Drawing.Color.Red;
            this.lblDisconnect.Location = new System.Drawing.Point(130, 289);
            this.lblDisconnect.Name = "lblDisconnect";
            this.lblDisconnect.Size = new System.Drawing.Size(73, 13);
            this.lblDisconnect.TabIndex = 12;
            this.lblDisconnect.Text = "Disconnected";
            this.lblDisconnect.Visible = false;
            // 
            // gbArm
            // 
            this.gbArm.Controls.Add(this.chkArm_rr);
            this.gbArm.Controls.Add(this.chkArm_rl);
            this.gbArm.Controls.Add(this.chkArm_fr);
            this.gbArm.Controls.Add(this.labelX4);
            this.gbArm.Controls.Add(this.labelX3);
            this.gbArm.Controls.Add(this.labelX2);
            this.gbArm.Controls.Add(this.labelX1);
            this.gbArm.Controls.Add(this.chkArm_fl);
            this.gbArm.Location = new System.Drawing.Point(44, 346);
            this.gbArm.Name = "gbArm";
            this.gbArm.Size = new System.Drawing.Size(237, 34);
            this.gbArm.TabIndex = 11;
            this.gbArm.Visible = false;
            // 
            // chkArm_rr
            // 
            this.chkArm_rr.ForeColor = System.Drawing.Color.Maroon;
            this.chkArm_rr.Location = new System.Drawing.Point(192, 8);
            this.chkArm_rr.Name = "chkArm_rr";
            this.chkArm_rr.Size = new System.Drawing.Size(39, 12);
            this.chkArm_rr.TabIndex = 6;
            this.chkArm_rr.Text = "0";
            // 
            // chkArm_rl
            // 
            this.chkArm_rl.ForeColor = System.Drawing.Color.Maroon;
            this.chkArm_rl.Location = new System.Drawing.Point(133, 8);
            this.chkArm_rl.Name = "chkArm_rl";
            this.chkArm_rl.Size = new System.Drawing.Size(39, 12);
            this.chkArm_rl.TabIndex = 5;
            this.chkArm_rl.Text = "0";
            // 
            // chkArm_fr
            // 
            this.chkArm_fr.ForeColor = System.Drawing.Color.Maroon;
            this.chkArm_fr.Location = new System.Drawing.Point(79, 8);
            this.chkArm_fr.Name = "chkArm_fr";
            this.chkArm_fr.Size = new System.Drawing.Size(39, 12);
            this.chkArm_fr.TabIndex = 5;
            this.chkArm_fr.Text = "0";
            // 
            // labelX4
            // 
            this.labelX4.Location = new System.Drawing.Point(176, 8);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(18, 14);
            this.labelX4.TabIndex = 4;
            this.labelX4.Text = "RR";
            // 
            // labelX3
            // 
            this.labelX3.Location = new System.Drawing.Point(121, 8);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(18, 14);
            this.labelX3.TabIndex = 3;
            this.labelX3.Text = "RL";
            // 
            // labelX2
            // 
            this.labelX2.Location = new System.Drawing.Point(61, 8);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(18, 14);
            this.labelX2.TabIndex = 2;
            this.labelX2.Text = "FR";
            // 
            // labelX1
            // 
            this.labelX1.Location = new System.Drawing.Point(3, 8);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(18, 14);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "FL";
            // 
            // chkArm_fl
            // 
            this.chkArm_fl.ForeColor = System.Drawing.Color.Maroon;
            this.chkArm_fl.Location = new System.Drawing.Point(19, 8);
            this.chkArm_fl.Name = "chkArm_fl";
            this.chkArm_fl.Size = new System.Drawing.Size(39, 12);
            this.chkArm_fl.TabIndex = 0;
            this.chkArm_fl.Text = "0";
            // 
            // prgBattery
            // 
            this.prgBattery.BackColor = System.Drawing.Color.White;
            this.prgBattery.Location = new System.Drawing.Point(71, 317);
            this.prgBattery.Name = "prgBattery";
            this.prgBattery.Size = new System.Drawing.Size(211, 23);
            this.prgBattery.TabIndex = 7;
            this.prgBattery.Text = "progressBarX1";
            // 
            // lblBattery
            // 
            this.lblBattery.BackColor = System.Drawing.Color.Transparent;
            this.lblBattery.Location = new System.Drawing.Point(21, 321);
            this.lblBattery.Name = "lblBattery";
            this.lblBattery.Size = new System.Drawing.Size(44, 19);
            this.lblBattery.TabIndex = 9;
            this.lblBattery.Text = "Battery:";
            // 
            // chkRobotLight
            // 
            this.chkRobotLight.Location = new System.Drawing.Point(24, 285);
            this.chkRobotLight.Name = "chkRobotLight";
            this.chkRobotLight.Size = new System.Drawing.Size(100, 23);
            this.chkRobotLight.TabIndex = 10;
            this.chkRobotLight.Text = "Robot Light";
            // 
            // gbMain
            // 
            this.gbMain.AutoSize = true;
            this.gbMain.Location = new System.Drawing.Point(12, 9);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(0, 13);
            this.gbMain.TabIndex = 14;
            // 
            // signalImg
            // 
            this.signalImg.Image = global::MRL.Loader.Properties.Resources.Antena_100;
            this.signalImg.Location = new System.Drawing.Point(284, 86);
            this.signalImg.Name = "signalImg";
            this.signalImg.Size = new System.Drawing.Size(68, 165);
            this.signalImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.signalImg.TabIndex = 15;
            this.signalImg.TabStop = false;
            // 
            // picCameraImage
            // 
            this.picCameraImage.BackColor = System.Drawing.Color.Transparent;
            this.picCameraImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picCameraImage.Location = new System.Drawing.Point(15, 37);
            this.picCameraImage.Name = "picCameraImage";
            this.picCameraImage.Size = new System.Drawing.Size(267, 240);
            this.picCameraImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picCameraImage.TabIndex = 13;
            this.picCameraImage.TabStop = false;
            this.picCameraImage.DoubleClick += new System.EventHandler(this.Control_DoubleClick);
            // 
            // lblPercent
            // 
            this.lblPercent.BackColor = System.Drawing.Color.Transparent;
            this.lblPercent.Location = new System.Drawing.Point(288, 321);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(44, 19);
            this.lblPercent.TabIndex = 16;
            this.lblPercent.Text = "%";
            // 
            // routeLbl
            // 
            this.routeLbl.AutoSize = true;
            this.routeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.routeLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.routeLbl.Location = new System.Drawing.Point(254, 289);
            this.routeLbl.Name = "routeLbl";
            this.routeLbl.Size = new System.Drawing.Size(98, 15);
            this.routeLbl.TabIndex = 17;
            this.routeLbl.Text = "Routed Signal";
            // 
            // signalPercentageLbl
            // 
            this.signalPercentageLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.signalPercentageLbl.Location = new System.Drawing.Point(288, 254);
            this.signalPercentageLbl.Name = "signalPercentageLbl";
            this.signalPercentageLbl.Size = new System.Drawing.Size(64, 24);
            this.signalPercentageLbl.TabIndex = 18;
            this.signalPercentageLbl.Text = "0";
            this.signalPercentageLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RobotControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.signalPercentageLbl);
            this.Controls.Add(this.routeLbl);
            this.Controls.Add(this.lblPercent);
            this.Controls.Add(this.signalImg);
            this.Controls.Add(this.gbMain);
            this.Controls.Add(this.picCameraImage);
            this.Controls.Add(this.lblDisconnect);
            this.Controls.Add(this.gbArm);
            this.Controls.Add(this.prgBattery);
            this.Controls.Add(this.lblBattery);
            this.Controls.Add(this.chkRobotLight);
            this.Name = "RobotControl";
            this.Size = new System.Drawing.Size(355, 395);
            this.DoubleClick += new System.EventHandler(this.Control_DoubleClick);
            this.gbArm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.signalImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCameraImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Timer imageTimer;
        private PictureBox picCameraImage;
        private Label lblDisconnect;
        private Panel gbArm;
        private CheckBox chkArm_rr;
        private CheckBox chkArm_rl;
        private CheckBox chkArm_fr;
        private Label labelX4;
        private Label labelX3;
        private Label labelX2;
        private Label labelX1;
        private CheckBox chkArm_fl;
        private ProgressBar prgBattery;
        private Label lblBattery;
        private CheckBox chkRobotLight;
        private Label gbMain;
        private PictureBox signalImg;
        private Label lblPercent;
        private Label routeLbl;
        private Label signalPercentageLbl;

    }
}
