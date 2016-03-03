
using MRL.Components.Tools.Objects;

namespace MRL.Components.Tools.Shapes
{
    public class ImageShape : ShapeBase
    {
        private PanoState _ImageInfo;
        public volatile bool isViewed = false;
        public ImageShape()
        {
            _classType = "Image";
        }
        public ImageShape(ImageShape iShape):this()
        {
            if (iShape.ImageInfo != null)
                this.ImageInfo = iShape.ImageInfo;
            //this.Body = iShape.Body;
        }
        public PanoState ImageInfo
        {
            get { return _ImageInfo; }
            set { _ImageInfo = value; }
        }
        public ImageShape Clone()
        {
            return new ImageShape(this);
        }
        public override string getHint()
        {
            return _classType + "=Took by: " + _ImageInfo.Source + "\nPos3D: " + RealPose.ToString() + "\nViewed: " + _ImageInfo.Viewed;
        }
    }
}
