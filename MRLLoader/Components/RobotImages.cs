using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Components.Tools.Objects;
using MRL.Components.Tools.Shapes;

namespace MRL.Components
{
    public partial class RobotImages : UserControl
    {
        private PanoState pano;

        public RobotImages()
        {
            InitializeComponent();

            ProjectCommons.imageWidget_OnSelected = new ProjectCommons._imageWidget_OnSelected(setPanoImage);
            ProjectCommons.imageWidget_OnVictimUpdated = new ProjectCommons._imageWidget_OnVictim(updateVictim);
            ProjectCommons.imageWidget_OnVictimDeleted = new ProjectCommons._imageWidget_OnVictim(delVictimFromCombo);
        }

        public void updateVictim(VictimShape v)
        {
            addVictimToCombo(v);
        }

        private void addVictimToCombo(VictimShape v)
        {
            txtCanvasPose.Text = v.CanvasPose.ToString(2);
            txtRealPose.Text = v.RealPose.ToString(2);

            foreach (VictimShape vl in cmbVictims.Items)
            {
                if (vl.VictimInfo.ID == v.VictimInfo.ID)
                {
                    vl.VictimInfo = v.VictimInfo;
                    vl.RealPose = v.RealPose;
                    vl.CanvasPose = v.CanvasPose;
                    vl.Center = v.Center;

                    cmbVictims.SelectedItem = vl;
                    return;
                }
            }

            cmbVictims.Items.Add(v);
            cmbVictims.SelectedIndex = cmbVictims.Items.Count - 1;
        }
        private void delVictimFromCombo(VictimShape v)
        {
            if (cmbVictims.Items.Contains(v))
                cmbVictims.Items.Remove(v);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void setPanoImage(PanoState p)
        {
            lblNoImage.Visible = false;

            picPanoImage.Image = p.Image;
            lblNoImage.Visible = false;
            this.pano = p;
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbVictims.SelectedItem == null) return;
            VictimShape v = (VictimShape)cmbVictims.SelectedItem;

            string filename = ProjectCommons.currentResultPath + v.VictimInfo.Position + ".bmp";
            try
            {
                this.pano.Image.Save(filename);
                cmbVictims.Items.Remove(v);
                txtRealPose.Text = "";
                txtCanvasPose.Text = "";
            }
            catch
            {
                MessageBox.Show("Error in Saving file : \r\n" + filename);
            }
        }

        private void cmbVictims_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbVictims.SelectedItem == null) return;
            try
            {
                VictimShape v = (VictimShape)cmbVictims.SelectedItem;
                txtCanvasPose.Text = v.CanvasPose.ToString(2);
                txtRealPose.Text = v.RealPose.ToString(2);

                if (ProjectCommons.panoViewer_OnVictimSelected != null)
                    ProjectCommons.panoViewer_OnVictimSelected(v.VictimInfo.ID);
            }
            catch
            {
            }
        }
    }
}
