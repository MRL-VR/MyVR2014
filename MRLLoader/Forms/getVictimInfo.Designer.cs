namespace MRL.Components
{
    partial class frmGetVictimInfo
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
            this.gbBtn = new System.Windows.Forms.GroupBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbData = new System.Windows.Forms.GroupBox();
            this.txtProbability = new System.Windows.Forms.TextBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtPosition3D = new System.Windows.Forms.TextBox();
            this.txtPosition2D = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl0 = new System.Windows.Forms.Label();
            this.gbBtn.SuspendLayout();
            this.gbData.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbBtn
            // 
            this.gbBtn.Controls.Add(this.btnOk);
            this.gbBtn.Controls.Add(this.btnCancel);
            this.gbBtn.Location = new System.Drawing.Point(125, 177);
            this.gbBtn.Name = "gbBtn";
            this.gbBtn.Size = new System.Drawing.Size(191, 56);
            this.gbBtn.TabIndex = 1;
            this.gbBtn.TabStop = false;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(6, 11);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(86, 40);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(98, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(86, 40);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // gbData
            // 
            this.gbData.Controls.Add(this.txtProbability);
            this.gbData.Controls.Add(this.txtStatus);
            this.gbData.Controls.Add(this.txtPosition3D);
            this.gbData.Controls.Add(this.txtPosition2D);
            this.gbData.Controls.Add(this.txtName);
            this.gbData.Controls.Add(this.txtID);
            this.gbData.Controls.Add(this.label5);
            this.gbData.Controls.Add(this.label4);
            this.gbData.Controls.Add(this.label3);
            this.gbData.Controls.Add(this.label2);
            this.gbData.Controls.Add(this.label1);
            this.gbData.Controls.Add(this.lbl0);
            this.gbData.Location = new System.Drawing.Point(6, 5);
            this.gbData.Name = "gbData";
            this.gbData.Size = new System.Drawing.Size(310, 171);
            this.gbData.TabIndex = 0;
            this.gbData.TabStop = false;
            // 
            // txtProbability
            // 
            this.txtProbability.Location = new System.Drawing.Point(69, 142);
            this.txtProbability.Name = "txtProbability";
            this.txtProbability.Size = new System.Drawing.Size(233, 20);
            this.txtProbability.TabIndex = 11;
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(69, 117);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(233, 20);
            this.txtStatus.TabIndex = 9;
            // 
            // txtPosition3D
            // 
            this.txtPosition3D.Location = new System.Drawing.Point(69, 91);
            this.txtPosition3D.Name = "txtPosition3D";
            this.txtPosition3D.ReadOnly = true;
            this.txtPosition3D.Size = new System.Drawing.Size(233, 20);
            this.txtPosition3D.TabIndex = 7;
            // 
            // txtPosition2D
            // 
            this.txtPosition2D.Location = new System.Drawing.Point(69, 65);
            this.txtPosition2D.Name = "txtPosition2D";
            this.txtPosition2D.Size = new System.Drawing.Size(233, 20);
            this.txtPosition2D.TabIndex = 5;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(69, 39);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(233, 20);
            this.txtName.TabIndex = 3;
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(69, 13);
            this.txtID.Name = "txtID";
            this.txtID.ReadOnly = true;
            this.txtID.Size = new System.Drawing.Size(233, 20);
            this.txtID.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Position3D:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Status:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Position2D:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ID:";
            // 
            // lbl0
            // 
            this.lbl0.AutoSize = true;
            this.lbl0.Location = new System.Drawing.Point(6, 145);
            this.lbl0.Name = "lbl0";
            this.lbl0.Size = new System.Drawing.Size(58, 13);
            this.lbl0.TabIndex = 10;
            this.lbl0.Text = "Probability:";
            // 
            // frmGetVictimInfo
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 239);
            this.Controls.Add(this.gbData);
            this.Controls.Add(this.gbBtn);
            this.Name = "frmGetVictimInfo";
            this.Text = "getVictimInfo";
            this.gbBtn.ResumeLayout(false);
            this.gbData.ResumeLayout(false);
            this.gbData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbBtn;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbData;
        public System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.Label lbl0;
        public System.Windows.Forms.TextBox txtStatus;
        public System.Windows.Forms.TextBox txtPosition2D;
        public System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtPosition3D;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox txtProbability;
    }
}