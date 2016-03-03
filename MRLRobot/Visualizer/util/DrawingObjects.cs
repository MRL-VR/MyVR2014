using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Drawing;
using MRL.Utils.GMath;

namespace VisualizerLibrary
{
    public static class DrawingObjects
    {
        private static bool _ShowMap = false;
        public static bool ShowMap { get { return _ShowMap; } set { _ShowMap = value; } }

        public static Dictionary<string, object> drawingObject = new Dictionary<string, object>();

        public static bool AddObject(string Key, object obj)
        {
            try
            {
                lock (drawingObject)
                {
                    if (!drawingObject.ContainsKey(Key))
                    {
                        drawingObject.Add(Key, obj);
                        return true;
                    }
                    else if (drawingObject[Key].GetType() == obj.GetType())
                    {
                        drawingObject[Key] = obj;
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }

    public class RobotModel
    {
        public RobotModel()
        {

        }

        public RobotModel(bool _haveSonar, bool _haveLaser, bool _havePositionText)
        {
            haveSonar = _haveSonar;
            haveLaser = _haveLaser;
            havePositionText = _havePositionText;
        }

        public Position2D position { get; set; }

        public bool haveSonar { get; set; }
        public bool haveLaser { get; set; }
        public bool havePositionText { get; set; }

        public List<Position2D> Laser { get; set; }
        public List<Position2D> sonar { get; set; }
    }

    public class SonarModel
    {
        public SonarModel(Color _SonarColor, Color _TextColor, List<Line> _Sonar, List<StringDraw> _SonarText)
        {
            SonarColor = _SonarColor;
            TextColor = _TextColor;
            Sonar = _Sonar;
            SonarText = _SonarText;
        }

        public Color SonarColor { get; set; }
        public Color TextColor { get; set; }
        public List<Line> Sonar { get; set; }
        public List<StringDraw> SonarText { get; set; }

    }

    public class LaserModel
    {
        public LaserModel()
        {

        }

        public LaserModel(List<Position2D> _LaserScan, Position2D _RobotPosition, Color _color)
        {
            LaserScan = _LaserScan;
            RobotPosition = _RobotPosition;
            color = _color;
        }

        public Color color { get; set; }
        public List<Position2D> LaserScan { get; set; }
        public Position2D RobotPosition { get; set; }
    }
}
