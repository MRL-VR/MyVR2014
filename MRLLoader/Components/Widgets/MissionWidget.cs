using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using MRL.Commons;
using MRL.Components.Tools.Shapes;
using MRL.CustomMath;
using MRL.Utils;
using SlimDX.Direct2D;

namespace MRL.Components.Tools.Widgets
{
    public class MissionWidget : WidgetBase
    {
        #region Variables

        private Dictionary<int, MissionShape> _missionsShape = new Dictionary<int, MissionShape>();
        private RobotInfo _selectedRobot;
        private List<Pose2D> _underDrawRect3D = new List<Pose2D>();
        private List<Pose2D> _underDrawRect2D = new List<Pose2D>();
        private Keys _shiftKey = Keys.S;
        private bool _isDrawing = false;
        private DrawState _drawState;
        private Brush drawingBrush = null;
        private Brush underDrawBrush = null;
        #endregion

        #region Constructor

        public MissionWidget()
        {
            try { this._drawState = ProjectCommons.config.Viewer.MissionWidget; }
            catch { this._drawState = new DrawState(); }
        }

        #endregion

        #region Methods

        public void addMission(RobotInfo robot, List<Pose2D> path)
        {
            MissionShape mShape = new MissionShape();

            mShape.RobotInfo = robot;
            mShape.RealBody = new List<Pose2D>();
            mShape.CanvasBody = new List<Pose2D>();

            mShape.RealBody.AddRange(path);

            mShape.CanvasBody.Clear();
            foreach (Pose2D p in mShape.RealBody)
                mShape.CanvasBody.Add(ProjectCommons.Drawing2D.ChangeRealToCanvas(p));

            try
            { _missionsShape.Add(mShape.RobotInfo.MountIndex, mShape); }
            catch
            { _missionsShape[mShape.RobotInfo.MountIndex] = mShape; }
        }

        public void clearMission(RobotInfo robot)
        {
            try
            {
                _missionsShape.Remove(_selectedRobot.MountIndex);
            }
            catch { }
        }
        public override void Paint(WindowRenderTarget renderTarget)
        {
            if (!_visible)
                return;
            lock (((ICollection)_missionsShape).SyncRoot)
            {
                lock (((ICollection)_underDrawRect2D).SyncRoot)
                {
                    if (_missionsShape.Count == 0 && _underDrawRect2D.Count == 0)
                        return;
                }
            }
            //ProjectCommons.writeConsoleMessage("BABA OMAD DIGE", ConsoleMessageType.Error);
            if (drawingBrush == null)
                drawingBrush = _drawState.Brush_BorderColor;
            if (underDrawBrush == null)
                underDrawBrush = _drawState.Brush_BorderColor;
            if (ProjectCommons.Drawing2D.BaseBody.lastUpdated != ProjectCommons.Drawing2D.lastMapUpdated)
            {
                ProjectCommons.Drawing2D.BaseBody.Initiate();
                recreateShape();
            }
            renderTarget.Transform = ProjectCommons.Drawing2D.changePositionMatrix(0f, 0f);
            lock (((ICollection)_missionsShape).SyncRoot)
            {
                foreach (int key in _missionsShape.Keys)
                {
                    List<Pose2D> curMission = _missionsShape[key].CanvasBody;
                    for (int j = 0; j < curMission.Count; j++)
                    {
                        int k = j - 1;
                        if (k >= 0)
                            renderTarget.DrawLine(drawingBrush, curMission[j], curMission[k]);
                    }
                }
            }
            lock (((ICollection)_underDrawRect2D).SyncRoot)
            {
                int i = 0;
                for (; i < _underDrawRect2D.Count; i++)
                {
                    int j = i - 1;
                    if (j >= 0)
                        renderTarget.DrawLine(drawingBrush, _underDrawRect2D[j], _underDrawRect2D[i]);
                    ProjectCommons.Drawing2D.BaseBody.ellipse.Center = new System.Drawing.PointF((float)_underDrawRect2D[i].X, (float)_underDrawRect2D[i].Y);
                    renderTarget.FillEllipse(underDrawBrush, ProjectCommons.Drawing2D.BaseBody.ellipse);
                }
                if (i > 0)
                    renderTarget.DrawLine(drawingBrush, _underDrawRect2D[i - 1], MouseDataBase.CurrentPoint_Scaled);
            }

        }
        public override void recreateShape()
        {
            lock (((ICollection)_underDrawRect3D).SyncRoot)
            {
                lock (((ICollection)_underDrawRect2D).SyncRoot)
                {
                    _underDrawRect2D.Clear();
                    foreach (Pose2D p in _underDrawRect3D)
                    {
                        _underDrawRect2D.Add(ProjectCommons.Drawing2D.ChangeRealToCanvas(p));
                    }
                }
            }
            lock (((ICollection)_missionsShape).SyncRoot)
            {
                foreach (MissionShape mShape in _missionsShape.Values)
                {
                    mShape.CanvasBody.Clear();
                    foreach (Pose2D p in mShape.RealBody)
                        mShape.CanvasBody.Add(ProjectCommons.Drawing2D.ChangeRealToCanvas(p));
                }
            }
        }
        public void robotAchieveMission(int mIndex, Pose2D currentPoint)
        {
            //ProjectCommons.writeConsoleMessage(" Index:" + mIndex + " Point: " + currentPoint.ToString(), ConsoleMessageType.Information);
            lock (((ICollection)_missionsShape).SyncRoot)
            {
                foreach (MissionShape mShape in _missionsShape.Values)
                {
                    if (mShape.RobotInfo.MountIndex == mIndex)
                    {
                        ProjectCommons.writeConsoleMessage(" Index:" + mIndex + " Point: " + currentPoint.ToString() + " iof:" + mShape.RealBody.IndexOf(currentPoint), ConsoleMessageType.Information);
                        int index = mShape.RealBody.IndexOf(currentPoint);
                        index++;
                        mShape.RealBody.RemoveRange(0, index);
                        mShape.CanvasBody.RemoveRange(0, index);
                        break;
                    }
                }
            }
        }

