using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Components.Tools;
using MRL.Components.Tools.Shapes;
using MRL.Components.Tools.Widgets;
using MRL.CustomMath;
using MRL.Mapping;
using MRL.Utils;
using SlimDX.Direct2D;
using MRL.Communication.Tools;

namespace MRL.Components
{
    public partial class GeoreferenceImageViewer : UserControl
    {

        #region Variables

        public static double WHEEL_SCALE_FACTOR = 1.1;

        private const string dateFormat = "MMddyyyy_hhmmss";

        private IEGMap _worldMap = null;
        public WidgetList widgetList;

        private MapWidget _mapWidget;
        private RobotWidget _robotWidget;
        private ImageWidget _imageWidget;
        private VictimWidget _victimWidget;
        private AnnotationWidget _annotationWidget;
        private MissionWidget _missionWidget;
        private ScaleBarWidget _scaleBarWidget;
        private RobotPathWidget _robotPathWidget;
        //private RSSIWidget _rssiWidget;
        private GoalPointsWidget _goalPoints;
        private ComStationWidget _comStation;
        private TargetWidget _targets;
        private SignalWidget _signalWidget;

        //private ComStationWidget _comStation;

        private string _hint = "";
        private System.Drawing.Point _hintLocation;
        private System.Drawing.Bitmap prevMap = null;
        private System.Drawing.Font hintFont = new System.Drawing.Font("Tahoma", 10f);
        private ReaderWriterLockSlim mRenderer = new ReaderWriterLockSlim();

        #endregion

        #region  Constructor

        public GeoreferenceImageViewer()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.ResizeRedraw, false);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            //TODO: create new directory for this run
            setResultDirectory();

            Factory factory = new Factory(FactoryType.Multithreaded, DebugLevel.None);
            RenderTargetProperties renderProperty = new RenderTargetProperties();
            WindowRenderTargetProperties wRenderTargetProperty = new WindowRenderTargetProperties();
            wRenderTargetProperty.Handle = this.Handle;
            wRenderTargetProperty.PixelSize = this.Size;

