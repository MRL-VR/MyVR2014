using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using MRL.Commons;
using MRL.CustomMath;

namespace MRL.Utils
{

    public class USARConfig
    {
        //added by edris
        public static int mountedRobotsCount = 0;

        //added by mohammad 1387/11/13
        public static int viewerCount = 0;

        public string configFileName;

        public string simHost;
        public int simPort;
        public string videoHost;
        public int videoPort;
        public string wssHost;  
        public int wssPort;



        public List<RobotInfo> botInfo = new List<RobotInfo>(4);
        public List<SonarInfo> sonarInfo = new List<SonarInfo>() {  new SonarInfo() { Index = 0, 
                                                                                     Name = "Sonar1", 
                                                                                     Position = new Pose2D(0.185f,  -0.115f, -0.2f), 
                                                                                     Direction = new Pose2D(0.0f, 0.0f, -1.2216f)}
                                                                                     ,
                                                                  
                                                                    new SonarInfo() { Index = 1, 
                                                                                     Name = "Sonar2", 
                                                                                     Position = new Pose2D(0.22f,-0.080f,-0.2f), 
                                                                                     Direction = new Pose2D(0.0f,0.0f,-0.733f)}
                                                                                     ,
                                                                    new SonarInfo() { Index = 2, 
                                                                                     Name = "Sonar3", 
                                                                                     Position = new Pose2D(0.2232f,-0.0811f,-0.1f), 
                                                                                     Direction = new Pose2D(0.0f,0.0f,-0.4886f)}
                                                                                     ,
                                                                    new SonarInfo() { Index =3, 
                                                                                     Name = "Sonar4", 
                                                                                     Position = new Pose2D(0.24f,-0.025f,-0.1f), 
                                                                                     Direction = new Pose2D(0.0f, 0.0f,-0.2443f)}
                                                                                      ,
                                                                    new SonarInfo() { Index =4, 
                                                                                     Name = "Sonar5", 
                                                                                     Position = new Pose2D(0.24f,0.025f,-0.1f), 
                                                                                     Direction = new Pose2D(0.0f, 0.0f,0.2443)}
                                                                                     ,
                                                                    new SonarInfo() { Index =5, 
                                                                                     Name = "Sonar6", 
                                                                                     Position = new Pose2D(0.2232f,0.0811f,-0.1f), 
                                                                                     Direction = new Pose2D(0.0f,0.0f,0.4886f)}
                                                                                     ,
                                                                    new SonarInfo() { Index =6, 
                                                                                     Name = "Sonar7", 
                                                                                     Position = new Pose2D(0.22f,0.080f,-0.2f), 
                                                                                     Direction = new Pose2D(0.0f,0.0f,0.733f)}
                                                                                     ,
                                                                          
                                                                     new SonarInfo() { Index =7, 
                                                                                     Name = "Sonar8", 
                                                                                     Position = new Pose2D(0.185f, 0.115f, -0.2f), 
                                                                                     Direction = new Pose2D(0.0f, 0.0f, 1.2216f)}
                                                                                     ,
               
                                                                  };


        public List<Vector3> GoalPoints = new List<Vector3>(4);
        public RobotInfo mapperInfo;
        public bool userLog;
        public double wireLink_Safe_Strength;
        public double wireLink_Danger_Strength;
        public ViewerInfo Viewer = new ViewerInfo();

        public int CAM_TILE_X;
        public int CAM_TILE_Y;
        public int MAX_CAM_WIDTH;
        public int MAX_CAM_HEIGHT;
        public int SUB_CAM_WIDTH;
        public int SUB_CAM_HEIGHT;
        public int IMAGE_TRANSFER_TYPE;
        public int OBSTACLE_AVOIDANCE_STATUS;

        public float MAX_ROBOT_SPEED = 2;
        public float MIN_ROBOT_SPEED = -2;

        public float MAX_ROTATION_ROBOT_SPEED = 2;
        public float MIN_ROTATION_ROBOT_SPEED = -2;

        private string path;
        private NameValueCollection usarCfg = new NameValueCollection();

        public USARConfig()
        {
            configFileName = "";
        }
        public USARConfig(string iniPath)
        {
            openConfig(iniPath);
        }

        public void reloadConfig()
        {
            initVars();
            openConfig(configFileName);
        }

