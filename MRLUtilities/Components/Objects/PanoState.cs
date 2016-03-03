//using System.Drawing;


namespace MRL.Components.Tools.Objects
{
    public class PanoState
    {
        private System.Drawing.Bitmap _image;
        private string _source;
        private double _time;
        private volatile bool _viewed;
        private int _id;
        public PanoState()
        {
            //this.pose = ib.getPose();
            //this.fov = ib.getFOV();
            //this.cursor = new Point(0, 0);
            //this.imageSize = new Rectangle();
            //this.source = ib.getRobotID();
            //this.time = ib.getTime();
            //this.image = ib.getImage();
            //this.viewed = new AtomicBoolean(false);
        }
        public PanoState(int id)
        {
            this.ID = id;
        }
        #region Property
        public System.Drawing.Bitmap Image
        {
            get { return _image; }
            set { _image = value; }
        }
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }
        public double Time
        {
            get { return _time; }
            set { _time = value; }
        }
        public bool Viewed
        {
            get { return _viewed; }
            set { _viewed = value; }
        }
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        #endregion
    }
}
