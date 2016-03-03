using System.Drawing;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace MRL.ImageProcessor
{
    public class ImageFilterings
    {
        private ContrastCorrection cFilter = new ContrastCorrection();
        private BrightnessCorrection bThinFilter = new BrightnessCorrection(); // thin
        private BrightnessCorrection bThickFilter = new BrightnessCorrection(); // thick
        private LevelsLinear levelsLinearilter = new LevelsLinear();
        private Sharpen sharpenFilter = new Sharpen();
        private GammaCorrection gammaFilter = new GammaCorrection();
        private FiltersSequence Filters = new FiltersSequence();

        private static ImageFilterings _Instance = new ImageFilterings();

        public static ImageFilterings Instance { get { return _Instance; } }
        public bool Enabled { get; set; }

        private ImageFilterings()
        {
            cFilter.Factor = 1;
            bThinFilter.AdjustValue = 0;
            Enabled = false;
        }

        public double ContrastFactor
        {
            get { return cFilter.Factor; }
            set { cFilter.Factor = (float)value; }
        }

        public double BrightnessFactor
        {
            get { return bThinFilter.AdjustValue; }
            set { bThinFilter.AdjustValue = (float)value; }
        }

        public Bitmap Apply(Bitmap p)
        {
            if (p == null || !Enabled)
                return p;
            return Filters.Apply(p);
        }

        private void Normal(Bitmap image)
        {
            if (image == null)
                return;
            ImageStatistics statistics = new ImageStatistics(image);
            levelsLinearilter.InRed = statistics.Red.GetRange(0.9);
            levelsLinearilter.InGreen = statistics.Green.GetRange(0.9);
            levelsLinearilter.InBlue = statistics.Blue.GetRange(0.9);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="BrightnessThin">0-100</param>
        /// <param name="BrightnessThick">0-100</param>
        /// <param name="Contrast">0-100</param>
        /// <param name="Gamma">0-100</param>
        public void SetParameters(int BrightnessThin, int BrightnessThick, int Contrast, int Gamma)
        {
            bThinFilter.AdjustValue = BrightnessThin / 100F;
            bThickFilter.AdjustValue = BrightnessThick / 100F;
        }

        private void SetFilters(params IFilter[] F)
        {
            Filters.Clear();
            foreach (var f in F)
                Filters.Add(f);
        }

        public void BlackSmoke()
        {
            SetFilters(levelsLinearilter, sharpenFilter);
        }

        public void ThinHaze(int value = -6)
        {
            bThinFilter.AdjustValue = value / 100F;
            SetFilters(bThinFilter, levelsLinearilter);
        }

        public void ThickHaze(int value = -6)
        {
            bThickFilter.AdjustValue = value / 100F;
            SetFilters(levelsLinearilter, bThickFilter);
        }

        public void All()
        {
            SetFilters(levelsLinearilter);
        }

        public void BlakeSecond(int value = 400)
        {
            gammaFilter.Gamma = value / 100F;
            SetFilters(gammaFilter, sharpenFilter);
        }

        public void ThinSmoke(int value = 220)
        {
            cFilter.Factor = value / 100F;
            SetFilters(cFilter, sharpenFilter);
        }
    }
}
