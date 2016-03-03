
namespace MRL.ImageProcessor
{
    public class FrameRateCalculator
    {
        private int lastTick;
        private int lastFrameRate;
        private int frameRate;

        public int CalculateFrameRate()
        {
            var t = System.Environment.TickCount;
            if (t - lastTick >= 1000)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                lastTick = t;
            }
            frameRate++;
            return lastFrameRate;
        }
    }
}