        public string getRobotType(string robotName)
        {
            for (int i = 0; i < botInfo.Count; i++)
                if (botInfo[i].Name.Equals(robotName))
                    return botInfo[i].Type;

            return null;
        }
        public string getValue(string name)
        {
            return usarCfg.Get(name);
        }
        public void setValue(string name, string value)
        {
            usarCfg.Set(name, value);
        }
        public void Save()
        {
            string[] str = File.ReadAllLines(path);
            List<string> allStr = new List<string>(str.Length);
            allStr.AddRange(str);
            for (int i = 0; i < allStr.Count; i++)
            {
                string curr = allStr[i];
                if (curr.Contains("#") || string.IsNullOrEmpty(curr.Trim()))
                    continue;
                string t1 = getTag(curr)[0].Trim();

                if (t1.Equals("Robots"))
                {
                    allStr.RemoveAt(i);
                    bool doDelNext = false;
                    while (i < allStr.Count)
                    {
                        if (allStr[i].EndsWith("\\"))
                        {
                            doDelNext = true;
                            allStr.RemoveAt(i);
                            continue;
                        }
                        else if (doDelNext)
                        {
                            allStr.RemoveAt(i);
                            break;
                        }
                        break;
                    }
                    string outPut = t1 + " = ";
                    for (int k = 0; k < this.botInfo.Count; k++)
                    {

                        string detail = this.botInfo[k].ToString().Substring(6, this.botInfo[k].ToString().Length - 6);
                        outPut += detail;
                        if (k != this.botInfo.Count - 1)
                            outPut += ",\\\n";
                    }
                    allStr.Insert(i, outPut);
                    continue;
                }

                if (t1.Equals("Mapper"))
                {
                    allStr.RemoveAt(i);
                    allStr.Insert(i, "Mapper = " + this.mapperInfo.ToString());
                    continue;
                }
                if (t1.Equals("Widget"))
                {
                    allStr.RemoveAt(i);
                    bool doDelNext = false;
                    while (i < allStr.Count)
                    {
                        if (allStr[i].EndsWith("\\"))
                        {
                            doDelNext = true;
                            allStr.RemoveAt(i);
                            continue;
                        }
                        else if (doDelNext)
                        {
                            allStr.RemoveAt(i);
                            break;
                        }
                        break;
                    }
                    allStr.Insert(i, "Widget = " + this.Viewer.ToString().Replace("\\", "\\\n\t\t"));
                    continue;
                }
                if (t1.Equals("GoalPoses"))
                {
                    allStr.RemoveAt(i);
                    string vals = "";
                    int gi = 0;
                    foreach (Vector3 p in GoalPoints)
                    {
                        vals += "Goal" + (++gi) + " " + p.ToString() + " ";
                    }
                    allStr.Insert(i, "GoalPoses = {" + vals.Trim() + "}");
                    continue;
                }


                string value = this.getValue(t1);
                string output = t1 + " = " + value;
                allStr[i] = output;
            }
            File.WriteAllLines(path, allStr.ToArray());
        }

        public string getRobotNameByMountID(int id)
        {
            int arrayID = 0;
            foreach (RobotInfo ri in botInfo)
            {
                if (ri.Spawned)
                {
                    if (id == arrayID) return ri.Name;
                    arrayID++;
                }
            }

            return "None";
        }
        public int getRobotMountIndexByName(string name)
        {
            int arrayID = 0;
            foreach (RobotInfo ri in botInfo)
            {
                if (ri.Spawned)
                {
                    if (name.Equals(ri.Name)) return arrayID;
                    arrayID++;
                }
            }
            return -1;
        }

        public RobotInfo getRobotConfigByName(string name)
        {
            int arrayID = 0;
            foreach (RobotInfo ri in botInfo)
            {
                if (ri.Spawned)
                {
                    if (name.Equals(ri.Name)) return ri;
                    arrayID++;
                }
            }
            return null;
        }

