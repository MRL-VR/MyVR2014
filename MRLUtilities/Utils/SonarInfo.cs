using MRL.CustomMath;

namespace MRL.Utils
{
    public class SonarInfo
    {
        public SonarInfo()
        {

        }
        public int Index { set; get; }
        public string Name { set; get; }
        public Pose2D Position { set; get; }
        public Pose2D Direction { set; get; }
    }
}
