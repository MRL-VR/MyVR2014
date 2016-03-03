namespace MRL.Components
{
    partial class GeoreferenceImageViewer
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
            this.cms = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cms_AddVictim = new System.Windows.Forms.ToolStripMenuItem();
            this.cms.SuspendLayout();
            this.SuspendLayout();
            // 
            // cms
            // 
            this.cms.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cms_AddVictim});
            this.cms.Name = "cms";
            this.cms.Size = new System.Drawing.Size(134, 26);
            // 
            // cms_AddVictim
            // 
            this.cms_AddVictim.Name = "cms_AddVictim";
            this.cms_AddVictim.Size = new System.Drawing.Size(133, 22);
            this.cms_AddVictim.Text = "Add Victim";
            this.cms_AddVictim.Click += new System.EventHandler(this.cms_AddVictim_Click);
            // 
            // GeoreferenceImageViewer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ContextMenuStrip = this.cms;
            this.Name = "GeoreferenceImageViewer";
            this.Size = new System.Drawing.Size(657, 535);
            this.Load += new System.EventHandler(this.GeoreferenceImageViewer_Load);
            this.SizeChanged += new System.EventHandler(this.GeoreferenceImageViewer_SizeChanged);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GeoreferenceImageViewer_Click);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.GeoreferenceImageViewer_DoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GeoreferenceImageViewer_MouseDown);
            this.MouseEnter += new System.EventHandler(this.GeoreferenceImageViewer_MouseEnter);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GeoreferenceImageViewer_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GeoreferenceImageViewer_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GeoreferenceImageViewer_MouseWheel);
            this.Resize += new System.EventHandler(this.GeoreferenceImageViewer_Resize);
            this.cms.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.ContextMenuStrip cms;
        private System.Windows.Forms.ToolStripMenuItem cms_AddVictim;
    }
}
