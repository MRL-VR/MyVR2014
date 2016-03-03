using MRL.Utils;

namespace MRL.Components.Tools.Shapes
{
    public class  RobotShape : ShapeBase
    {
        #region Variables

        public RobotInfo RobotInfo;

        #endregion

        #region Constructor

        public RobotShape()
        {
            _classType = "Robot";
        }
        public RobotShape(RobotShape rs):this()
        {
            if (rs.RobotInfo != null)
                this.RobotInfo = rs.RobotInfo;
        }

        #endregion

        #region Methods

        public RobotShape Clone()
        {
            return new RobotShape(this);
        }
        public override string getHint()
        {
            return _classType +  "=ID: " + RobotInfo.MountIndex + "\nName: " + RobotInfo.Name + "\nType: " + RobotInfo.Type 
                + "\nCanvas Pos: " + CanvasPose.ToString() + "\nReal Pos: " + RealPose.ToString()
                + "\nRobot IP: " + RobotInfo.RobotIP + "\nRobot Port: " + RobotInfo.RobotPort ;
        }
        #endregion


    }
}
