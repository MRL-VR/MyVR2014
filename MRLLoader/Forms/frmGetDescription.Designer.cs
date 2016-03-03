namespace MRL.Components
{
    partial class frmGetDescription
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
            this.gbData = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lbl0 = new System.Windows.Forms.Label();
            this.gbBtn = new System.Windows.Forms.GroupBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbData.SuspendLayout();
            this.gbBtn.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbData
            // 
            this.gbData.Controls.Add(this.txtDescription);
            this.gbData.Controls.Add(this.lbl0);
            this.gbData.Location = new System.Drawing.Point(9, 6);
            this.gbData.Name = "gbData";
            this.gbData.Size = new System.Drawing.Size(310, 90);
            this.gbData.TabIndex = 0;
            this.gbData.TabStop = false;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(9, 32);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(295, 52);
            this.txtDescription.TabIndex = 1;
            // 
            // lbl0
            // 
            this.lbl0.AutoSize = true;
            this.lbl0.Location = new System.Drawing.Point(6, 16);
            this.lbl0.Name = "lbl0";
            this.lbl0.Size = new System.Drawing.Size(66, 13);
            this.lbl0.TabIndex = 0;
            this.lbl0.Text = "Description :";
            // 
            // gbBtn
            // 
            this.gbBtn.Controls.Add(this.btnOk);
            this.gbBtn.Controls.Add(this.btnCancel);
            this.gbBtn.Location = new System.Drawing.Point(118, 102);
            this.gbBtn.Name = "gbBtn";
            this.gbBtn.Size = new System.Drawing.Size(201, 61);
            this.gbBtn.TabIndex = 0;
            this.gbBtn.TabStop = false;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(6, 10);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(92, 45);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(104, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 45);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmGetDescription
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(327, 170);
            this.Controls.Add(this.gbBtn);
            this.Controls.Add(this.gbData);
            this.Name = "frmGetDescription";
            this.Text = "getDescription";
            this.gbData.ResumeLayout(false);
            this.gbData.PerformLayout();
            this.gbBtn.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbData;
        private System.Windows.Forms.GroupBox gbBtn;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lbl0;
        public System.Windows.Forms.TextBox txtDescription;
    }
}