        private void initVars()
        {
            botInfo = new List<RobotInfo>(4);
            usarCfg = new NameValueCollection();
        }
        private void openConfig(string iniPath)
        {
            try
            {
                StreamReader reader = new StreamReader(iniPath);
                usarCfg = ParsingUSARIni(reader);
                path = iniPath;
                reader.Close();

                configFileName = iniPath;

                simHost = usarCfg.Get("SimHost");
                simPort = int.Parse(usarCfg.Get("SimPort"));

                wssHost = usarCfg.Get("WSSHost");
                wssPort = int.Parse(usarCfg.Get("WSSPort"));

                videoHost = usarCfg.Get("VideoHost");
                videoPort = int.Parse(usarCfg.Get("VideoPort"));

                CAM_TILE_X = int.Parse(usarCfg.Get("CamTileX"));
                CAM_TILE_Y = int.Parse(usarCfg.Get("CamTileY"));
                MAX_CAM_WIDTH = int.Parse(usarCfg.Get("MaxCamWidth"));
                MAX_CAM_HEIGHT = int.Parse(usarCfg.Get("MaxCamHeight"));
                SUB_CAM_WIDTH = int.Parse(usarCfg.Get("SubCamWidth"));
                SUB_CAM_HEIGHT = int.Parse(usarCfg.Get("SubCamHeight"));
                IMAGE_TRANSFER_TYPE = int.Parse(usarCfg.Get("ImageTransferType"));
                IMAGE_TRANSFER_TYPE = int.Parse(usarCfg.Get("ImageTransferType"));
                OBSTACLE_AVOIDANCE_STATUS = int.Parse(usarCfg.Get("ObstacleAvoidanceStatus"));


                string goalPoses = usarCfg.Get("GoalPoses");
                USARParser gp = new USARParser("GOAL " + goalPoses.Trim());
                NameValueCollection gv = gp.getSegment(0);

                for (int i = 0; i < gv.Count; i++)
                {
                    GoalPoints.Add(new Vector3(gv.Get(i)));
                }


                string[] robots = usarCfg.Get("Robots").Split("\\".ToCharArray());
                botInfo.AddRange(robots.Select(r => RobotInfo.parseInfo(r)));

                mapperInfo = RobotInfo.parseInfo(usarCfg.Get("Mapper"));

                wireLink_Safe_Strength = double.Parse(usarCfg.Get("Link_Safe_Strength"));
                wireLink_Danger_Strength = double.Parse(usarCfg.Get("Link_Danger_Strength"));

                userLog = bool.Parse(usarCfg.Get("UserLog"));

                //Set widget settings
                string[] widgets = usarCfg.Get("widget").Split("\\".ToCharArray());
                for (int i = 0; i < widgets.Length; i++)
                {
                    string cur = widgets[i].Trim();

                    if (cur.StartsWith("Annotation")) { Viewer.AnnotationWidget = DrawState.parseInfo(cur); continue; }
                    if (cur.StartsWith("Image")) { Viewer.ImageWidget = DrawState.parseInfo(cur); continue; }
                    if (cur.StartsWith("Laser")) { Viewer.LaserWidget = DrawState.parseInfo(cur); continue; }
                    if (cur.StartsWith("Map")) { Viewer.MapWidget = DrawState.parseInfo(cur); continue; }
                    if (cur.StartsWith("Mission")) { Viewer.MissionWidget = DrawState.parseInfo(cur); continue; }
                    if (cur.StartsWith("RobotPath")) { Viewer.RobotPathWidget = DrawState.parseInfo(cur); continue; }
                    if (cur.StartsWith("Robot")) { Viewer.RobotWidget = DrawState.parseInfo(cur); continue; }
                    if (cur.StartsWith("ScaleBar")) { Viewer.ScaleBarWidget = DrawState.parseInfo(cur); continue; }
                    if (cur.StartsWith("Victim")) { Viewer.VictimWidget = DrawState.parseInfo(cur); continue; }
                }
            }
            catch (Exception e)
            {
                USARLog.println(e.Message, 1, "USARConfig");
            }
        }

        private NameValueCollection ParsingUSARIni(StreamReader reader)
        {
            NameValueCollection nvc = new NameValueCollection();

            int i = 0;
            while (!reader.EndOfStream)
            {
                string l = reader.ReadLine();
                if (l.Contains("#") || string.IsNullOrEmpty(l.Trim())) continue;

                l = l.Trim();
                while (l.EndsWith("\\"))
                {
                    l += reader.ReadLine();
                }
                i++;
                string[] terms = l.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                nvc.Add(terms[0].Trim(), terms[1].Trim());
            }

            return nvc;
        }
        private string[] getTag(string str)
        {
            return str.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        }

        internal int getMountedRobotsCount()
        {
            if (mountedRobotsCount == 0)
            {
                foreach (RobotInfo ri in botInfo)
                {
                    if (ri.Spawned)
                        mountedRobotsCount++;
                }

                return mountedRobotsCount;
            }
            else
                return mountedRobotsCount;
        }
    }

}
