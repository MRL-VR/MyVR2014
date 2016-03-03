using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Components.Tools.Shapes
{
  public  class TargetShape : ShapeBase
    {
        #region Variables

        public RobotInfo RobotInfo;
        public Pose2D TargetPosition;
        #endregion

        #region Constructor

        public TargetShape()
        {
            _classType = "Target";
        }

        public TargetShape(TargetShape ts)
            : this()
        {
            if (RobotInfo != null && TargetPosition != null)
            {
                this.RobotInfo = ts.RobotInfo;
                this.TargetPosition = ts.TargetPosition;
            }
        }
        #endregion

        #region Methods

        public TargetShape Clone()
        {
            return new TargetShape(this);
        }
        public override string getHint()
        {
            return _classType + "=ID: " + RobotInfo.MountIndex + "\nName: " + RobotInfo.Name + "\nType: " + RobotInfo.Type
                              + "\nPosition: " + TargetPosition.ToString();
        }

        #endregion
    }
}
