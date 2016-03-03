using MRL.CustomMath;

namespace MRL.Components.Tools.Shapes
{
    public class GoalPointShape : ShapeBase
    {
        public GoalPointShape()
        {
            _classType = "GoalPoint";
        }
        public GoalPointShape(Pose3D RealPose)
            : this()
        {
            this.RealPose = RealPose;
        }
        public GoalPointShape(GoalPointShape gps)
            : this()
        {
            if (gps == null)
                return;

            this.CanvasPose = gps.CanvasPose;
            this.RealPose = gps.RealPose;
        }
        public override string getHint()
        {
            return _classType + "=\nCanvas Pos: " + CanvasPose.ToString() + "\nReal Pos: " + RealPose.ToString();
        }
    }
}