        #endregion

        #region Property

        public override WidgetTypes Type
        {
            get { return WidgetTypes.Mission; }
        }

        #endregion

        #region events

        public override bool mouseClicked(EventArgs e) { return false; }
        public override bool mouseDClicked(EventArgs e) { return true; }
        public override bool mouseMoved(MouseEventArgs e) { return false; }
        public override bool mouseDown(MouseEventArgs e)
        {
            if (_isDrawing)
            {
                if (e.Button == MouseButtons.Left)
                {
                    lock (((ICollection)_underDrawRect3D).SyncRoot)
                    {
                        _underDrawRect3D.Add(ProjectCommons.Drawing2D.ChangeCanvasToReal(MouseDataBase.CurrentPoint_Scaled));
                    }
                    lock (((ICollection)_underDrawRect2D).SyncRoot)
                    {
                        _underDrawRect2D.Add(new Pose2D(MouseDataBase.CurrentPoint_Scaled));
                    }
                }
                else if (e.Button == MouseButtons.Right)
                    if (_underDrawRect3D.Count > 0)
                    {
                        _underDrawRect3D.RemoveAt(_underDrawRect3D.Count - 1);
                        _underDrawRect2D.RemoveAt(_underDrawRect2D.Count - 1);
                    }
            }
            return false;
        }
        public override bool mouseUp(MouseEventArgs e) { return false; }
        public override bool mouseWheelMoved(MouseEventArgs e) { return true; }

        public override bool keyDown(KeyEventArgs e)
        {
            try
            {
                _selectedRobot = (RobotInfo)MouseData.SelectedObject;
                if (e.KeyCode == _shiftKey && MouseData.SelectedObject != null && !_isDrawing)// && Visible)
                {
                    //if (_missionsShape.ContainsKey(_selectedRobot.MountIndex) && _underDrawRect3D.Count == 0)
                    //{
                    //_underDrawRect3D.AddRange(_missionsShape[_selectedRobot.MountIndex].Body3D);
                    //_underDrawRect2D.AddRange(_missionsShape[_selectedRobot.MountIndex].Body2D);
                    //_missionsShape.Remove(_selectedRobot.MountIndex);
                    //}
                    //else
                    //{
                    //_underDrawRect3D.Add(_selectedRobot.Position3D);
                    //_underDrawRect2D.Add(change3DPoseTo2DPose(_selectedRobot.Position3D));
                    //}
                    if (!(_missionsShape.ContainsKey(_selectedRobot.MountIndex) && _underDrawRect3D.Count == 0))
                    {
                        Pose2D t = new Pose2D(_selectedRobot.GetPosition());

                        _underDrawRect3D.Add(t);
                        _underDrawRect2D.Add(ProjectCommons.Drawing2D.ChangeRealToCanvas(t));
                        //_underDrawRect2D.Add(new Pose2D(10, 10, 10));
                        _isDrawing = true;
                        MouseData.ForceForNoChangeSelectedObject = true;
                        ProjectCommons.keyboardCapturedByGeoViewer = true;
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    List<Pose2D> p = new List<Pose2D>();
                    _missionsShape.Remove(_selectedRobot.MountIndex);
                    ProjectCommons.missionWidget_OnNewListReceived(_selectedRobot.MountIndex, p);
                }
            }
            catch { _selectedRobot = null; }

            return true;
        }
        public override bool keyUp(KeyEventArgs e)
        {
            if (e.KeyCode == _shiftKey)
            {
                _isDrawing = false;
                ProjectCommons.keyboardCapturedByGeoViewer = false;

                if (_underDrawRect3D.Count > 1)
                {
                    MissionShape mShape = new MissionShape();

                    mShape.RobotInfo = new RobotInfo(_selectedRobot);
                    mShape.RealBody = new List<Pose2D>();
                    mShape.CanvasBody = new List<Pose2D>();

                    mShape.RealBody.AddRange(_underDrawRect3D);
                    mShape.CanvasBody.AddRange(_underDrawRect2D);
                    _missionsShape.Add(mShape.RobotInfo.MountIndex, mShape);
                    ProjectCommons.writeConsoleMessage("Shape Created. Point Count :" + mShape.RealBody.Count, ConsoleMessageType.Exclamation);

                    if (ProjectCommons.missionWidget_OnNewListReceived != null)
                    {
                        ProjectCommons.missionWidget_OnNewListReceived(mShape.RobotInfo.MountIndex, mShape.RealBody);

                        ProjectCommons.writeConsoleMessage("MShape Count" + mShape.RealBody.Count, ConsoleMessageType.Exclamation);
                    }
                }

            }
            if (e.KeyCode == _shiftKey || e.KeyCode == Keys.Delete)
            {
                _underDrawRect3D.Clear();
                _underDrawRect2D.Clear();
                MouseData.SelectedObject = null;
                MouseData.ForceForNoChangeSelectedObject = false;
            }

            return true;
        }

        #endregion
    }
}
