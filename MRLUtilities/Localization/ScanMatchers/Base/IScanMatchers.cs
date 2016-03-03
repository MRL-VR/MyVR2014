using MRL.CustomMath;

namespace MRL.Exploration.ScanMatchers.Base
{
    public interface IScanMatchers
    {
        MatchResult Match(ScanObservation scan1, ScanObservation scan2, Pose2D initQ);
    }
}
