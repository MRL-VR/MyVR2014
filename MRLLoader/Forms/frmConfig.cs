using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MRL.CustomMath;
using MRL.Utils;
using MRL.IDE.Robot;

namespace MRL.Components
{
	//test
    public partial class frmConfig : Form
    {
        public frmConfig()
        {
            InitializeComponent();
        }

        #region Variables

        public string ConfigFileName { get; set; }

        private USARConfig cfg;
        private int Info_RobotType_index = -1;
        private int RobotList_Index = -1;
        private bool robotInfoSaveSetting = true;
        private bool isApplyForAllItem = false;
        private FontDialog widgetFontDialog = new FontDialog();

        private int grv_cmbSelectedIndex = -1; //georeferenceViewer combobox Selected index

        Bitmap sampleBackGround = null;
        #endregion

        #region All Tab unity

        private void loadConfig()
        {
            //string address = (Application.StartupPath);
            //int len = address.IndexOf("MRLLoader");
            //if (len == -1) return;

            //address = address.Substring(0, len) + "Config\\";
            //address += "LocalConfig.ini";

            cfg = new USARConfig(ConfigFileName);
        }
        private void setSetting()
        {
            lstRobotList.Items.Clear();

            txtSimIP.Text = cfg.simHost;
            txtSimPort.Text = cfg.simPort.ToString();

            txtImageServerIP.Text = cfg.videoHost;
            txtImageServerPort.Text = cfg.videoPort.ToString();

            txtWSSIP.Text = cfg.wssHost;
            txtWSSPort.Text = cfg.wssPort.ToString();

            /* Robot Information */
            cmbInfo_RobotType.SelectedIndex = 0;

            /* Robot Management */
            int count = cfg.botInfo.Count;
            for (int i = 0; i < count; i++)
            {
                lstRobotList.Items.Add(cfg.botInfo[i].Type + " - " + cfg.botInfo[i].Name);
            }

            if (lstRobotList.Items.Count > 0)
            {
                lstRobotList.SelectedIndex = 0;
            }

            /* Station Management */
            txtStationType.Text = cfg.mapperInfo.Type;
            txtStationName.Text = cfg.mapperInfo.Name;
            txtStationPosition.Text = cfg.mapperInfo.PositionString; //cfg.mapperInfo.Position3D.X + "," + cfg.mapperInfo.Position3D.Y + "," + cfg.mapperInfo.Rotation3D.Z;
            txtStationRotation.Text = cfg.mapperInfo.RotationString;
            txtStationBotIP.Text = cfg.mapperInfo.RobotIP;
            txtStationBotPort.Text = cfg.mapperInfo.RobotPort.ToString();

            // Other
            chkUserLog.Checked = bool.Parse(cfg.getValue("UserLog").ToString());
            chkAutoSave.Checked = bool.Parse(cfg.getValue("AutoSave").ToString());
            txtTraceUserStep.Text = cfg.getValue("Trace_Rec_Steps").ToString();

            txtUTSwitchTime.Text = cfg.getValue("UTSwitchTime").ToString();

            txtVictimRange.Text = cfg.getValue("Victim_Range").ToString();
            txtSafeStrength.Text = cfg.getValue("Link_Safe_Strength").ToString();
            txtDangerStrength.Text = cfg.getValue("Link_Danger_Strength").ToString();
            txtMapResolution.Text = cfg.getValue("Map_Resolution").ToString();
            txtMapDownSample.Text = cfg.getValue("Map_DownSample").ToString();
            txtMapWallThickness.Text = cfg.getValue("Map_WallThickness").ToString();

            txtTileX.Text = cfg.CAM_TILE_X.ToString();
            txtTileY.Text = cfg.CAM_TILE_Y.ToString();
            txtMaxCamW.Text = cfg.MAX_CAM_WIDTH.ToString();
            txtMaxCamH.Text = cfg.MAX_CAM_HEIGHT.ToString();
            txtSubCamW.Text = cfg.SUB_CAM_WIDTH.ToString();
            txtSubCamH.Text = cfg.SUB_CAM_HEIGHT.ToString();

            int imageTransferType = cfg.IMAGE_TRANSFER_TYPE;
            int obstacleOvoidanceStatus = cfg.OBSTACLE_AVOIDANCE_STATUS;

            if (imageTransferType == 1)
                directRadioButton.Checked = true;
            else
                networkRadioButton.Checked = true;

            if (obstacleOvoidanceStatus == 1)
                oaOnRadioBtn.Checked = true;
            else
                oaOffRadioBtn.Checked = true;

            cmbControlMode.SelectedIndex = int.Parse(cfg.getValue("controlMode").ToString()) - 1;
            cmbMode.SelectedIndex = int.Parse(cfg.getValue("mode").ToString()) - 1;
        }
        private void SaveSetting()
        {
            /* Server Information */
            cfg.setValue("SimHost", txtSimIP.Text);
            cfg.setValue("SimPort", txtSimPort.Text);

            cfg.setValue("VideoHost", txtImageServerIP.Text);
            cfg.setValue("VideoPort", txtImageServerPort.Text);

            cfg.setValue("WSSHost", txtWSSIP.Text);
            cfg.setValue("WSSPort", txtWSSPort.Text);

            cfg.setValue("CamTileX", txtTileX.Text);
            cfg.setValue("CamTileY", txtTileY.Text);

            cfg.setValue("MaxCamWidth", txtMaxCamW.Text);
            cfg.setValue("MaxCamHeight", txtMaxCamH.Text);

            cfg.setValue("SubCamWidth", txtSubCamW.Text);
            cfg.setValue("SubCamHeight", txtSubCamH.Text);

            cfg.setValue("ImageTransferType", directRadioButton.Checked ? "1" : "2");
            cfg.setValue("ObstacleAvoidanceStatus", oaOnRadioBtn.Checked ? "1" : "2");


            /* Robot Information */
            cmbInfo_RobotType.SelectedIndex = 0;

            /* Robot Management */
            lstRobotList_SelectedIndexChanged(new object(), new EventArgs());


            /* Station Management */
            cfg.mapperInfo.Type = txtStationType.Text;
            cfg.mapperInfo.Name = txtStationName.Text;
            cfg.mapperInfo.Position3D = new Vector3(txtStationPosition.Text);
            cfg.mapperInfo.RotationString = txtStationRotation.Text;
            cfg.mapperInfo.RobotIP = txtStationBotIP.Text;
            cfg.mapperInfo.RobotPort = int.Parse(txtStationBotPort.Text.Trim()); ;

            // Other

            cfg.setValue("UserLog", chkUserLog.Checked.ToString());
            cfg.setValue("AutoSave", chkAutoSave.Checked.ToString());
            cfg.setValue("Trace_Rec_Steps", txtTraceUserStep.Text.ToString());

            cfg.setValue("UTSwitchTime", txtUTSwitchTime.Text.ToString());

            cfg.setValue("Victim_Range", txtVictimRange.Text.Trim());
            cfg.setValue("Link_Safe_Strength", txtSafeStrength.Text.Trim());
            cfg.setValue("Link_Danger_Strength", txtDangerStrength.Text.Trim());
            cfg.setValue("Map_Resolution", txtMapResolution.Text.Trim());
            cfg.setValue("Map_DownSample", txtMapDownSample.Text.Trim());
            cfg.setValue("Map_WallThickness", txtMapWallThickness.Text.Trim());

            cfg.setValue("controlMode", (cmbControlMode.SelectedIndex + 1).ToString());
            cfg.setValue("mode", (cmbMode.SelectedIndex + 1).ToString());

            //Georeference Viewer
            if (cmbWidget.SelectedIndex < cmbWidget.Items.Count - 1)
                cmbWidget.SelectedIndex += 1;
            else
                cmbWidget.SelectedIndex -= 1;
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            cmbRobotType.Items.AddRange(ValidatedRobot.GetRobotTypes().Select(x => x.Name).ToArray());
            loadConfig();
            setSetting();
            cmbWidget.SelectedIndex = 1;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
        private void btnSaveClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            SaveSetting();
            cfg.Save();
            Close();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSetting();
            cfg.Save();
        }

