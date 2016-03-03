using System;

using MRL.CustomMath;

namespace MRL.Exploration.GridSLAM.Base
{
    class MotionModel
    {
        #region Private Members

        private double _tErr1 = 0.00001;
        private double _tErr2 = 0.00001;
        private double _rErr1 = 0.00001;
        private double _rErr2 = 0.00001;
        private double _dRot1;
        private double _dRot2;
        private double _dTrans;
        #endregion

        public Pose2D _odoState;
        public Pose2D _odoPrevState;

        public Pose2D SetOdoState
        {
            get
            {
                return _odoState;
            }
            set
            {
                _odoState = value;
            }
        }


        public Pose2D SetPrevState
        {
            get
            {
                return _odoPrevState;
            }
            set
            {
                _odoPrevState = value;
            }
        }

        public MotionModel()
        {
        }


        public Pose2D GetSample(Pose2D PrevState, Pose2D State)
        {
            _dRot1 = Math.Atan2(State.Y - PrevState.Y, State.X - PrevState.X) - PrevState.Rotation;
            _dRot1 = LimitTheta(_dRot1);

            _dTrans =
                Math.Sqrt(Math.Pow(State.Y - PrevState.Y, 2) + Math.Pow(State.X - PrevState.X, 2));
            _dRot2 = State.Rotation - PrevState.Rotation - _dRot1;

            _dRot2 = LimitTheta(_dRot2);


            double dHatRot1 = _dRot1 - ProbabilityUtils.RandomFromGaussian(_rErr1 * Math.Abs(_dRot1) + _tErr1 * _dTrans);
            double dHatTrans = _dTrans -
                               ProbabilityUtils.RandomFromGaussian(_tErr2 * _dTrans + _rErr2 * (Math.Abs(_dRot1) + Math.Abs(_dRot2)));
            double dHatRot2 = _dRot2 - ProbabilityUtils.RandomFromGaussian(_rErr1 * Math.Abs(_dRot2) + _tErr1 * _dTrans);

            double x = State.X + dHatTrans * Math.Cos(State.Rotation + dHatRot1);
            double y = State.Y + dHatTrans * Math.Sin(State.Rotation + dHatRot1);
            double theta = State.Rotation + dHatRot1 + dHatRot2;
            theta = LimitTheta(theta);


            return new Pose2D(x, y, theta);
        }

        private static double LimitTheta(double theta)
        {
            while (theta > Math.PI) { theta -= Math.PI * 2; };
            while (theta < -Math.PI) { theta += Math.PI * 2; };
            return theta;
        }

        #region IMotionModel Members

        /// <summary>
        /// For the odometry model, this returns the state you give it!
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public Pose2D GetPerfectSample(Pose2D state)
        {
            return state;
        }

        /// <summary>
        /// Returns 'Odometry'
        /// </summary>
        /// <returns></returns>
        public string ModelType()
        {
            return "Odometry";
        }



        #endregion

    }
}




