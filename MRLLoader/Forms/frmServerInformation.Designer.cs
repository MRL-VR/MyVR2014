namespace MRL.Components
{
    partial class frmServerInformation
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
            this.btnSendCommand = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtServerCMD = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtComStationParseCMD = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRobotParseCMD = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtGoalPointParseCMD = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnParseCommand = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtOriginalMessage = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtComStation = new System.Windows.Forms.TextBox();
            this.gbRobotPosition = new System.Windows.Forms.GroupBox();
            this.btnAssign = new System.Windows.Forms.Button();
            this.lstAssigned = new System.Windows.Forms.ListBox();
            this.cmbPositions = new System.Windows.Forms.ComboBox();
            this.cmbRobots = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.gbGoalPoint = new System.Windows.Forms.GroupBox();
            this.lstGoalPoints = new System.Windows.Forms.ListBox();
            this.btnAccpt = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gbRobotPosition.SuspendLayout();
            this.gbGoalPoint.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSendCommand
            // 
            this.btnSendCommand.Location = new System.Drawing.Point(6, 97);
            this.btnSendCommand.Name = "btnSendCommand";
            this.btnSendCommand.Size = new System.Drawing.Size(93, 26);
            this.btnSendCommand.TabIndex = 0;
            this.btnSendCommand.Text = "Get Command";
            this.btnSendCommand.UseVisualStyleBackColor = true;
            this.btnSendCommand.Click += new System.EventHandler(this.btnSendCommand_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server Command:";
            // 
            // txtServerCMD
            // 
            this.txtServerCMD.Location = new System.Drawing.Point(6, 32);
            this.txtServerCMD.Name = "txtServerCMD";
            this.txtServerCMD.Size = new System.Drawing.Size(175, 20);
            this.txtServerCMD.TabIndex = 2;
            this.txtServerCMD.Text = "GETSTARTPOSES";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(187, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "ComStation Parse command:";
            // 
            // txtComStationParseCMD
            // 
            this.txtComStationParseCMD.Location = new System.Drawing.Point(187, 32);
            this.txtComStationParseCMD.Name = "txtComStationParseCMD";
            this.txtComStationParseCMD.Size = new System.Drawing.Size(175, 20);
            this.txtComStationParseCMD.TabIndex = 2;
            this.txtComStationParseCMD.Text = "ComStation";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Robots:";
            // 
            // txtRobotParseCMD
            // 
            this.txtRobotParseCMD.Location = new System.Drawing.Point(6, 71);
            this.txtRobotParseCMD.Name = "txtRobotParseCMD";
            this.txtRobotParseCMD.Size = new System.Drawing.Size(175, 20);
            this.txtRobotParseCMD.TabIndex = 2;
            this.txtRobotParseCMD.Text = "Robot";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(187, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Goal Point Parse Command:";
            // 
            // txtGoalPointParseCMD
            // 
            this.txtGoalPointParseCMD.Location = new System.Drawing.Point(187, 71);
            this.txtGoalPointParseCMD.Name = "txtGoalPointParseCMD";
            this.txtGoalPointParseCMD.Size = new System.Drawing.Size(175, 20);
            this.txtGoalPointParseCMD.TabIndex = 2;
            this.txtGoalPointParseCMD.Text = "Target";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnParseCommand);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtGoalPointParseCMD);
            this.groupBox1.Controls.Add(this.btnSendCommand);
            this.groupBox1.Controls.Add(this.txtRobotParseCMD);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtComStationParseCMD);
            this.groupBox1.Controls.Add(this.txtServerCMD);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(369, 125);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server Info";
            // 
            // btnParseCommand
            // 
            this.btnParseCommand.Location = new System.Drawing.Point(105, 97);
            this.btnParseCommand.Name = "btnParseCommand";
            this.btnParseCommand.Size = new System.Drawing.Size(93, 26);
            this.btnParseCommand.TabIndex = 3;
            this.btnParseCommand.Text = "Parse Command";
            this.btnParseCommand.UseVisualStyleBackColor = true;
            this.btnParseCommand.Click += new System.EventHandler(this.btnParseCommand_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Robots Parse Command:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtOriginalMessage);
            this.groupBox2.Location = new System.Drawing.Point(12, 139);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(369, 94);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Original Message";
            // 
            // txtOriginalMessage
            // 
            this.txtOriginalMessage.Location = new System.Drawing.Point(9, 19);
            this.txtOriginalMessage.Multiline = true;
            this.txtOriginalMessage.Name = "txtOriginalMessage";
            this.txtOriginalMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOriginalMessage.Size = new System.Drawing.Size(353, 67);
            this.txtOriginalMessage.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtComStation);
            this.groupBox3.Location = new System.Drawing.Point(12, 236);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(369, 39);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Com Station Position";
            // 
            // txtComStation
            // 
            this.txtComStation.Location = new System.Drawing.Point(9, 13);
            this.txtComStation.Name = "txtComStation";
            this.txtComStation.Size = new System.Drawing.Size(229, 20);
            this.txtComStation.TabIndex = 2;
            // 
            // gbRobotPosition
            // 
            this.gbRobotPosition.Controls.Add(this.btnAssign);
            this.gbRobotPosition.Controls.Add(this.lstAssigned);
            this.gbRobotPosition.Controls.Add(this.cmbPositions);
            this.gbRobotPosition.Controls.Add(this.cmbRobots);
            this.gbRobotPosition.Controls.Add(this.label5);
            this.gbRobotPosition.Controls.Add(this.label3);
            this.gbRobotPosition.Location = new System.Drawing.Point(12, 277);
            this.gbRobotPosition.Name = "gbRobotPosition";
            this.gbRobotPosition.Size = new System.Drawing.Size(369, 144);
            this.gbRobotPosition.TabIndex = 3;
            this.gbRobotPosition.TabStop = false;
            this.gbRobotPosition.Text = "Robots Position";
            // 
            // btnAssign
            // 
            this.btnAssign.Location = new System.Drawing.Point(294, 16);
            this.btnAssign.Name = "btnAssign";
            this.btnAssign.Size = new System.Drawing.Size(68, 47);
            this.btnAssign.TabIndex = 2;
            this.btnAssign.Text = "Assign";
            this.btnAssign.UseVisualStyleBackColor = true;
            this.btnAssign.Click += new System.EventHandler(this.btnAssign_Click);
            // 
            // lstAssigned
            // 
            this.lstAssigned.FormattingEnabled = true;
            this.lstAssigned.Location = new System.Drawing.Point(6, 68);
            this.lstAssigned.Name = "lstAssigned";
            this.lstAssigned.Size = new System.Drawing.Size(356, 69);
            this.lstAssigned.TabIndex = 1;
            // 
            // cmbPositions
            // 
            this.cmbPositions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPositions.FormattingEnabled = true;
            this.cmbPositions.Location = new System.Drawing.Point(56, 42);
            this.cmbPositions.Name = "cmbPositions";
            this.cmbPositions.Size = new System.Drawing.Size(234, 21);
            this.cmbPositions.TabIndex = 0;
            // 
            // cmbRobots
            // 
            this.cmbRobots.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRobots.FormattingEnabled = true;
            this.cmbRobots.Location = new System.Drawing.Point(56, 16);
            this.cmbRobots.Name = "cmbRobots";
            this.cmbRobots.Size = new System.Drawing.Size(234, 21);
            this.cmbRobots.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Positions:";
            // 
            // gbGoalPoint
            // 
            this.gbGoalPoint.Controls.Add(this.lstGoalPoints);
            this.gbGoalPoint.Location = new System.Drawing.Point(12, 422);
            this.gbGoalPoint.Name = "gbGoalPoint";
            this.gbGoalPoint.Size = new System.Drawing.Size(369, 97);
            this.gbGoalPoint.TabIndex = 3;
            this.gbGoalPoint.TabStop = false;
            this.gbGoalPoint.Text = "Goal Points";
            // 
            // lstGoalPoints
            // 
            this.lstGoalPoints.FormattingEnabled = true;
            this.lstGoalPoints.Location = new System.Drawing.Point(9, 19);
            this.lstGoalPoints.Name = "lstGoalPoints";
            this.lstGoalPoints.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstGoalPoints.Size = new System.Drawing.Size(353, 69);
            this.lstGoalPoints.TabIndex = 1;
            // 
            // btnAccpt
            // 
            this.btnAccpt.Location = new System.Drawing.Point(308, 525);
            this.btnAccpt.Name = "btnAccpt";
            this.btnAccpt.Size = new System.Drawing.Size(73, 26);
            this.btnAccpt.TabIndex = 4;
            this.btnAccpt.Text = "Accept";
            this.btnAccpt.UseVisualStyleBackColor = true;
            this.btnAccpt.Click += new System.EventHandler(this.btnAccpt_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(229, 525);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 26);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmServerInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 556);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAccpt);
            this.Controls.Add(this.gbGoalPoint);
            this.Controls.Add(this.gbRobotPosition);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmServerInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmServerInformation";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbRobotPosition.ResumeLayout(false);
            this.gbRobotPosition.PerformLayout();
            this.gbGoalPoint.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSendCommand;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServerCMD;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtComStationParseCMD;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtRobotParseCMD;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtGoalPointParseCMD;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtOriginalMessage;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtComStation;
        private System.Windows.Forms.GroupBox gbRobotPosition;
        private System.Windows.Forms.GroupBox gbGoalPoint;
        private System.Windows.Forms.Button btnAssign;
        private System.Windows.Forms.ListBox lstAssigned;
        private System.Windows.Forms.ComboBox cmbPositions;
        private System.Windows.Forms.ComboBox cmbRobots;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lstGoalPoints;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnAccpt;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnParseCommand;
    }
}