            ProjectCommons.Drawing2D.windowRenderTarget = new WindowRenderTarget(factory, renderProperty, wRenderTargetProperty);
        }

        #endregion

        #region Methods

        private void createWidgets()
        {
            widgetList = new WidgetList();

            _mapWidget = new MapWidget();
            _robotWidget = new RobotWidget();
            _imageWidget = new ImageWidget();
            _victimWidget = new VictimWidget();
            _annotationWidget = new AnnotationWidget();
            _missionWidget = new MissionWidget();
            _scaleBarWidget = new ScaleBarWidget();
            _targets = new TargetWidget();
            _robotPathWidget = new RobotPathWidget();
            //_rssiWidget = new RSSIWidget();
            _goalPoints = new GoalPointsWidget();
            _comStation = new ComStationWidget();
            _signalWidget = new SignalWidget();

            //_comStation = new ComStationWidget();

            widgetList.Add(_mapWidget);
            widgetList.Add(_missionWidget);
            widgetList.Add(_robotPathWidget);
            widgetList.Add(_annotationWidget);
            widgetList.Add(_imageWidget);
            widgetList.Add(_victimWidget);

            //widgetList.Add(_rssiWidget);
            widgetList.Add(_goalPoints);
            widgetList.Add(_robotWidget);
            widgetList.Add(_scaleBarWidget);
            widgetList.Add(_targets);
            widgetList.Add(_comStation);
            widgetList.Add(_signalWidget);

            foreach (WidgetBase w in widgetList)
            { w.Hint += new ProjectCommons._hint(Hint); }
        }

        public void updateGeomap(System.Drawing.Bitmap b)
        {
            try
            {
                mRenderer.EnterWriteLock();

                if (b != null)
                {
                    SlimDX.Direct2D.Bitmap convertedBMP = ProjectCommons.Drawing2D.Convert(b);
                    _mapWidget.upadteMap(convertedBMP);
                    if (prevMap == null)
                    {
                        prevMap = b;
                        ProjectCommons.Drawing2D.lastMapUpdated = 0;
                    }
                    else
                    {
                        if (!b.Size.Equals(prevMap.Size))
                            ProjectCommons.Drawing2D.lastMapUpdated++;
                        prevMap = b;
                    }

                    this.Invalidate();
                }
            }
            catch (Exception ex)
            {
                ProjectCommons.writeConsoleMessage("Can not update map.\n" + ex.ToString(), ConsoleMessageType.Error);
            }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }
        public void updateRobot(int mountIndex, Pose3D p)
        {
            try
            {
                mRenderer.EnterWriteLock();
                _robotWidget.updateRobot(mountIndex, p);
                _robotPathWidget.addPosition(mountIndex, p);
                //     _rssiWidget.updateRobot(mountIndex, p);

                this.Invalidate();
            }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }

        public void updateComstation(Pose3D p)
        {
            try
            {
                mRenderer.EnterWriteLock();
                _comStation.updateComStation();
                this.Invalidate();
            }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }
        public void addVictim(int index, Pose3D p, double probability)
        {
            try
            {
                mRenderer.EnterWriteLock();
                _victimWidget.updateVictim(index, p, probability);
                this.Invalidate();
            }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }
        public void removeAllVictims()
        {
            try
            {
                mRenderer.EnterWriteLock();
                _victimWidget.removeAllVictims();
                this.Invalidate();
            }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }


        public void addMission(int index, List<Pose2D> missionPath)
        {
            RobotInfo r = ProjectCommons.config.botInfo[index];
            _missionWidget.addMission(r, missionPath);
        }

        public void updateSignals(List<SignalLine> signals)
        {
            try
            {
                if (mRenderer == null) return;
                if (_signalWidget == null) return;

                mRenderer.EnterWriteLock();
                _signalWidget.updateSignals(signals);
                this.Invalidate();
            }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }

        public void removeMission(int rIndex)
        {
            try
            {
                mRenderer.EnterWriteLock();
                RobotInfo r = ProjectCommons.config.botInfo[rIndex];
                _missionWidget.clearMission(r);
                this.Invalidate();
            }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }
        public void addRobotCameraImage(int id, System.Drawing.Bitmap bmp, Pose3D position, string name)
        {
            try
            {
                mRenderer.EnterWriteLock();
                _imageWidget.updateRobotImage(id, bmp, position, name);
                this.Invalidate();
            }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }
        public void robotAchieveMission(int robotIndex, Pose2D currentPoint)
        {
            try
            {
                mRenderer.EnterWriteLock();
                _missionWidget.robotAchieveMission(robotIndex, currentPoint);
                this.Invalidate();

                //mRenderer.EnterWriteLock();
                //_missionWidget.robotAchieveMission(robotIndex);
                //this.Invalidate();
            }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }

        public void generateResult()
        {
            try
            {
                mRenderer.EnterWriteLock();
                ThreadPool.QueueUserWorkItem(new WaitCallback(SaveResult));
            }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }
        private void Hint(string msg, System.Drawing.Point location)
        {
            try
            {
                mRenderer.EnterWriteLock();
                _hint = msg;
                _hintLocation = location;
                this.Invalidate();
            }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }

        private void rePaint()
        {
            if ((MouseData.SelectedShape != null) && (MouseData.SelectedObject is RobotInfo))
            {
                ShapeBase shapeBase = MouseData.SelectedShape;
                System.Drawing.Point center = new System.Drawing.Point(this.Width / 4, this.Height / 2);
                MouseDataBase.DrawPoint.X = center.X - (int)(shapeBase.CanvasPose.X * ProjectCommons.Drawing2D.Scale);
                MouseDataBase.DrawPoint.Y = center.Y - (int)(shapeBase.CanvasPose.Y * ProjectCommons.Drawing2D.Scale);
            }

            if (ProjectCommons.Drawing2D.GlobalMap != null)
            {
                foreach (WidgetBase w in widgetList)
                { w.Paint(ProjectCommons.Drawing2D.windowRenderTarget); }
            }
            if (ProjectCommons.Drawing2D.lastMapUpdated != -1)
            {
                ProjectCommons.Drawing2D.BaseBody.lastUpdated = ProjectCommons.Drawing2D.lastMapUpdated;
            }
            if (_hint != "")
            {
                string[] temp = _hint.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                SlimDX.Color4 hFontBackColor = System.Drawing.Color.White, hFontColor = System.Drawing.Color.Black;
                Brush fontColor = new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, hFontColor);
                Brush fontBackColor = new SolidColorBrush(ProjectCommons.Drawing2D.windowRenderTarget, hFontBackColor);

                int x = (int)(_hintLocation.X * ProjectCommons.Drawing2D.Scale) + 15 + MouseDataBase.DrawPoint.X;// +(15 + (int)Math.Ceiling(0.8f * ProjectCommons.Drawing2D.Scale));
                int y = (int)(_hintLocation.Y * ProjectCommons.Drawing2D.Scale) + 5 + MouseDataBase.DrawPoint.Y;// (10 + (int)Math.Ceiling(0.8f * ProjectCommons.Drawing2D.Scale));
                if (x < 1) x = 1;
                if (y < 1) y = 1;

                System.Drawing.Size s = ProjectCommons.Drawing2D.mesureString(temp[1], hintFont);
                ProjectCommons.Drawing2D.windowRenderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix_noneWorldAndScale((float)x, (float)y);
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, s.Width, s.Height);
                ProjectCommons.Drawing2D.windowRenderTarget.FillRectangle(fontBackColor, rect);
                ProjectCommons.Drawing2D.windowRenderTarget.DrawText(temp[1], ProjectCommons.Drawing2D.textFormat, rect, fontColor);
            }
        }

        #endregion

        #region Property
        [DefaultValue(true)]
        [Category("Widgets")]
        [DisplayName("Map")]
        [Description("Indicate visibility of geo map widget")]
        public bool MapVisiblity
        {
            get
            {
                if (_mapWidget != null)
                    return _mapWidget.Visible;
                else
                    return false;
            }
            set
            {
                if (_mapWidget != null)
                {
                    _mapWidget.Visible = value;
                    //if (value)
                    //{
                    //    if (!widgetList.Contains(_mapWidget))
                    //        widgetList.Add(_mapWidget);
                    //    widgetList.ReOrderWithLastOrder();
                    //}
                    //else
                    //    widgetList.Remove(_mapWidget);
                    this.Invalidate();

                }
            }
        }

        [DefaultValue(true)]
        [Category("Widgets")]
        [DisplayName("Robot")]
        [Description("Indicate visibility of robot widget")]
        public bool RobotVisiblity
        {
            get
            {
                if (_robotWidget != null)
                    return _robotWidget.Visible;
                else
                    return false;
            }
            set
            {
                if (_robotWidget != null)
                {
                    _robotWidget.Visible = value;
                    //if (value)
                    //{
                    //    if (!widgetList.Contains(_robotWidget))
                    //        widgetList.Add(_robotWidget);
                    //    widgetList.ReOrderWithLastOrder();
                    //}
                    //else
                    //    widgetList.Remove(_robotWidget);
                    this.Invalidate();

                }
            }
        }

        [DefaultValue(true)]
        [Category("Widgets")]
        [DisplayName("Camera Pics")]
        [Description("Indicate visibility of camera widget")]
        public bool ImageVisiblity
        {
            get
            {
                if (_imageWidget != null)
                    return _imageWidget.Visible;
                else
                    return false;
            }
            set
            {
                if (_imageWidget != null)
                {
                    _imageWidget.Visible = value;
                    //if (value)
                    //{
                    //    if (!widgetList.Contains(_imageWidget))
                    //        widgetList.Add(_imageWidget);
                    //    widgetList.ReOrderWithLastOrder();
                    //}
                    //else
                    //    widgetList.Remove(_imageWidget);
                    this.Invalidate();

                }
            }
        }

        [DefaultValue(true)]
        [Category("Widgets")]
        [DisplayName("Victim")]
        [Description("Indicate visibility of victim widget")]
        public bool VictimVisiblity
        {
            get
            {
                if (_victimWidget != null)
                    return _victimWidget.Visible;
                else
                    return false;
            }
            set
            {
                if (_victimWidget != null)
                {
                    _victimWidget.Visible = value;
                    //if (value)
                    //{
                    //    if (!widgetList.Contains(_victimWidget))
                    //        widgetList.Add(_victimWidget);
                    //    widgetList.ReOrderWithLastOrder();
                    //}
                    //else
                    //    widgetList.Remove(_victimWidget);
                    this.Invalidate();
                }
            }
        }

        [DefaultValue(true)]
        [Category("Widgets")]
        [DisplayName("Annotation")]
        [Description("Indicate visibility of annotation widget")]
        public bool AnnotationVisiblity
        {
            get
            {
                if (_annotationWidget != null)
                    return _annotationWidget.Visible;
                else
                    return false;
            }
            set
            {
                if (_annotationWidget != null)
                {
                    _annotationWidget.Visible = value;
                    //if (value)
                    //{
                    //    if (!widgetList.Contains(_annotationWidget))
                    //        widgetList.Add(_annotationWidget);
                    //    widgetList.ReOrderWithLastOrder();
                    //}
                    //else
                    //    widgetList.Remove(_annotationWidget);

                    this.Invalidate();
                }
            }
        }

        [DefaultValue(true)]
        [Category("Widgets")]
        [DisplayName("Mission")]
        [Description("Indicate visibility of mission widget")]
        public bool MissionVisiblity
        {
            get
            {
                if (_missionWidget != null)
                    return _missionWidget.Visible;
                else
                    return false;
            }
            set
            {
                if (_missionWidget != null)
                {
                    _missionWidget.Visible = value;
                    //if (value)
                    //{
                    //    if (!widgetList.Contains(_missionWidget))
                    //        widgetList.Add(_missionWidget);
                    //    //widgetList.ReOrderWithLastOrder();
                    //}
                    ////else
                    ////    widgetList.Remove(_missionWidget);

                    this.Invalidate();
                }
            }
        }

        [DefaultValue(true)]
        [Category("Widgets")]
        [DisplayName("Robot Path")]
        [Description("Indicate visibility of robot path widget")]
        public bool RobotPathVisiblity
        {
            get
            {
                if (_robotPathWidget != null)
                    return _robotPathWidget.Visible;
                else
                    return false;
            }
            set
            {
                if (_robotPathWidget != null)
                {
                    _robotPathWidget.Visible = value;
                    //if (value)
                    //{
                    //    if (!widgetList.Contains(_robotPathWidget))
                    //        widgetList.Add(_robotPathWidget);
                    //    widgetList.ReOrderWithLastOrder();
                    //}
                    //else
                    //    widgetList.Remove(_robotPathWidget);

                    this.Invalidate();
                }
            }
        }


        [DefaultValue(true)]
        [Category("Widgets")]
        [DisplayName("Goal Points")]
        [Description("Indicate visibility of goal point widget")]
        public bool GoalPointsVisiblity
        {
            get
            {
                if (_goalPoints != null)
                    return _goalPoints.Visible;
                else
                    return false;
            }
            set
            {
                if (_goalPoints != null)
                {
                    _goalPoints.Visible = value;
                    //if (value)
                    //{
                    //    if (!widgetList.Contains(_goalPoints))
                    //        widgetList.Add(_goalPoints);
                    //    widgetList.ReOrderWithLastOrder();
                    //}
                    //else
                    //    widgetList.Remove(_goalPoints);

                    this.Invalidate();
                }
            }
        }

        [DefaultValue(true)]
        [Category("Widgets")]
        [DisplayName("Station")]
        [Description("Indicate visibility of comm.station widget")]
        public bool ComStationVisiblity
        {
            get
            {
                if (_comStation != null)
                    return _comStation.Visible;
                else
                    return false;
            }
            set
            {
                if (_comStation != null)
                {
                    _comStation.Visible = value;
                    //if (value)
                    //{
                    //    if (!widgetList.Contains(_comStation))
                    //        widgetList.Add(_comStation);
                    //    widgetList.ReOrderWithLastOrder();
                    //}
                    //else
                    //    widgetList.Remove(_comStation);

                    this.Invalidate();
                }
            }
        }

        //[DefaultValue(true)]
        //[Category("Widgets")]
        //[DisplayName("ComGraph")]
        //[Description("Indicate visibility of comm.graph widget")]
        //public bool RSSIVisiblity
        //{
        //    get
        //    {
        //        if (_rssiWidget != null)
        //            return _rssiWidget.Visible;
        //        else
        //            return false;
        //    }
        //    set
        //    {
        //        if (_rssiWidget != null)
        //        {
        //            _rssiWidget.Visible = value;
        //            if (value)
        //            {
        //                if (!widgetList.Contains(_rssiWidget))
        //                    widgetList.Add(_rssiWidget);
        //            }
        //            else
        //                widgetList.Remove(_rssiWidget);

        //            this.Invalidate();
        //        }
        //    }
        //}

        [DefaultValue(true)]
        [Category("Widgets")]
        [DisplayName("Scale Bar")]
        [Description("Indicate visibility of scalebar widget")]
        public bool ScaleBarVisiblity
        {
            get
            {
                if (_scaleBarWidget != null)
                    return _scaleBarWidget.Visible;
                else
                    return false;
            }
            set
            {
                if (_scaleBarWidget != null)
                {
                    _scaleBarWidget.Visible = value;
                    //if (value)
                    //{
                    //    if (!widgetList.Contains(_scaleBarWidget))
                    //        widgetList.Add(_scaleBarWidget);
                    //    widgetList.ReOrderWithLastOrder();
                    //}
                    //else
                    //    widgetList.Remove(_scaleBarWidget);

                    this.Invalidate();
                }
            }
        }

        [DefaultValue(true)]
        [Category("Widgets")]
        [DisplayName("Signal Tree")]
        [Description("Indicate visibility of signal tree widget")]
        public bool SignalWidgetVisiblity
        {
            get
            {
                if (_signalWidget != null)
                    return _signalWidget.Visible;
                else
                    return false;
            }
            set
            {
                if (_signalWidget != null)
                {
                    _signalWidget.Visible = value;
                    //if (value)
                    //{
                    //    if (!widgetList.Contains(_scaleBarWidget))
                    //        widgetList.Add(_scaleBarWidget);
                    //    widgetList.ReOrderWithLastOrder();
                    //}
                    //else
                    //    widgetList.Remove(_scaleBarWidget);

                    this.Invalidate();
                }
            }
        }



        [Browsable(false)]
        public IEGMap GridMapper
        {
            set
            {
                _worldMap = value;
                ProjectCommons.writeConsoleMessage(widgetList.Count.ToString(), ConsoleMessageType.Exclamation);
                ProjectCommons.writeConsoleMessage("IEGMap = " + (value == null), ConsoleMessageType.Exclamation);
                ProjectCommons.Drawing2D.GlobalMap = value;
            }
        }

        #endregion

        #region  Events

        private void GeoreferenceImageViewer_Load(object sender, EventArgs e)
        {
            MouseData.ViewerSize.Width = this.Width;
            MouseData.ViewerSize.Height = this.Height;
            createWidgets();
        }
        private void GeoreferenceImageViewer_Click(object sender, MouseEventArgs e)
        {
            int count = widgetList.Count;
            for (int i = 0; i < count; i++)
                widgetList[i].mouseClicked(e);

            this.Invalidate();
        }
        private void GeoreferenceImageViewer_DoubleClick(object sender, MouseEventArgs e)
        {
            int count = widgetList.Count;
            for (int i = 0; i < count; i++)
                widgetList[i].mouseDClicked(e);

            this.Invalidate();
        }
        private void GeoreferenceImageViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (!MouseData.ForceForNoChangeSelectedObject)
                MouseData.SelectedObject = null;

            MouseDataBase.StartPoint = e.Location;

            int count = widgetList.Count;
            for (int i = 0; i < count; i++)
                widgetList[i].mouseDown(e);
        }
        private void GeoreferenceImageViewer_MouseUp(object sender, MouseEventArgs e)
        {
            MouseDataBase.StartPoint.X = -1;
            MouseDataBase.StartPoint.Y = -1;

            MouseDataBase.DrawPoint.X += MouseDataBase.Offset.X;
            MouseDataBase.DrawPoint.Y += MouseDataBase.Offset.Y;

            MouseDataBase.Offset.X = 0;
            MouseDataBase.Offset.Y = 0;

            this.Invalidate();
        }
        private void GeoreferenceImageViewer_MouseMove(object sender, MouseEventArgs e)
        {
            MouseDataBase.CurrentPoint = e.Location;
            MouseDataBase.CurrentPoint_Scaled.X = (int)Math.Ceiling((e.Location.X - MouseDataBase.DrawPoint.X) / ProjectCommons.Drawing2D.Scale);
            MouseDataBase.CurrentPoint_Scaled.Y = (int)Math.Ceiling((e.Location.Y - MouseDataBase.DrawPoint.Y) / ProjectCommons.Drawing2D.Scale);

            if (e.Button == MouseButtons.Left)
            {
                // mouse dragged
                MouseDataBase.Offset.X = MouseDataBase.CurrentPoint.X - MouseDataBase.StartPoint.X;
                MouseDataBase.Offset.Y = MouseDataBase.CurrentPoint.Y - MouseDataBase.StartPoint.Y;
            }
            else
            {
                int x = (int)((MouseDataBase.CurrentPoint.X - MouseDataBase.DrawPoint.X) / ProjectCommons.Drawing2D.Scale);
                int y = (int)((MouseDataBase.CurrentPoint.Y - MouseDataBase.DrawPoint.Y) / ProjectCommons.Drawing2D.Scale);

                _hint = "";
                if (MouseDataBase.StartPoint.X == -1)
                {
                    int count = widgetList.Count;
                    for (int i = 0; i < count; i++)
                        widgetList[i].mouseMoved(e);
                    //_robotWidget.mouseMoved(e);
                }
            }
            this.Invalidate();
        }
        private void GeoreferenceImageViewer_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            float scaleDiff = (float)Math.Pow(WHEEL_SCALE_FACTOR, (e.Delta / 60));

            ProjectCommons.Drawing2D.Scale *= scaleDiff;
            if (ProjectCommons.Drawing2D.Scale < 0.1f)
                ProjectCommons.Drawing2D.Scale = 0.1f;
            int count = widgetList.Count;
            for (int i = 0; i < count; i++)
                widgetList[i].mouseWheelMoved(e);

            this.Invalidate();
        }
        private void GeoreferenceImageViewer_MouseEnter(object sender, EventArgs e)
        {
            this.Focus();
        }

        public void Viewer_KeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                generateResult();
            }

            int count = widgetList.Count;
            for (int i = 0; i < count; i++)
                widgetList[i].keyDown(e);

            if (e.KeyCode == Keys.Delete)
            {
                ProjectCommons.writeConsoleMessage("collection count : " + MouseData.objectCollection.Count, ConsoleMessageType.Normal);
            }

            if (e.KeyCode == Keys.PageDown)
            {
                MouseData.changeTabIndex(true);
                this.Invalidate();
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                MouseData.changeTabIndex(false);
                this.Invalidate();
            }
        }
        public void Viewer_KeyUp(KeyEventArgs e)
        {
            int count = widgetList.Count;
            for (int i = 0; i < count; i++)
                widgetList[i].keyUp(e);

            this.Invalidate();
        }

        private void GeoreferenceImageViewer_Resize(object sender, EventArgs e)
        {
            MouseData.ViewerSize.Height = this.Height;
            MouseData.ViewerSize.Width = this.Width;
        }

        private void cms_AddVictim_Click(object sender, EventArgs e)
        {
            try
            {
                mRenderer.EnterWriteLock();
                _victimWidget.addVictim(MouseDataBase.CurrentPoint_Scaled);
                this.Invalidate();
            }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }
        private void GeoreferenceImageViewer_SizeChanged(object sender, EventArgs e)
        {
            if (ProjectCommons.Drawing2D.windowRenderTarget.Transform != null)
            {
                try
                {
                    mRenderer.EnterWriteLock();
                    ProjectCommons.Drawing2D.windowRenderTarget.Resize(this.ClientSize);
                }
                finally
                {
                    mRenderer.ExitWriteLock();
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (mRenderer.RecursiveWriteCount > 0) return;

            try
            {
                mRenderer.EnterWriteLock();

                if (!ProjectCommons.Drawing2D.windowRenderTarget.IsOccluded)
                {
                    ProjectCommons.Drawing2D.windowRenderTarget.BeginDraw();

                    ProjectCommons.Drawing2D.changeMatrix();
                    ProjectCommons.Drawing2D.windowRenderTarget.Clear(this.BackColor);

                    rePaint();
                    ProjectCommons.Drawing2D.windowRenderTarget.EndDraw();
                }

            }
            catch { }
            finally
            {
                mRenderer.ExitWriteLock();
            }
        }

        #endregion

    }
}
