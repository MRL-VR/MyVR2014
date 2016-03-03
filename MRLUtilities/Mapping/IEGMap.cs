using System.Collections.Generic;
using MRL.Commons;
using MRL.Components.Tools.Shapes;
using MRL.CustomMath;

namespace MRL.Mapping
{

    public interface IEGMap
    {
        bool exportGeoTiff(string fName, List<VictimShape> victimData, bool bBinary);
        event ProjectCommons._geoRefrencedMap_Updated geoReferenceMap_Updated;
        Pose2D TransposeRealToCanvas(Pose2D p);
        Pose2D TransposeCanvas2Real(Pose2D p);

        IEGMap Clone();
    }

}
