
using MRL.Commons;

namespace MRL.Utils
{

    public class ViewerInfo
    {
        public DrawState AnnotationWidget = new DrawState();
        public DrawState ImageWidget = new DrawState();
        public DrawState LaserWidget = new DrawState();
        public DrawState MapWidget = new DrawState();
        public DrawState MissionWidget = new DrawState();
        public DrawState RobotPathWidget = new DrawState();
        public DrawState RobotWidget = new DrawState();
        public DrawState ScaleBarWidget = new DrawState();
        public DrawState VictimWidget = new DrawState();
        public DrawState SignalWidget = new DrawState();
        public override string ToString()
        {
            return
                "Robot" + RobotWidget.ToString() + "\\" +
                "Annotation" + AnnotationWidget.ToString() + "\\" +
                "Image" + ImageWidget.ToString() + "\\" +
                "RobotPath" + RobotPathWidget.ToString() + "\\" +
                "Laser" + LaserWidget.ToString() + "\\" +
                "Mission" + MissionWidget.ToString() + "\\" +
                "Victim" + VictimWidget.ToString();
        }
    }

}
