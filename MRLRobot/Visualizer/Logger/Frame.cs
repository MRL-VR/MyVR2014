using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.CustomMath;
using MRL.Communication.External_Commands;
using System.Xml.Serialization;
using MRL.Utils;
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Drawing.Imaging;
using MRL.Mapping;

namespace MRLRobot
{
    [Serializable]
    public class Frames
    {
        [XmlIgnore]
        public static Frames Instance = new Frames();

        public Frame frame = new Frame();
     

        public void SetFrame(Pose2D currRobotPosition, SonarManager sonarsList, Laser laser, List<SonarInfo> sonarinfo, Bitmap map)
        {           
            frame = (new Frame(currRobotPosition, sonarsList, laser, sonarinfo));
        }

        public void Save(string FileAdr)
        {
            this.frame.sonar.BeforSaveToXml();

            System.Xml.Serialization.XmlSerializer writer =
              new System.Xml.Serialization.XmlSerializer(typeof(Frames));

            System.IO.StreamWriter file = new System.IO.StreamWriter(FileAdr);
            writer.Serialize(file, this);
            file.Close();
        }

        public void Load(string FileAdr)
        {
            System.Xml.Serialization.XmlSerializer reader =
              new System.Xml.Serialization.XmlSerializer(typeof(Frames));

            System.IO.StreamReader file = new System.IO.StreamReader(FileAdr);
            Frames obj = new Frames();
            obj = (Frames)reader.Deserialize(file);
            this.frame = obj.frame;
            frame.sonar.AfterLoadFromXml();
            file.Close();
        }
    }

    [Serializable]
    public class Frame
    {
        public Frame()
        {

        }

        public Frame(Pose2D currRobotPosition, SonarManager sonarsList, Laser laser, List<SonarInfo> sonarinfo)
        {
            this.currRobotPosition = currRobotPosition;
            this.sonar = sonarsList;
            this.laser = laser;
            this.sonarinfo = sonarinfo;
        }

        public List<SonarInfo> sonarinfo;
        public Pose2D currRobotPosition;
        public SonarManager sonar;
        public Laser laser;
    }
}
