using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using MRL.Commons;
using MRL.Components.Tools.Objects;
using MRL.Components.Tools.Shapes;
using MRL.CustomMath;
using MRL.Mapping;
using MRL.Utils;

namespace MRL.Components
{
    public partial class GeoreferenceImageViewer
    {
        #region SaveResult

        private void setResultDirectory()
        {
            string path = Directory.GetCurrentDirectory();
            if (!path.EndsWith("\\")) path += "\\";

            path += "RESULTS\\";

            int dirNumber = 0;

            string saved = path;
            while (true)
            {
                path = saved;
                path += "LEVEL_RESULT_" + dirNumber + "\\";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    break;
                }
                else
                    dirNumber++;
            }

            ProjectCommons.currentResultPath = path;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SaveResult(object arg)
        {
            try
            {
                // Create date format and filename
                DateTime df = DateTime.Now;
                String filename = ProjectCommons.currentResultPath + "usar_" + df.ToString(dateFormat);
                ProjectCommons.writeConsoleMessage("Saving data to file prefix : " + filename, ConsoleMessageType.Normal);

                //List<AnnotationShape> annotationData = _annotationWidget.TotalShape;
                List<VictimShape> victimData = _victimWidget.TotalVictims;
                List<RobotShape> allBots = _robotWidget.TotalRobot;
                List<RobotPath> pathData = _robotPathWidget.TotalPath;

                // Save the victim file
                bool res = exportVictimFile(filename + "_victim", victimData);
                ProjectCommons.writeConsoleMessage("Done saving victims to file : " + res, ConsoleMessageType.Normal);

                // Save the additional info
                if (victimData.Count > 0)
                {
                    res = exportVictimInfoFile(filename + "_victim_info", victimData) && res;
                    ProjectCommons.writeConsoleMessage("Done saving victim info to file : " + res, ConsoleMessageType.Normal);
                }

                // Save victim map info
                if (victimData.Count > 0)
                {
                    res = exportVictimMapInfo(filename + "_victim", victimData) && res;
                    ProjectCommons.writeConsoleMessage("Done saving victim map info to file : " + res, ConsoleMessageType.Normal);
                }

                // Save robots path
                res = exportPathInfoFile(filename + "_path", pathData) && res;
                ProjectCommons.writeConsoleMessage("Done saving robot tracks to file : " + res, ConsoleMessageType.Normal);

                // Save shape info
                //if (annotationData.Count > 0)
                //{
                //    res = exportShapeInfoFile(filename + "_shape", annotationData) && res;
                //    ProjectCommons.writeConsoleMessage("Done saving shape map to file : " + res, ConsolMessageType.Normal);
                //}

                // Save the map to file
                IEGMap _clonedMap = _worldMap.Clone();

                if (_clonedMap != null)
                {
                    res = _clonedMap.exportGeoTiff(filename + "_map", victimData, true) && res;
                    ProjectCommons.writeConsoleMessage("Done saving map to file : " + res, ConsoleMessageType.Normal);

                    // Save probabilistic map info
                    res = _clonedMap.exportGeoTiff(filename + "_map_prob", victimData, false) && res;
                    ProjectCommons.writeConsoleMessage("Done saving probability map to file : " + res, ConsoleMessageType.Normal);
                }

                // Indicate that the save completed
                if (res)
                    ProjectCommons.writeConsoleMessage("Completed saving", ConsoleMessageType.Exclamation);
                else
                    ProjectCommons.writeConsoleMessage("Saving Operations Not Completed", ConsoleMessageType.Error);
            }
            catch(Exception ex)
            {
                ProjectCommons.writeConsoleMessage(ex.ToString(), ConsoleMessageType.Error);
            }
        }

        private bool exportShapeInfoFile(string fName, List<AnnotationShape> annotationData)
        {
            return false;
        }

        private bool exportPathInfoFile(string fName, List<RobotPath> pathData)
        {
            TrackMapInfo tmi = new TrackMapInfo(pathData);
            return tmi.exportPathInfoFile(fName );
        }

        private bool exportVictimMapInfo(string fName, List<VictimShape> vShapes)
        {
            VictimMapInfo vmi = new VictimMapInfo(vShapes);
            return vmi.exportVictimInfoFile(fName);
        }

