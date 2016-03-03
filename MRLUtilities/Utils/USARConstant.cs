
namespace MRL.Utils
{

    public class USARConstant
    {

        //public readonly static int CAM_TILE_X = 4; //832;
        //public readonly static int CAM_TILE_Y = 2; //640;

        //public readonly static int MAX_CAM_WIDTH = 640; //832;
        //public readonly static int MAX_CAM_HEIGHT = 480; //640;

        //// used for identifying size of each sub image in multiview images
        //public readonly static int SUB_CAM_WIDTH = 320;
        //public readonly static int SUB_CAM_HEIGHT = 240;


        public readonly static int HUKOYO_READINGS = 271;
        public readonly static int DUALLASER_READINGS = 360;
        public readonly static int ONESIDELASER_READINGS = 181;
        public readonly static sbyte UNIT_RAD = 4;
        public readonly static string[] UNIT_NAME = new string[] { "RAW", "Meter", "Centimeter", "Millimeter", "Radian", "Degree", "UnrealUnit" };
        public readonly static float[] UNIT_TRANS = new float[] { 1.0f, 1.0f, 100.0f, 1000.0f, 1.0f, 57.2958f, 1.0f,
                                                                  1.0f, 1.0f, 100.0f, 1000.0f, 0.0f, 0.0f, 52.5f,
                                                                  0.01f, 0.01f, 1.0f, 10.0f, 0.0f, 0.0f, 0.525f, 
                                                                  0.001f, 0.001f, 0.1f, 1.0f, 0.0f, 0.0f, 0.0525f,
                                                                  1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 57.2958f, 10430.3784f, 
                                                                  0.01745f, 0.0f, 0.0f, 0.0f, 0.01745f, 1.0f, 182.0444f,
                                                                  0.01905f, 0.01905f, 1.905f, 19.05f, 9.5873e-5f, 0.005493f, 1.0f };
    }

}
