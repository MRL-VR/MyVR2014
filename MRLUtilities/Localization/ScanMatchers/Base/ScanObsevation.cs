using System;
using MRL.Commons;

namespace MRL.Exploration.ScanMatchers.Base
{

    [Serializable]
    public class ScanObservation
    {
        // Fields
        private float _Factor;
        private ILaserRangeData _RangeData;

        // Methods
        public ScanObservation(float factor, ILaserRangeData rangeData)
        {
            this._Factor = factor;
            this._RangeData = rangeData;
        }

        // Properties
        public float Factor
        {
            get
            {
                return this._Factor;
            }
        }

        public int Length
        {
            get
            {
                return this.RangeScanner.Range.Length;
            }
        }

        public ILaserRangeData RangeScanner
        {
            get
            {
                return this._RangeData;
            }
        }
    }

}