        private bool exportVictimFile(string fName, List<VictimShape> vShapes)
        {
            try
            {
                StreamWriter sw = new StreamWriter(fName + ".txt");

                foreach (VictimShape vShape in vShapes)
                {
                    Victim v = vShape.VictimInfo;
                    sw.WriteLine(string.Format("Victim{0}, {1}, {2}, {3}, {4}, {5}",
                                    v.ID, vShape.RealPose.X, vShape.RealPose.Y, 0, "VICTIM", 1));
                }
                sw.WriteLine("END");
                sw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool exportVictimInfoFile(string fName, List<VictimShape> vShapes)
        {
            try
            {
                StreamWriter bw = new StreamWriter(fName + ".txt");

                foreach (VictimShape vShape in vShapes)
                {
                    Victim v = vShape.VictimInfo;
                    bw.WriteLine(string.Format("Victim{0}, {1}", v.ID, v.Status));
                }

                // Shutdown the file and return success
                bw.Flush();
                bw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    internal class TrackMapInfo
    {
        private int version = 450;
        private String charset = "Neutral";
        private String delimiter = ",";
        private PointF minPoint = new PointF(float.MaxValue, float.MaxValue);
        private PointF maxPoint = new PointF(float.MinValue, float.MinValue);
        private String[] columns = { "ID integer", "Name char(128)" };
        private List<int> ids = new List<int>();
        private List<String> dess = new List<String>();
        private List<PLine> paths = new List<PLine>();
        List<Color> robotColors = new List<Color>() { Color.Yellow, Color.Red, Color.Green, 
                                                      Color.Blue, Color.Cyan, Color.Magenta, 
                                                      Color.Brown, Color.Purple };
        private int count = 1;

        public TrackMapInfo(List<RobotPath> pathData)
        {
            for (int i = 0; i < pathData.Count; i++)
                addPath(pathData[i]);
        }

        public void addPath(RobotPath rs)
        {
            ids.Add(count++);
            dess.Add(rs.RobotInfo.Name);
            PLine pl = new PLine(rs.RealPath);
            pl.setPen(4, 2, robotColors[rs.RobotInfo.MountIndex].ToArgb() & 0x00ffffff);
            paths.Add(pl);
            PointF maxP = pl.getMaxPoint();
            PointF minP = pl.getMinPoint();
            if (maxP.X > maxPoint.X)
                maxPoint.X = maxP.X;
            if (minP.X < minPoint.X)
                minPoint.X = minP.X;
            if (maxP.Y > maxPoint.Y)
                maxPoint.Y = maxP.Y;
            if (minP.Y < minPoint.Y)
                minPoint.Y = minP.Y;
        }

        public bool exportPathInfoFile(String fName)
        {
            try
            {
                StreamWriter bw = new StreamWriter(fName + ".mid");
                int count = ids.Count;
                for (int i = 0; i < count; i++)
                    bw.WriteLine("" + ids[i] + ",\"" + dess[i] + "\"");
                bw.Close();
                bw = new StreamWriter(fName + ".mif");
                bw.WriteLine("Version " + version);
                bw.WriteLine("Charset \"" + charset + "\"");
                bw.WriteLine("Delimiter \"" + delimiter + "\"");
                bw.WriteLine("CoordSys NonEarth Units \"m\" Bounds " +
                    "(" + minPoint.X + ", " + minPoint.Y + ") " + "" + "(" + maxPoint.X + ", " + maxPoint.Y + ")");
                bw.WriteLine("Columns " + columns.Length);
                for (int i = 0; i < columns.Length; i++)
                    bw.WriteLine("  " + columns[i]);
                bw.WriteLine("Data");
                bw.WriteLine("");
                foreach (PLine p in paths)
                {
                    bw.WriteLine(p.ToString());
                }
                bw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    internal class PLine
    {
        List<PointF> lines = new List<PointF>();
        int[] pen = null; // width, pattern, color
        int[] brush = null; // pattern, forecolor backcolor
        private PointF minPoint = new PointF(float.MaxValue, float.MaxValue);
        private PointF maxPoint = new PointF(float.MinValue, float.MinValue);

        public PLine() { }

        public PLine(List<Pose2D> al)
        {
            Pose2D p = null;
            int count = al.Count;
            for (int i = 0; i < count; i++)
            {
                p = al[i];
                addPoint(p);
            }
        }

        public void addPoint(PointF p)
        {
            lines.Add(p);

            if (p.X > maxPoint.X)
                maxPoint.X = p.X;
            if (p.X < minPoint.X)
                minPoint.X = p.X;
            if (p.Y > maxPoint.Y)
                maxPoint.Y = p.Y;
            if (p.Y < minPoint.Y)
                minPoint.Y = p.Y;
        }

        public void addPoint(Pose2D p)
        {
            this.addPoint((PointF)p);
        }

        public PointF getMaxPoint()
        {
            return maxPoint;
        }

        public PointF getMinPoint()
        {
            return minPoint;
        }

        public void setPen(int width, int pattern, int color)
        {
            pen = new int[3];
            pen[0] = width;
            pen[1] = pattern;
            pen[2] = color;
        }

        public void setBrush(int pattern, int fColor, int bColor)
        {
            brush = new int[3];
            brush[0] = pattern;
            brush[1] = fColor;
            brush[2] = bColor;
        }

        public override String ToString()
        {
            String res = "pline " + lines.Count + "\n";
            foreach (PointF p in lines)
                res += p.Y + " " + p.X + "\n";
            if (pen != null)
                res += "  Pen(" + pen[0] + ", " + pen[1] + ", " + pen[2] + ")" + "\n";
            if (brush != null)
                res += "  Brush(" + brush[0] + ", " + brush[1] + ", " + brush[2] + ")" + "\n";
            return res;
        }
    }

    internal class VictimMapInfo
    {
        private int version = 450;
        private String charset = "Neutral";
        private String delimiter = ",";
        private PointF minPoint = new PointF(float.MaxValue, float.MaxValue);
        private PointF maxPoint = new PointF(float.MinValue, float.MinValue);
        private String[] columns = { "ID integer", "Name char(128)" };
        private List<int> ids = new List<int>();
        private List<String> dess = new List<String>();
        private List<PointF> locs = new List<PointF>();
        private int[] symbol = null;

        private int count = 1;

        /** Creates a new instance of TrackMIF */
        public VictimMapInfo()
        {

        }

        public VictimMapInfo(List<VictimShape> vShapes)
        {
            int count = vShapes.Count;
            for (int i = 0; i < count; i++)
                addVictim(vShapes[i]);
            setSymbol(56, 0xff0000, 16);
        }

        public void addVictim(VictimShape vShape)
        {
            ids.Add(count++);
            String des = vShape.Report().Replace(' ', '_');
            dess.Add(des);
            PointF p = new PointF((float)vShape.RealPose.Y, (float)vShape.RealPose.X);
            locs.Add(new PointF(p.X, p.Y));
            if (p.X > maxPoint.X)
                maxPoint.X = p.X;
            if (p.X < minPoint.X)
                minPoint.X = p.X;
            if (p.Y > maxPoint.Y)
                maxPoint.Y = p.Y;
            if (p.Y < minPoint.Y)
                minPoint.Y = p.Y;
        }

        public void setSymbol(int shape, int color, int size)
        {
            symbol = new int[3];
            symbol[0] = shape;
            symbol[1] = color;
            symbol[2] = size;
        }

        public bool exportVictimInfoFile(String fName)
        {
            try
            {
                StreamWriter sw = new StreamWriter(fName + ".mid");
                for (int i = 0; i < ids.Count; i++)
                    sw.WriteLine("" + ids[i] + ",\"" + dess[i] + "\"");
                sw.Close();
                sw = new StreamWriter(fName + ".mif");
                sw.WriteLine("Version " + version);
                sw.WriteLine("Charset \"" + charset + "\"");
                sw.WriteLine("Delimiter \"" + delimiter + "\"");
                sw.WriteLine("CoordSys NonEarth Units \"m\" Bounds " +
                            "(" + minPoint.X + ", " + minPoint.Y + ") " + "" + "(" + maxPoint.X + ", " + maxPoint.Y + ")");
                sw.WriteLine("Columns " + columns.Length);
                for (int i = 0; i < columns.Length; i++)
                    sw.WriteLine("  " + columns[i]);
                sw.WriteLine("Data");
                sw.WriteLine("");
                foreach (PointF p in locs)
                {
                    sw.WriteLine("Point " + p.X + " " + p.Y);
                    if (symbol != null)
                        sw.WriteLine("  Symbol(" + symbol[0] + ", " + symbol[1] + ", " + symbol[2] + ")");
                }
                sw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }


}
