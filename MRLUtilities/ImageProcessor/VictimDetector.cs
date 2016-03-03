//using System;
//using System.Drawing;
//using Emgu.CV;
//using Emgu.CV.GPU;
//using Emgu.CV.Structure;

//namespace MRL.ImageProcessor
//{ 
//    // Amir Panah

//    public class VictimDetector
//    {
//        public bool DetectFeatures(Bitmap bmp)
//        {

//            Rectangle[] regions = GetBodies(bmp);
//            MCvAvgComp[][] facesDetected = GetFaces(bmp);

//            bool result = false;

//            if (regions.Length > 0 || facesDetected[0].Length > 0)
//                result = true;

//            return result;
//        }


//        public Rectangle[] GetBodies(Bitmap bmp)
//        {
//            Image<Bgr, Byte> image = new Image<Bgr, byte>(bmp);

//            Rectangle[] regions;

//            //check if there is a compatible GPU to run pedestrian detection
//            if (GpuInvoke.HasCuda)
//            {  //this is the GPU version
//                using (GpuHOGDescriptor des = new GpuHOGDescriptor())
//                {
//                    des.SetSVMDetector(GpuHOGDescriptor.GetDefaultPeopleDetector());

//                    using (GpuImage<Bgr, Byte> gpuImg = new GpuImage<Bgr, byte>(image))
//                    using (GpuImage<Bgra, Byte> gpuBgra = gpuImg.Convert<Bgra, Byte>())
//                    {
//                        regions = des.DetectMultiScale(gpuBgra);
//                    }
//                }
//            }
//            else
//            {  //this is the CPU version
//                using (HOGDescriptor des = new HOGDescriptor())
//                {
//                    des.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());

//                    regions = des.DetectMultiScale(image);
//                }
//            }

//            return regions;
//        }
//        // end Body function


//        // firs face function

//        public MCvAvgComp[][] GetFaces(Bitmap bmp)
//        {
//            Image<Bgr, Byte> image = new Image<Bgr, byte>(bmp); //Read the files as an 8-bit Bgr image  
//            Image<Gray, Byte> gray = image.Convert<Gray, Byte>(); //Convert it to Grayscale

//            //normalizes brightness and increases contrast of the image
//            gray._EqualizeHist();

//            //Read the HaarCascade objects
//            HaarCascade face = new HaarCascade("haarcascade_frontalface_alt_tree.xml");
//            HaarCascade eye = new HaarCascade("haarcascade_eye.xml");

//            //Detect the faces  from the gray scale image and store the locations as rectangle
//            //The first dimensional is the channel
//            //The second dimension is the index of the rectangle in the specific channel
//            MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
//               face,
//               1.1,
//               10,
//               Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
//               new Size(20, 20));

//            return facesDetected;

//        }


//        public bool result { get; set; }
        
//        // Amir Panah
//    }
//}