        #endregion

        #region Robot Information

        private void cmbInfo_RobotType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblError.Visible = false;
            string prefix;

            if (Info_RobotType_index != -1 && RobotList_Index < cfg.botInfo.Count)
            {
                prefix = cmbInfo_RobotType.Items[Info_RobotType_index].ToString() + "_"; ;
                cfg.setValue(prefix + txtSafeRange.Tag, txtSafeRange.Text);
                cfg.setValue(prefix + txtDefaultFOV.Tag, txtDefaultFOV.Text);
                cfg.setValue(prefix + txtMaxSpeed.Tag, txtMaxSpeed.Text);
                cfg.setValue(prefix + txtMinSpeed.Tag, txtMinSpeed.Text);
                cfg.setValue(prefix + txtHighSpeed.Tag, txtHighSpeed.Text);
                cfg.setValue(prefix + txtLowSpeed.Tag, txtLowSpeed.Text);
                cfg.setValue(prefix + txtMaxTele.Tag, txtMaxTele.Text);
                cfg.setValue(prefix + txtRingSize.Tag, txtRingSize.Text);
                cfg.setValue(prefix + txtAdjTurn.Tag, txtAdjTurn.Text);
                cfg.setValue(prefix + txtAdjTran.Tag, txtAdjTran.Text);
                cfg.setValue(prefix + txtSpeedScale.Tag, txtSpeedScale.Text);
                cfg.setValue(prefix + txtSafeRoll.Tag, txtSafeRoll.Text);
                cfg.setValue(prefix + txtSafePich.Tag, txtSafePich.Text);
                cfg.setValue(prefix + txtPosRFIDVictSensor.Tag, txtPosRFIDVictSensor.Text);
                cfg.setValue(prefix + txtPosRFIDSensor.Tag, txtPosRFIDSensor.Text);
                cfg.setValue(prefix + txtPosRangeScanner.Tag, txtPosRangeScanner.Text);
            }
            prefix = cmbInfo_RobotType.Text.Trim() + "_"; ;

            try
            {
                txtSafeRange.Text = cfg.getValue(prefix + txtSafeRange.Tag).ToString();
                txtDefaultFOV.Text = cfg.getValue(prefix + txtDefaultFOV.Tag).ToString();
                txtMaxSpeed.Text = cfg.getValue(prefix + txtMaxSpeed.Tag).ToString();
                txtMinSpeed.Text = cfg.getValue(prefix + txtMinSpeed.Tag).ToString();
                txtHighSpeed.Text = cfg.getValue(prefix + txtHighSpeed.Tag).ToString();
                txtLowSpeed.Text = cfg.getValue(prefix + txtLowSpeed.Tag).ToString();
                txtMaxTele.Text = cfg.getValue(prefix + txtMaxTele.Tag).ToString();
                txtRingSize.Text = cfg.getValue(prefix + txtRingSize.Tag).ToString();
                txtAdjTurn.Text = cfg.getValue(prefix + txtAdjTurn.Tag).ToString();
                txtAdjTran.Text = cfg.getValue(prefix + txtAdjTran.Tag).ToString();
                txtSpeedScale.Text = cfg.getValue(prefix + txtSpeedScale.Tag).ToString();
                txtSafeRoll.Text = cfg.getValue(prefix + txtSafeRoll.Tag).ToString();
                txtSafePich.Text = cfg.getValue(prefix + txtSafePich.Tag).ToString();
                txtPosRFIDVictSensor.Text = cfg.getValue(prefix + txtPosRFIDVictSensor.Tag).ToString();
                txtPosRFIDSensor.Text = cfg.getValue(prefix + txtPosRFIDSensor.Tag).ToString();
                txtPosRangeScanner.Text = cfg.getValue(prefix + txtPosRangeScanner.Tag).ToString();
            }
            catch
            {
                lblError.Visible = true;
                lblError.Text = "This Robot type does not exist.";
            }
            Info_RobotType_index = cmbInfo_RobotType.SelectedIndex;
        }

        #endregion

        #region Robot Management

