using MRL.Components.Tools.Objects;

namespace MRL.Components.Tools.Shapes
{
    public class VictimShape : ShapeBase
    {

        #region Variables

        private Victim _victimInfo;

        #endregion

        #region Constructor

        public VictimShape()
        {
            _classType = "Victim";
        }
        public VictimShape(VictimShape rs)
            : this()
        {
            if (rs.VictimInfo != null)
                this.VictimInfo = rs.VictimInfo;
        }

        #endregion

        #region Methods

        public VictimShape Clone()
        {
            return new VictimShape(this);
        }

        public string Report()
        {
            return "Victim" + _victimInfo.ID + " [" + _victimInfo.Status + "]";
        }

        public override string ToString()
        {
            return this.VictimInfo.NameAndProbablity;
        }

        #endregion

        #region Property

        public Victim VictimInfo
        {
            get { return _victimInfo; }
            set { _victimInfo = value; }
        }
        public override string getHint()
        {
            return _classType + "=ID: " + _victimInfo.ID + " \nName: " + _victimInfo.Name + "\nCanvas Pos :" + CanvasPose.ToString()
                + " \nReal Pos: " + RealPose.ToString() + " \nProb: " + _victimInfo.Probability;
        }
        #endregion

    }
}
