using System.Collections.Generic;

using MRL.Components.Tools.Objects;
using MRL.CustomMath;

namespace MRL.Components.Tools.Shapes
{
    public class AnnotationShape : ShapeBase
    {
        private AnnotationState _annotationInfo;
        private List<Pose2D> _polysReal = new List<Pose2D>();
        private List<Pose2D> _polysCanvas = new List<Pose2D>();

        public AnnotationShape()
        {
            _classType = "Annotation";
        }
        public AnnotationState AnnotationInfo
        {
            get { return _annotationInfo; }
            set { _annotationInfo = value; }
        }
        public List<Pose2D> RealBody
        {
            get { return _polysReal; }
            set { _polysReal = value; }
        }
        public List<Pose2D> CanvasBody
        {
            get { return _polysCanvas; }
            set { _polysCanvas = value; }
        }

        public override string getHint()
        {
            return _classType + "=ID: " + _annotationInfo.ID + "\nDescription: " + _annotationInfo.Description;
        }
    }
}