        private void setRobotInfo()
        {
            int index = lstRobotList.SelectedIndex;
            if (index == -1)
                return;

            cmbRobotType.Text = cfg.botInfo[index].Type;

            txtRobotName.Text = cfg.botInfo[index].Name;
            txtRobotPosition.Text = cfg.botInfo[index].PositionString;
            txtRobotRotaion.Text = cfg.botInfo[index].RotationString;
            txtRobotIP.Text = cfg.botInfo[index].RobotIP;
            txtRobotPort.Text = cfg.botInfo[index].RobotPort.ToString();
        }
        private void lstRobotList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RobotList_Index != -1 && RobotList_Index < cfg.botInfo.Count && robotInfoSaveSetting)
            {
                cfg.botInfo[RobotList_Index].Type = cmbRobotType.Text;
                cfg.botInfo[RobotList_Index].Name = txtRobotName.Text;
                cfg.botInfo[RobotList_Index].RotationString = txtRobotRotaion.Text;
                cfg.botInfo[RobotList_Index].RobotIP = txtRobotIP.Text;
                cfg.botInfo[RobotList_Index].RobotPort = int.Parse(txtRobotPort.Text);
                cfg.botInfo[RobotList_Index].PositionString = txtRobotPosition.Text;
            }
            setRobotInfo();
            RobotList_Index = lstRobotList.SelectedIndex;
        }
        private void btnAddRobot_Click(object sender, EventArgs e)
        {
            robotInfoSaveSetting = false;
            cfg.botInfo.Add(new RobotInfo("Robot" + cfg.botInfo.Count.ToString()));
            setSetting();
            robotInfoSaveSetting = true;
        }
        private void btnDelRobot_Click(object sender, EventArgs e)
        {
            if (lstRobotList.SelectedIndex == -1)
                return;
            robotInfoSaveSetting = false;
            cfg.botInfo.Remove(cfg.botInfo[lstRobotList.SelectedIndex]);
            setSetting();
            setRobotInfo();
            robotInfoSaveSetting = true;
        }

        #endregion

        #region Georeference Viewer

        private void drawSample()
        {
            if (sampleBackGround == null)
                sampleBackGround = (Bitmap)picSample.Image;
            if (sampleBackGround == null)
                return;
            Bitmap bmp = new Bitmap(sampleBackGround);
            Graphics g = Graphics.FromImage(bmp);
            int x = 5
                , y = 5
                , w = picSample.Width - 10
                , h = picSample.Height - 10;
            Rectangle rec = new Rectangle(x, y, w, h);
            Pen p = null;
            Brush b = null;
            if (chkSelectedMode.Checked)
            {
                p = new Pen(new SolidBrush(Color.FromArgb((int)numericSBorderColorAplha.Value, picSBorderColor.BackColor)), (float)numericSBorderWide.Value);
                b = new SolidBrush(Color.FromArgb((int)numericSBackColorAlpha.Value, picSBackColor.BackColor));
            }
            else
            {
                p = new Pen(new SolidBrush(Color.FromArgb((int)numericBorderColorAlpha.Value, picBorderColor.BackColor)), (float)numericBorderWide.Value);
                b = new SolidBrush(Color.FromArgb((int)numericBackColorAlpha.Value, picBackColor.BackColor));
            }
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawEllipse(p, rec);
            g.FillEllipse(b, rec);
            g.Dispose();
            picSample.Image = new Bitmap(bmp);

            g.Dispose();
            bmp = new Bitmap(sampleBackGround);
            g = Graphics.FromImage(bmp);
            string drawStr = "Sample Text";
            Brush br = new SolidBrush(Color.FromArgb((int)numericHintFontColorAlpha.Value, picHintFontColor.BackColor));
            Brush br1 = new SolidBrush(Color.FromArgb((int)numericHintFontBackColorAlpha.Value, picHintFontBackColor.BackColor));

            Point[] backColor = new Point[4];
            int x1 = 2;
            int y1 = 2;
            int len = (int)Math.Ceiling(g.MeasureString(drawStr, widgetFontDialog.Font).Width);
            int height = (int)Math.Ceiling(g.MeasureString(drawStr, widgetFontDialog.Font).Height + 3);

            backColor[0] = new Point(x1, y1);
            backColor[1] = new Point(x1 + len, y1);
            backColor[2] = new Point(x1 + len, y1 + height);
            backColor[3] = new Point(x1, y1 + height);

            g.FillPolygon(br1, backColor);
            g.DrawString(drawStr, widgetFontDialog.Font, br, new PointF(x1, y1));
            picHintSample.Image = bmp;

        }
        private void pic_Click(object sender, EventArgs e)
        {
            PictureBox p = (PictureBox)sender;
            ColorDialog cd = new ColorDialog();
            cd.Color = p.BackColor;
            cd.ShowDialog();
            p.BackColor = cd.Color;
            drawSample();
        }
        private bool checkToApply(string str)
        {
            // return false;
            if (!isApplyForAllItem)
                return true;
            int index = -1;

            int count = lstWidget.Items.Count;
            for (int i = 0; i < count; i++)
            {
                string str1 = lstWidget.Items[i].ToString();
                str1 = str1.Replace(" ", "").ToLower();
                if (str1 == str.ToLower())
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
                return lstWidget.GetItemChecked(index);

            count = lstHint.Items.Count;
            for (int i = 0; i < count; i++)
            {
                string str1 = lstHint.Items[i].ToString();
                str1 = str1.Replace(" ", "").ToLower();
                if (str1 == str.ToLower())
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
                return lstHint.GetItemChecked(index);

            //switch (str)
            //{
            //    case "BackColor":
            //        if (
            //        break;
            //    case "BackColorAlpha":
            //        break;
            //    case "SelectedBackColor":
            //        break;
            //    case "SelectedBackColorAlpha":
            //        break;
            //    case "BorderColor":
            //        break;
            //    case "BorderColorAlpha":
            //        break;
            //    case "SelectedBorderColor":
            //        break;
            //    case "SelectedBorderColorAlpha":
            //        break;
            //    case "HintFontColor":
            //        break;
            //    case "HintFontColorAlpha":
            //        break;
            //    case "HintFontBackColor":
            //        break;
            //    case "HintFontBackColorAlpha":
            //        break;
            //    case "HintFont":
            //        break;
            //    case "BorderWide":
            //        break;
            //    case "SelectedBorderWide":
            //        break;
            //}
            return true;
        }
        private void setWidgetValue(string widgetName)
        {
            switch (widgetName)
            {
                case "Robot Widget":

                    if (checkToApply("BackColor")) cfg.Viewer.RobotWidget.BackColor = picBackColor.BackColor;
                    if (checkToApply("BackColorAlpha")) cfg.Viewer.RobotWidget.BackColorAlpha = (int)numericBackColorAlpha.Value;

                    if (checkToApply("SelectedBackColor")) cfg.Viewer.RobotWidget.SelectedBackColor = picSBackColor.BackColor;
                    if (checkToApply("SelectedBackColorAlpha")) cfg.Viewer.RobotWidget.SelectedBackColorAlpha = (int)numericSBackColorAlpha.Value;

                    if (checkToApply("BorderColor")) cfg.Viewer.RobotWidget.BorderColor = picBorderColor.BackColor;
                    if (checkToApply("BorderColorAlpha")) cfg.Viewer.RobotWidget.BorderColorAlpha = (int)numericBorderColorAlpha.Value;

                    if (checkToApply("SelectedBorderColor")) cfg.Viewer.RobotWidget.SelectedBorderColor = picSBorderColor.BackColor;
                    if (checkToApply("SelectedBorderColorAlpha")) cfg.Viewer.RobotWidget.SelectedBorderColorAlpha = (int)numericSBorderColorAplha.Value;

                    if (checkToApply("HintFontColor")) cfg.Viewer.RobotWidget.HintFontColor = picHintFontColor.BackColor;
                    if (checkToApply("HintFontColorAlpha")) cfg.Viewer.RobotWidget.HintFontColorAlpha = (int)numericHintFontColorAlpha.Value;

                    if (checkToApply("HintFontBackColor")) cfg.Viewer.RobotWidget.HintFontBackColor = picHintFontBackColor.BackColor;
                    if (checkToApply("HintFontBackColorAlpha")) cfg.Viewer.RobotWidget.HintFontBackColorAlpha = (int)numericHintFontBackColorAlpha.Value;

                    if (checkToApply("HintFont")) cfg.Viewer.RobotWidget.HintFont = widgetFontDialog.Font;

                    if (checkToApply("BorderWide")) cfg.Viewer.RobotWidget.BorderWide = (float)numericBorderWide.Value;
                    if (checkToApply("SelectedBorderWide")) cfg.Viewer.RobotWidget.SelectedBorderWide = (float)numericSBorderWide.Value;
                    break;
                case "Annotation":
                    if (checkToApply("BackColor")) cfg.Viewer.AnnotationWidget.BackColor = picBackColor.BackColor;
                    if (checkToApply("BackColorAlpha")) cfg.Viewer.AnnotationWidget.BackColorAlpha = (int)numericBackColorAlpha.Value;

                    if (checkToApply("SelectedBackColor")) cfg.Viewer.AnnotationWidget.SelectedBackColor = picSBackColor.BackColor;
                    if (checkToApply("SelectedBackColorAlpha")) cfg.Viewer.AnnotationWidget.SelectedBackColorAlpha = (int)numericSBackColorAlpha.Value;

                    if (checkToApply("BorderColor")) cfg.Viewer.AnnotationWidget.BorderColor = picBorderColor.BackColor;
                    if (checkToApply("BorderColorAlpha")) cfg.Viewer.AnnotationWidget.BorderColorAlpha = (int)numericBorderColorAlpha.Value;

                    if (checkToApply("SelectedBorderColor")) cfg.Viewer.AnnotationWidget.SelectedBorderColor = picSBorderColor.BackColor;
                    if (checkToApply("SelectedBorderColorAlpha")) cfg.Viewer.AnnotationWidget.SelectedBorderColorAlpha = (int)numericSBorderColorAplha.Value;

                    if (checkToApply("HintFontColor")) cfg.Viewer.AnnotationWidget.HintFontColor = picHintFontColor.BackColor;
                    if (checkToApply("HintFontColorAlpha")) cfg.Viewer.AnnotationWidget.HintFontColorAlpha = (int)numericHintFontColorAlpha.Value;

                    if (checkToApply("HintFontBackColor")) cfg.Viewer.AnnotationWidget.HintFontBackColor = picHintFontBackColor.BackColor;
                    if (checkToApply("HintFontBackColorAlpha")) cfg.Viewer.AnnotationWidget.HintFontBackColorAlpha = (int)numericHintFontBackColorAlpha.Value;

                    if (checkToApply("HintFont")) cfg.Viewer.AnnotationWidget.HintFont = widgetFontDialog.Font;

                    if (checkToApply("BorderWide")) cfg.Viewer.AnnotationWidget.BorderWide = (float)numericBorderWide.Value;
                    if (checkToApply("SelectedBorderWide")) cfg.Viewer.AnnotationWidget.SelectedBorderWide = (float)numericSBorderWide.Value;
                    break;
                case "Image Widget":
                    if (checkToApply("BackColor")) cfg.Viewer.ImageWidget.BackColor = picBackColor.BackColor;
                    if (checkToApply("BackColorAlpha")) cfg.Viewer.ImageWidget.BackColorAlpha = (int)numericBackColorAlpha.Value;

                    if (checkToApply("SelectedBackColor")) cfg.Viewer.ImageWidget.SelectedBackColor = picSBackColor.BackColor;
                    if (checkToApply("SelectedBackColorAlpha")) cfg.Viewer.ImageWidget.SelectedBackColorAlpha = (int)numericSBackColorAlpha.Value;

                    if (checkToApply("BorderColor")) cfg.Viewer.ImageWidget.BorderColor = picBorderColor.BackColor;
                    if (checkToApply("BorderColorAlpha")) cfg.Viewer.ImageWidget.BorderColorAlpha = (int)numericBorderColorAlpha.Value;

                    if (checkToApply("SelectedBorderColor")) cfg.Viewer.ImageWidget.SelectedBorderColor = picSBorderColor.BackColor;
                    if (checkToApply("SelectedBorderColorAlpha")) cfg.Viewer.ImageWidget.SelectedBorderColorAlpha = (int)numericSBorderColorAplha.Value;

                    if (checkToApply("HintFontColor")) cfg.Viewer.ImageWidget.HintFontColor = picHintFontColor.BackColor;
                    if (checkToApply("HintFontColorAlpha")) cfg.Viewer.ImageWidget.HintFontColorAlpha = (int)numericHintFontColorAlpha.Value;

                    if (checkToApply("HintFontBackColor")) cfg.Viewer.ImageWidget.HintFontBackColor = picHintFontBackColor.BackColor;
                    if (checkToApply("HintFontBackColorAlpha")) cfg.Viewer.ImageWidget.HintFontBackColorAlpha = (int)numericHintFontBackColorAlpha.Value;

                    if (checkToApply("HintFont")) cfg.Viewer.ImageWidget.HintFont = widgetFontDialog.Font;

                    if (checkToApply("BorderWide")) cfg.Viewer.ImageWidget.BorderWide = (float)numericBorderWide.Value;
                    if (checkToApply("SelectedBorderWide")) cfg.Viewer.ImageWidget.SelectedBorderWide = (float)numericSBorderWide.Value;
                    break;
                case "Robot Path Widget":
                    if (checkToApply("BackColor")) cfg.Viewer.RobotPathWidget.BackColor = picBackColor.BackColor;
                    if (checkToApply("BackColorAlpha")) cfg.Viewer.RobotPathWidget.BackColorAlpha = (int)numericBackColorAlpha.Value;

                    if (checkToApply("SelectedBackColor")) cfg.Viewer.RobotPathWidget.SelectedBackColor = picSBackColor.BackColor;
                    if (checkToApply("SelectedBackColorAlpha")) cfg.Viewer.RobotPathWidget.SelectedBackColorAlpha = (int)numericSBackColorAlpha.Value;

                    if (checkToApply("BorderColor")) cfg.Viewer.RobotPathWidget.BorderColor = picBorderColor.BackColor;
                    if (checkToApply("BorderColorAlpha")) cfg.Viewer.RobotPathWidget.BorderColorAlpha = (int)numericBorderColorAlpha.Value;

                    if (checkToApply("SelectedBorderColor")) cfg.Viewer.RobotPathWidget.SelectedBorderColor = picSBorderColor.BackColor;
                    if (checkToApply("SelectedBorderColorAlpha")) cfg.Viewer.RobotPathWidget.SelectedBorderColorAlpha = (int)numericSBorderColorAplha.Value;

                    if (checkToApply("HintFontColor")) cfg.Viewer.RobotPathWidget.HintFontColor = picHintFontColor.BackColor;
                    if (checkToApply("HintFontColorAlpha")) cfg.Viewer.RobotPathWidget.HintFontColorAlpha = (int)numericHintFontColorAlpha.Value;

                    if (checkToApply("HintFontBackColor")) cfg.Viewer.RobotPathWidget.HintFontBackColor = picHintFontBackColor.BackColor;
                    if (checkToApply("HintFontBackColorAlpha")) cfg.Viewer.RobotPathWidget.HintFontBackColorAlpha = (int)numericHintFontBackColorAlpha.Value;

                    if (checkToApply("HintFont")) cfg.Viewer.RobotPathWidget.HintFont = widgetFontDialog.Font;

                    if (checkToApply("BorderWide")) cfg.Viewer.RobotPathWidget.BorderWide = (float)numericBorderWide.Value;
                    if (checkToApply("SelectedBorderWide")) cfg.Viewer.RobotPathWidget.SelectedBorderWide = (float)numericSBorderWide.Value;

                    break;
                case "Laser Widget":
                    if (checkToApply("BackColor")) cfg.Viewer.LaserWidget.BackColor = picBackColor.BackColor;
                    if (checkToApply("BackColorAlpha")) cfg.Viewer.LaserWidget.BackColorAlpha = (int)numericBackColorAlpha.Value;

                    if (checkToApply("SelectedBackColor")) cfg.Viewer.LaserWidget.SelectedBackColor = picSBackColor.BackColor;
                    if (checkToApply("SelectedBackColorAlpha")) cfg.Viewer.LaserWidget.SelectedBackColorAlpha = (int)numericSBackColorAlpha.Value;

                    if (checkToApply("BorderColor")) cfg.Viewer.LaserWidget.BorderColor = picBorderColor.BackColor;
                    if (checkToApply("BorderColorAlpha")) cfg.Viewer.LaserWidget.BorderColorAlpha = (int)numericBorderColorAlpha.Value;

                    if (checkToApply("SelectedBorderColor")) cfg.Viewer.LaserWidget.SelectedBorderColor = picSBorderColor.BackColor;
                    if (checkToApply("SelectedBorderColorAlpha")) cfg.Viewer.LaserWidget.SelectedBorderColorAlpha = (int)numericSBorderColorAplha.Value;

                    if (checkToApply("HintFontColor")) cfg.Viewer.LaserWidget.HintFontColor = picHintFontColor.BackColor;
                    if (checkToApply("HintFontColorAlpha")) cfg.Viewer.LaserWidget.HintFontColorAlpha = (int)numericHintFontColorAlpha.Value;

                    if (checkToApply("HintFontBackColor")) cfg.Viewer.LaserWidget.HintFontBackColor = picHintFontBackColor.BackColor;
                    if (checkToApply("HintFontBackColorAlpha")) cfg.Viewer.LaserWidget.HintFontBackColorAlpha = (int)numericHintFontBackColorAlpha.Value;

                    if (checkToApply("HintFont")) cfg.Viewer.LaserWidget.HintFont = widgetFontDialog.Font;

                    if (checkToApply("BorderWide")) cfg.Viewer.LaserWidget.BorderWide = (float)numericBorderWide.Value;
                    if (checkToApply("SelectedBorderWide")) cfg.Viewer.LaserWidget.SelectedBorderWide = (float)numericSBorderWide.Value;

                    break;
                case "Mission Widget":
                    if (checkToApply("BackColor")) cfg.Viewer.MissionWidget.BackColor = picBackColor.BackColor;
                    if (checkToApply("BackColorAlpha")) cfg.Viewer.MissionWidget.BackColorAlpha = (int)numericBackColorAlpha.Value;

                    if (checkToApply("SelectedBackColor")) cfg.Viewer.MissionWidget.SelectedBackColor = picSBackColor.BackColor;
                    if (checkToApply("SelectedBackColorAlpha")) cfg.Viewer.MissionWidget.SelectedBackColorAlpha = (int)numericSBackColorAlpha.Value;

                    if (checkToApply("BorderColor")) cfg.Viewer.MissionWidget.BorderColor = picBorderColor.BackColor;
                    if (checkToApply("BorderColorAlpha")) cfg.Viewer.MissionWidget.BorderColorAlpha = (int)numericBorderColorAlpha.Value;

                    if (checkToApply("SelectedBorderColor")) cfg.Viewer.MissionWidget.SelectedBorderColor = picSBorderColor.BackColor;
                    if (checkToApply("SelectedBorderColorAlpha")) cfg.Viewer.MissionWidget.SelectedBorderColorAlpha = (int)numericSBorderColorAplha.Value;

                    if (checkToApply("HintFontColor")) cfg.Viewer.MissionWidget.HintFontColor = picHintFontColor.BackColor;
                    if (checkToApply("HintFontColorAlpha")) cfg.Viewer.MissionWidget.HintFontColorAlpha = (int)numericHintFontColorAlpha.Value;

                    if (checkToApply("HintFontBackColor")) cfg.Viewer.MissionWidget.HintFontBackColor = picHintFontBackColor.BackColor;
                    if (checkToApply("HintFontBackColorAlpha")) cfg.Viewer.MissionWidget.HintFontBackColorAlpha = (int)numericHintFontBackColorAlpha.Value;

                    if (checkToApply("HintFont")) cfg.Viewer.MissionWidget.HintFont = widgetFontDialog.Font;

                    if (checkToApply("BorderWide")) cfg.Viewer.MissionWidget.BorderWide = (float)numericBorderWide.Value;
                    if (checkToApply("SelectedBorderWide")) cfg.Viewer.MissionWidget.SelectedBorderWide = (float)numericSBorderWide.Value;
                    break;
                case "Victim Widget":
                    if (checkToApply("BackColor")) cfg.Viewer.VictimWidget.BackColor = picBackColor.BackColor;
                    if (checkToApply("BackColorAlpha")) cfg.Viewer.VictimWidget.BackColorAlpha = (int)numericBackColorAlpha.Value;

                    if (checkToApply("SelectedBackColor")) cfg.Viewer.VictimWidget.SelectedBackColor = picSBackColor.BackColor;
                    if (checkToApply("SelectedBackColorAlpha")) cfg.Viewer.VictimWidget.SelectedBackColorAlpha = (int)numericSBackColorAlpha.Value;

                    if (checkToApply("BorderColor")) cfg.Viewer.VictimWidget.BorderColor = picBorderColor.BackColor;
                    if (checkToApply("BorderColorAlpha")) cfg.Viewer.VictimWidget.BorderColorAlpha = (int)numericBorderColorAlpha.Value;

                    if (checkToApply("SelectedBorderColor")) cfg.Viewer.VictimWidget.SelectedBorderColor = picSBorderColor.BackColor;
                    if (checkToApply("SelectedBorderColorAlpha")) cfg.Viewer.VictimWidget.SelectedBorderColorAlpha = (int)numericSBorderColorAplha.Value;

                    if (checkToApply("HintFontColor")) cfg.Viewer.VictimWidget.HintFontColor = picHintFontColor.BackColor;
                    if (checkToApply("HintFontColorAlpha")) cfg.Viewer.VictimWidget.HintFontColorAlpha = (int)numericHintFontColorAlpha.Value;

                    if (checkToApply("HintFontBackColor")) cfg.Viewer.VictimWidget.HintFontBackColor = picHintFontBackColor.BackColor;
                    if (checkToApply("HintFontBackColorAlpha")) cfg.Viewer.VictimWidget.HintFontBackColorAlpha = (int)numericHintFontBackColorAlpha.Value;

                    if (checkToApply("HintFont")) cfg.Viewer.VictimWidget.HintFont = widgetFontDialog.Font;

                    if (checkToApply("BorderWide")) cfg.Viewer.VictimWidget.BorderWide = (float)numericBorderWide.Value;
                    if (checkToApply("SelectedBorderWide")) cfg.Viewer.VictimWidget.SelectedBorderWide = (float)numericSBorderWide.Value;

                    break;
            }
        }
        private void getWidgetValue(string widgetName)
        {
            switch (widgetName)
            {
                case "Robot Widget":
                    picBackColor.BackColor = cfg.Viewer.RobotWidget.BackColor;
                    numericBackColorAlpha.Value = cfg.Viewer.RobotWidget.BackColorAlpha;

                    picSBackColor.BackColor = cfg.Viewer.RobotWidget.SelectedBackColor;
                    numericSBackColorAlpha.Value = cfg.Viewer.RobotWidget.SelectedBackColorAlpha;

                    picBorderColor.BackColor = cfg.Viewer.RobotWidget.BorderColor;
                    numericBorderColorAlpha.Value = cfg.Viewer.RobotWidget.BorderColorAlpha;

                    picSBorderColor.BackColor = cfg.Viewer.RobotWidget.SelectedBorderColor;
                    numericSBorderColorAplha.Value = cfg.Viewer.RobotWidget.SelectedBorderColorAlpha;

                    picHintFontColor.BackColor = cfg.Viewer.RobotWidget.HintFontColor;
                    numericHintFontColorAlpha.Value = cfg.Viewer.RobotWidget.HintFontColorAlpha;

                    picHintFontBackColor.BackColor = cfg.Viewer.RobotWidget.HintFontBackColor;
                    numericHintFontBackColorAlpha.Value = cfg.Viewer.RobotWidget.HintFontBackColorAlpha;

                    txtHintFont.Text = getFontString(cfg.Viewer.RobotWidget.HintFont);
                    widgetFontDialog.Font = cfg.Viewer.RobotWidget.HintFont;

                    numericBorderWide.Value = (decimal)cfg.Viewer.RobotWidget.BorderWide;
                    numericSBorderWide.Value = (decimal)cfg.Viewer.RobotWidget.SelectedBorderWide;

                    break;
                case "Annotation":
                    picBackColor.BackColor = cfg.Viewer.AnnotationWidget.BackColor;
                    numericBackColorAlpha.Value = cfg.Viewer.AnnotationWidget.BackColorAlpha;

                    picSBackColor.BackColor = cfg.Viewer.AnnotationWidget.SelectedBackColor;
                    numericSBackColorAlpha.Value = cfg.Viewer.AnnotationWidget.SelectedBackColorAlpha;

                    picBorderColor.BackColor = cfg.Viewer.AnnotationWidget.BorderColor;
                    numericBorderColorAlpha.Value = cfg.Viewer.AnnotationWidget.BorderColorAlpha;

                    picSBorderColor.BackColor = cfg.Viewer.AnnotationWidget.SelectedBorderColor;
                    numericSBorderColorAplha.Value = cfg.Viewer.AnnotationWidget.SelectedBorderColorAlpha;

                    picHintFontColor.BackColor = cfg.Viewer.AnnotationWidget.HintFontColor;
                    numericHintFontColorAlpha.Value = cfg.Viewer.AnnotationWidget.HintFontColorAlpha;

                    picHintFontBackColor.BackColor = cfg.Viewer.AnnotationWidget.HintFontBackColor;
                    numericHintFontBackColorAlpha.Value = cfg.Viewer.AnnotationWidget.HintFontBackColorAlpha;

                    txtHintFont.Text = getFontString(cfg.Viewer.AnnotationWidget.HintFont);
                    widgetFontDialog.Font = cfg.Viewer.AnnotationWidget.HintFont;

                    numericBorderWide.Value = (decimal)cfg.Viewer.AnnotationWidget.BorderWide;
                    numericSBorderWide.Value = (decimal)cfg.Viewer.AnnotationWidget.SelectedBorderWide;
                    break;
                case "Image Widget":
                    picBackColor.BackColor = cfg.Viewer.ImageWidget.BackColor;
                    numericBackColorAlpha.Value = cfg.Viewer.ImageWidget.BackColorAlpha;

                    picSBackColor.BackColor = cfg.Viewer.ImageWidget.SelectedBackColor;
                    numericSBackColorAlpha.Value = cfg.Viewer.ImageWidget.SelectedBackColorAlpha;

                    picBorderColor.BackColor = cfg.Viewer.ImageWidget.BorderColor;
                    numericBorderColorAlpha.Value = cfg.Viewer.ImageWidget.BorderColorAlpha;

                    picSBorderColor.BackColor = cfg.Viewer.ImageWidget.SelectedBorderColor;
                    numericSBorderColorAplha.Value = cfg.Viewer.ImageWidget.SelectedBorderColorAlpha;

                    picHintFontColor.BackColor = cfg.Viewer.ImageWidget.HintFontColor;
                    numericHintFontColorAlpha.Value = cfg.Viewer.ImageWidget.HintFontColorAlpha;

                    picHintFontBackColor.BackColor = cfg.Viewer.ImageWidget.HintFontBackColor;
                    numericHintFontBackColorAlpha.Value = cfg.Viewer.ImageWidget.HintFontBackColorAlpha;

                    txtHintFont.Text = getFontString(cfg.Viewer.ImageWidget.HintFont);
                    widgetFontDialog.Font = cfg.Viewer.ImageWidget.HintFont;

                    numericBorderWide.Value = (decimal)cfg.Viewer.ImageWidget.BorderWide;
                    numericSBorderWide.Value = (decimal)cfg.Viewer.ImageWidget.SelectedBorderWide;
                    break;
                case "Robot Path Widget":
                    picBackColor.BackColor = cfg.Viewer.RobotPathWidget.BackColor;
                    numericBackColorAlpha.Value = cfg.Viewer.RobotPathWidget.BackColorAlpha;

                    picSBackColor.BackColor = cfg.Viewer.RobotPathWidget.SelectedBackColor;
                    numericSBackColorAlpha.Value = cfg.Viewer.RobotPathWidget.SelectedBackColorAlpha;

                    picBorderColor.BackColor = cfg.Viewer.RobotPathWidget.BorderColor;
                    numericBorderColorAlpha.Value = cfg.Viewer.RobotPathWidget.BorderColorAlpha;

                    picSBorderColor.BackColor = cfg.Viewer.RobotPathWidget.SelectedBorderColor;
                    numericSBorderColorAplha.Value = cfg.Viewer.RobotPathWidget.SelectedBorderColorAlpha;

                    picHintFontColor.BackColor = cfg.Viewer.RobotPathWidget.HintFontColor;
                    numericHintFontColorAlpha.Value = cfg.Viewer.RobotPathWidget.HintFontColorAlpha;

                    picHintFontBackColor.BackColor = cfg.Viewer.RobotPathWidget.HintFontBackColor;
                    numericHintFontBackColorAlpha.Value = cfg.Viewer.RobotPathWidget.HintFontBackColorAlpha;

                    txtHintFont.Text = getFontString(cfg.Viewer.RobotPathWidget.HintFont);
                    widgetFontDialog.Font = cfg.Viewer.RobotPathWidget.HintFont;

                    numericBorderWide.Value = (decimal)cfg.Viewer.RobotPathWidget.BorderWide;
                    numericSBorderWide.Value = (decimal)cfg.Viewer.RobotPathWidget.SelectedBorderWide;
                    break;
                case "Laser Widget":
                    picBackColor.BackColor = cfg.Viewer.LaserWidget.BackColor;
                    numericBackColorAlpha.Value = cfg.Viewer.LaserWidget.BackColorAlpha;

                    picSBackColor.BackColor = cfg.Viewer.LaserWidget.SelectedBackColor;
                    numericSBackColorAlpha.Value = cfg.Viewer.LaserWidget.SelectedBackColorAlpha;

                    picBorderColor.BackColor = cfg.Viewer.LaserWidget.BorderColor;
                    numericBorderColorAlpha.Value = cfg.Viewer.LaserWidget.BorderColorAlpha;

                    picSBorderColor.BackColor = cfg.Viewer.LaserWidget.SelectedBorderColor;
                    numericSBorderColorAplha.Value = cfg.Viewer.LaserWidget.SelectedBorderColorAlpha;

                    picHintFontColor.BackColor = cfg.Viewer.LaserWidget.HintFontColor;
                    numericHintFontColorAlpha.Value = cfg.Viewer.LaserWidget.HintFontColorAlpha;

                    picHintFontBackColor.BackColor = cfg.Viewer.LaserWidget.HintFontBackColor;
                    numericHintFontBackColorAlpha.Value = cfg.Viewer.LaserWidget.HintFontBackColorAlpha;

                    txtHintFont.Text = getFontString(cfg.Viewer.LaserWidget.HintFont);
                    widgetFontDialog.Font = cfg.Viewer.LaserWidget.HintFont;

                    numericBorderWide.Value = (decimal)cfg.Viewer.LaserWidget.BorderWide;
                    numericSBorderWide.Value = (decimal)cfg.Viewer.LaserWidget.SelectedBorderWide;
                    break;
                case "Mission Widget":
                    picBackColor.BackColor = cfg.Viewer.MissionWidget.BackColor;
                    numericBackColorAlpha.Value = cfg.Viewer.MissionWidget.BackColorAlpha;

                    picSBackColor.BackColor = cfg.Viewer.MissionWidget.SelectedBackColor;
                    numericSBackColorAlpha.Value = cfg.Viewer.MissionWidget.SelectedBackColorAlpha;

                    picBorderColor.BackColor = cfg.Viewer.MissionWidget.BorderColor;
                    numericBorderColorAlpha.Value = cfg.Viewer.MissionWidget.BorderColorAlpha;

                    picSBorderColor.BackColor = cfg.Viewer.MissionWidget.SelectedBorderColor;
                    numericSBorderColorAplha.Value = cfg.Viewer.MissionWidget.SelectedBorderColorAlpha;

                    picHintFontColor.BackColor = cfg.Viewer.MissionWidget.HintFontColor;
                    numericHintFontColorAlpha.Value = cfg.Viewer.MissionWidget.HintFontColorAlpha;

                    picHintFontBackColor.BackColor = cfg.Viewer.MissionWidget.HintFontBackColor;
                    numericHintFontBackColorAlpha.Value = cfg.Viewer.MissionWidget.HintFontBackColorAlpha;

                    txtHintFont.Text = getFontString(cfg.Viewer.MissionWidget.HintFont);
                    widgetFontDialog.Font = cfg.Viewer.MissionWidget.HintFont;

                    numericBorderWide.Value = (decimal)cfg.Viewer.MissionWidget.BorderWide;
                    numericSBorderWide.Value = (decimal)cfg.Viewer.MissionWidget.SelectedBorderWide;
                    break;
                case "Victim Widget":
                    picBackColor.BackColor = cfg.Viewer.VictimWidget.BackColor;
                    numericBackColorAlpha.Value = cfg.Viewer.VictimWidget.BackColorAlpha;

                    picSBackColor.BackColor = cfg.Viewer.VictimWidget.SelectedBackColor;
                    numericSBackColorAlpha.Value = cfg.Viewer.VictimWidget.SelectedBackColorAlpha;

                    picBorderColor.BackColor = cfg.Viewer.VictimWidget.BorderColor;
                    numericBorderColorAlpha.Value = cfg.Viewer.VictimWidget.BorderColorAlpha;

                    picSBorderColor.BackColor = cfg.Viewer.VictimWidget.SelectedBorderColor;
                    numericSBorderColorAplha.Value = cfg.Viewer.VictimWidget.SelectedBorderColorAlpha;

                    picHintFontColor.BackColor = cfg.Viewer.VictimWidget.HintFontColor;
                    numericHintFontColorAlpha.Value = cfg.Viewer.VictimWidget.HintFontColorAlpha;

                    picHintFontBackColor.BackColor = cfg.Viewer.VictimWidget.HintFontBackColor;
                    numericHintFontBackColorAlpha.Value = cfg.Viewer.VictimWidget.HintFontBackColorAlpha;

                    txtHintFont.Text = getFontString(cfg.Viewer.VictimWidget.HintFont);
                    widgetFontDialog.Font = cfg.Viewer.VictimWidget.HintFont;

                    numericBorderWide.Value = (decimal)cfg.Viewer.VictimWidget.BorderWide;
                    numericSBorderWide.Value = (decimal)cfg.Viewer.VictimWidget.SelectedBorderWide;
                    break;
            }
        }
        private void cmbWidget_SelectedIndexChanged(object sender, EventArgs e)
        {
            // isApplyForAllItem = false;
            if (cmbWidget.SelectedItem.ToString() == "All Widget")
            {
                if (MessageBox.Show("Do you want apply robot setting for all widgets?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    cmbWidget.SelectedIndex = grv_cmbSelectedIndex;
                    return;
                }
                isApplyForAllItem = true;
            }

            if (grv_cmbSelectedIndex != -1 && grv_cmbSelectedIndex < cmbWidget.Items.Count)
            {
                string wName = cmbWidget.Items[grv_cmbSelectedIndex].ToString();
                if (wName.Equals("All Widget"))
                {
                    foreach (object obj in cmbWidget.Items)
                        if (obj.ToString().Equals("All Widget"))
                            continue;
                        else
                            setWidgetValue(obj.ToString());
                    isApplyForAllItem = false;
                }
                else
                    setWidgetValue(wName);
            }
            if (cmbWidget.Text.Equals("All Widget"))
                getWidgetValue(cmbWidget.Items[1].ToString());
            else
                getWidgetValue(cmbWidget.Text);

            grv_cmbSelectedIndex = cmbWidget.SelectedIndex;
            drawSample();
        }
        private string getFontString(Font font)
        {
            string str = "Name:" + font.Name;
            str += "; Size:" + font.Size.ToString();
            str += "; Style:" + font.Style;
            return str;
        }
        private void txtWidgetFont_Click(object sender, EventArgs e)
        {
            widgetFontDialog.ShowDialog();
            txtHintFont.Text = getFontString(widgetFontDialog.Font);
            picHintFontColor.Focus();
        }
        private void numeric_ValueChanged(object sender, EventArgs e)
        {
            drawSample();
        }
        private void numericSBorderWide_ValueChanged(object sender, EventArgs e)
        {

            NumericUpDown nm = (NumericUpDown)sender;
            if (nm.Name == numericBorderWide.Name)
                lblBorderWide.Text = numericBorderWide.Value.ToString();
            else
                lblSelectedBorderWide.Text = numericSBorderWide.Value.ToString();
            drawSample();

        }
        private void chkSelectedMode_CheckedChanged(object sender, EventArgs e)
        {
            drawSample();
        }

        #endregion

        private void txtSimIP_TextChanged(object sender, EventArgs e)
        {
            if (chkApplyToAll.Checked)
            {
                string val =txtSimIP.Text;
                txtImageServerIP.Text = val;
                txtWSSIP.Text = val;
             
            }
        }
       
    }

}
