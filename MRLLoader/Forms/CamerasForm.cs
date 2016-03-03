using System;
using System.Windows.Forms;

namespace MRL.Components
{
    public partial class CamerasForm : Form
    {
        public static CamerasForm Instance { get; private set; }

        static CamerasForm()
        {
            Instance = new CamerasForm();
        }

        private CamerasForm()
        {
            InitializeComponent();
            Controls.Add(RobotControlList.Instance);
        }

        private void CamerasForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }

        private void CamerasForm_Load(object sender, EventArgs e)
        {

        }
    }
}
