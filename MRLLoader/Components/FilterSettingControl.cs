using System;
using System.Linq;
using System.Windows.Forms;
using MRL.ImageProcessor;

namespace MRL.Components
{
    public partial class FilterSettingControl : UserControl
    {
        public FilterSettingControl()
        {
            InitializeComponent();
            UpdateParams();
        }

        private void SetTrackBarEnabled(bool Brightness, bool Bright, bool Contrast, bool Gamma)
        {
            bool[] p = { Brightness, Bright, Contrast, Gamma };
            var items = groupBox2.Controls.OfType<Control>().Where(x => x.Tag != null && x.Tag is int).ToArray();
            for (int i = 0; i < 4; i++)
                items.Where(x => (int)x.Tag == i).ToList().ForEach(x => x.Visible = p[i]);
        }

        private void BlackSmoke_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender as RadioButton).Checked)
                return;
            SetTrackBarEnabled(false, false, false, false);
            ImageFiltering.Instance.BlackSmoke();
        }

        private void ThinHaze_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender as RadioButton).Checked)
                return;
            BrightnessThin.Value = -6;
            UpdateParams();
            SetTrackBarEnabled(true, false, false, false);
            ImageFiltering.Instance.ThinHaze();
        }

        private void ThickHaze_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender as RadioButton).Checked)
                return;
            BrightTick.Value = -6;
            UpdateParams();
            SetTrackBarEnabled(false, true, false, false);
            ImageFiltering.Instance.ThickHaze();
        }

        private void All_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender as RadioButton).Checked)
                return;
            SetTrackBarEnabled(false, false, false, false);
            ImageFiltering.Instance.All();
        }
        private void ThinSmoke_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender as RadioButton).Checked)
                return;
            SetTrackBarEnabled(false, false, true, false);
            ImageFiltering.Instance.ThinSmoke();
        }

        private void BlakeSecond_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender as RadioButton).Checked)
                return;
            SetTrackBarEnabled(false, false, false, true);
            ImageFiltering.Instance.BlakeSecond();
        }

        private void Track_Scroll(object sender, EventArgs e)
        {
            UpdateParams();
        }

        private void UpdateParams()
        {
            BrightTxt2.Text = BrightnessThin.Value.ToString();
            GammaTxt.Text = gammaTrack.Value.ToString();
            ContrastTxt.Text = ContrastTrack.Value.ToString();
            brightTxt.Text = BrightTick.Value.ToString();
            ImageFiltering.Instance.SetParameters(BrightnessThin.Value, BrightTick.Value, ContrastTrack.Value, gammaTrack.Value);
        }

        private void FilterSettingForm_Load(object sender, EventArgs e)
        {
            var items = groupBox2.Controls.OfType<Control>().Where(x => x.Tag != null && x.Tag is int).ToArray();
            foreach (var item in items)
            {
                if (item is TextBox)
                    item.Top = BrightTxt2.Top;
                else if (item is Label)
                    item.Top = label5.Top;
                else if (item is TrackBar)
                    item.Top = BrightnessThin.Top;
            }
            ThickHaze.Checked = true;
            checkBox1.Checked = false;
            checkBox1_CheckedChanged(null, null);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var b = checkBox1.Checked;
            ImageFiltering.Instance.Enabled = b;
            groupBox1.Enabled = groupBox2.Enabled = b;
        }
    }
}
