using System;
using System.Drawing;
using MRL.CustomMath;

namespace MRL.Components.Tools.Shapes
{
    public abstract class ShapeBase
    {
     
        #region Variables

        public Point Center;
        public Pose3D RealPose;
        public Pose2D CanvasPose;
        protected String _classType;

        #endregion

        #region Constructor

        public ShapeBase()
        {
            Center = new Point(0, 0);
           

            CanvasPose = new Pose2D();
            RealPose = new Pose3D();
        }

        #endregion

        #region Methods

        public abstract string getHint();
    
     
        #endregion
    }
}
