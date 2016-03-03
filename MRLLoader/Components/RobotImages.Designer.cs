using System.Windows.Forms;
namespace MRL.Components
{
    partial class RobotImages
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
            this.panelEx1 = new System.Windows.Forms.Panel();
            this.txtCanvasPose = new System.Windows.Forms.TextBox();
            this.txtRealPose = new System.Windows.Forms.TextBox();
            this.cmbVictims = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.labelX3 = new System.Windows.Forms.Label();
            this.labelX2 = new System.Windows.Forms.Label();
            this.labelX1 = new System.Windows.Forms.Label();
            this.lblNoImage = new System.Windows.Forms.Label();
            this.picPanoImage = new System.Windows.Forms.PictureBox();
            this.panelEx1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPanoImage)).BeginInit();
            this.SuspendLayout();
            // 
            // panelEx1
            // 
            this.panelEx1.Controls.Add(this.txtCanvasPose);
            this.panelEx1.Controls.Add(this.txtRealPose);
            this.panelEx1.Controls.Add(this.cmbVictims);
            this.panelEx1.Controls.Add(this.btnSave);
            this.panelEx1.Controls.Add(this.labelX3);
            this.panelEx1.Controls.Add(this.labelX2);
            this.panelEx1.Controls.Add(this.labelX1);
            this.panelEx1.Controls.Add(this.lblNoImage);
            this.panelEx1.Controls.Add(this.picPanoImage);
            this.panelEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEx1.Location = new System.Drawing.Point(0, 0);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(331, 104);
            this.panelEx1.TabIndex = 0;
            // 
            // txtCanvasPose
            // 
            this.txtCanvasPose.BackColor = System.Drawing.Color.White;
            this.txtCanvasPose.Location = new System.Drawing.Point(196, 48);
            this.txtCanvasPose.Name = "txtCanvasPose";
            this.txtCanvasPose.ReadOnly = true;
            this.txtCanvasPose.Size = new System.Drawing.Size(121, 20);
            this.txtCanvasPose.TabIndex = 5;
            // 
            // txtRealPose
            // 
            this.txtRealPose.BackColor = System.Drawing.Color.White;
            this.txtRealPose.Location = new System.Drawing.Point(196, 26);
            this.txtRealPose.Name = "txtRealPose";
            this.txtRealPose.ReadOnly = true;
            this.txtRealPose.Size = new System.Drawing.Size(121, 20);
            this.txtRealPose.TabIndex = 5;
            // 
            // cmbVictims
            // 
            this.cmbVictims.DisplayMember = "Text";
            this.cmbVictims.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbVictims.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVictims.FormattingEnabled = true;
            this.cmbVictims.ItemHeight = 14;
            this.cmbVictims.Location = new System.Drawing.Point(196, 4);
            this.cmbVictims.Name = "cmbVictims";
            this.cmbVictims.Size = new System.Drawing.Size(121, 20);
            this.cmbVictims.TabIndex = 1;
            this.cmbVictims.SelectedIndexChanged += new System.EventHandler(this.cmbVictims_SelectedIndexChanged);
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Location = new System.Drawing.Point(124, 72);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(193, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // labelX3
            // 
            this.labelX3.AutoSize = true;
            this.labelX3.Location = new System.Drawing.Point(122, 48);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(73, 13);
            this.labelX3.TabIndex = 2;
            this.labelX3.Text = "Canvas Pose:";
            // 
            // labelX2
            // 
            this.labelX2.AutoSize = true;
            this.labelX2.Location = new System.Drawing.Point(122, 26);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(59, 13);
            this.labelX2.TabIndex = 2;
            this.labelX2.Text = "Real Pose:";
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.Location = new System.Drawing.Point(122, 4);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(38, 13);
            this.labelX1.TabIndex = 2;
            this.labelX1.Text = "Victim:";
            // 
            // lblNoImage
            // 
            this.lblNoImage.AutoSize = true;
            this.lblNoImage.Location = new System.Drawing.Point(35, 44);
            this.lblNoImage.Name = "lblNoImage";
            this.lblNoImage.Size = new System.Drawing.Size(53, 13);
            this.lblNoImage.TabIndex = 4;
            this.lblNoImage.Text = "No Image";
            // 
            // picPanoImage
            // 
            this.picPanoImage.BackColor = System.Drawing.Color.Transparent;
            this.picPanoImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPanoImage.Location = new System.Drawing.Point(2, 4);
            this.picPanoImage.Name = "picPanoImage";
            this.picPanoImage.Size = new System.Drawing.Size(116, 93);
            this.picPanoImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPanoImage.TabIndex = 0;
            this.picPanoImage.TabStop = false;
            // 
            // RobotImages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Controls.Add(this.panelEx1);
            this.Name = "RobotImages";
            this.Size = new System.Drawing.Size(331, 104);
            this.panelEx1.ResumeLayout(false);
            this.panelEx1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPanoImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panelEx1;
        private System.Windows.Forms.PictureBox picPanoImage;
        private Label labelX1;
        private ComboBox cmbVictims;
        private Button btnSave;
        private System.Windows.Forms.Label lblNoImage;
        private TextBox txtCanvasPose;
        private TextBox txtRealPose;
        private Label labelX3;
        private Label labelX2;
    }
}
