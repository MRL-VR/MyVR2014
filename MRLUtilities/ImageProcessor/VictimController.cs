//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Drawing;
//using System.Linq;
//using System.Threading;
//using Emgu.CV.Structure;

//namespace MRL.ImageProcessor
//{
//    public class VictimController
//    {
//        private VictimDetector vd = new VictimDetector();
//        private volatile bool isFree = true;
//        private AutoResetEvent reactivateSignal = new AutoResetEvent(false);
//        private Bitmap SelectedImage;
//        public delegate void SelectedAreaDlg(Rectangle[] rects,long DetectingTime);
//        public event SelectedAreaDlg OnSelectedArea;

//        public void Start()
//        {
//            ThreadPool.QueueUserWorkItem(new WaitCallback(Run));
//        }

//        public void GetImage(Bitmap bmp)
//        {
//            if (isFree)
//            {
//                SelectedImage = bmp;
//                reactivateSignal.Set();
//            }
//        }

//        public void Run(object o)
//        {
//            while (reactivateSignal.WaitOne())
//            {
//                isFree = false;
//                Stopwatch sw = Stopwatch.StartNew();

//                Rectangle[] rects = vd.GetBodies(SelectedImage);
//                MCvAvgComp[][] rects2 = vd.GetFaces(SelectedImage);

//                List<Rectangle> listRect = new List<Rectangle>();
//                for (int i = 0; i < rects2.Count(); i++)
//                {
//                    for (int j = 0; j < rects2[i].Count(); j++)
//                    {
//                        listRect.Add(rects2[i][j].rect);
//                    }
//                }
                
//                listRect.AddRange(rects.ToList());

//                Rectangle[] finalRect = listRect.ToArray();
//                sw.Stop();
//                OnSelectedArea(finalRect,sw.ElapsedMilliseconds);

//                isFree = true;
//            }
//        }


//    }
//}
