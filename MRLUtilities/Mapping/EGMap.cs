using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MRL.Commons;
using MRL.Communication.External_Commands;
using MRL.Components.Tools.Shapes;
using MRL.CustomMath;
using MRL.Exploration.GridSLAM.Base;
using MRL.Utils;

namespace MRL.Mapping
{

    //public class MapGenerator
    //{
    //    internal class SyncEvents
    //    {
    //        private EventWaitHandle _newScanEvent;
    //        private EventWaitHandle _exitThreadEvent;

    //        private WaitHandle[] _eventArray1;

    //        public SyncEvents()
    //        {
    //            _newScanEvent = new ManualResetEvent(false);
    //            _exitThreadEvent = new ManualResetEvent(false);

    //            _eventArray1 = new WaitHandle[2];
    //            _eventArray1[0] = _newScanEvent;
    //            _eventArray1[1] = _exitThreadEvent;
    //        }

    //        public EventWaitHandle ExitThreadEvent
    //        {
    //            get { return _exitThreadEvent; }
    //        }
    //        public EventWaitHandle NewScanEvent
    //        {
    //            get { return _newScanEvent; }
    //        }
    //        public WaitHandle[] EventArrayForCommitingData
    //        {
    //            get { return _eventArray1; }
    //        }
    //    }

    //    private EGMap parentCtrl = null;
    //    private SyncEvents _syncEvents;

    //    public MapGenerator(EGMap parent)
    //    {
    //        this.parentCtrl = parent;
    //        _syncEvents = new SyncEvents();
    //    }

    //    [MethodImpl(MethodImplOptions.Synchronized)]
    //    public void SetScanSignal()
    //    {
    //        _syncEvents.NewScanEvent.Set();
    //    }

    //    [MethodImpl(MethodImplOptions.Synchronized)]
    //    public void SetExitSignal()
    //    {
    //        _syncEvents.NewScanEvent.Reset();
    //        _syncEvents.ExitThreadEvent.Set();
    //    }

    //    public void Run()
    //    {
    //        try
    //        {
    //            while (WaitHandle.WaitAny(_syncEvents.EventArrayForCommitingData) != 1)
    //            {
    //                if (parentCtrl.scan_list.Count >= parentCtrl.commitCount)
    //                {
    //                    parentCtrl.commit();

    //                    if (parentCtrl.scan_list.Count == 0)
    //                        _syncEvents.NewScanEvent.Reset();
    //                }
    //            }

    //            ProjectCommons.writeConsoleMessage("Mapping Thread Forciablity Closed", ConsoleMessageType.Information);
    //            Thread.Sleep(2000);
    //        }
    //        catch (ThreadAbortException tee)
    //        {
    //            ProjectCommons.writeConsoleMessage("Mapping Thread Aborted", ConsoleMessageType.Error);
    //            USARLog.println("Thread Aborted", 0, this.ToString());
    //            parentCtrl.CleanUpAndRepairThread();
    //        }
    //        catch (Exception e)
    //        {
    //            ProjectCommons.writeConsoleMessage("Unable commiting data : " + e.ToString(), ConsoleMessageType.Error);
    //            USARLog.println("Unable commiting data : " + e.ToString(), 5, this.ToString());
    //            parentCtrl.CleanUpAndRepairThread();
    //        }
    //    }
    //}

    [Serializable()]
    public class EGMap : IEGMap
    {
        /* Parameters */
        public static double DEFAULT_CAMERA_FOV = 0.525;
        public static double DEFAULT_RESOLUTION = 0.025;
        public static double DEFAULT_DOWNSAMPLE = 4;
        public static double DEFAULT_START_ANGLE = 0;
        public static double DEFAULT_WALL_THICKNESS = 0.2;
        public static double DEFAULT_BORDER = 0;

        public static double DEFAULT_POSITIVE_PROB = 0.56;
        public static double DEFAULT_P_OCC = 0.5;
        public static double DEFAULT_NEGATIVE_PROB = 0.45;
        public static double DEFAULT_MAX_PROB = 0.95;
        public static double DEFAULT_CLOSE_RANGE = 5.0;
        public static double DEFAULT_LONG_RANGE = 10.0;

        public static double DEFAULT_DISTRIBUTION_MEAN = 0.0;
        public static double DEFAULT_DISTRIBUTION_SIGMA = 0.02;
        public static double DEFAULT_REFINEMENT_ITERATION = 30;
        public static double DEFAULT_MIN_DISTANCE_TO_WALL_FOR_ROBOT = 0.25;
        public static double DEFAULT_POTENTIAL_SURE_RANGE = 0.1;
        public static double DEFAULT_POTENTIAL_INCREMENTION_RADIUS = 1.0;
        public static double DEFAULT_MAX_PROB_FOR_NAVIGATION = 0.6; //Changed From 0.7
        public static double DEFAULT_VICTIM_RADIUS = 0.9;

        public static int NUM_EGRID_PARAMS = 20;

        public static int RESOLUTION = 0;
        public static int DOWNSAMPLE = 1;
        public static int START_ANGLE = 2;
        public static int WALL_THICKNESS = 3;
        public static int BORDER = 4;
        public static int POSITIVE_PROB = 5;
        public static int P_OCC = 6;
        public static int NEGATIVE_PROB = 7;
        public static int MAX_PROB = 8;
        public static int CLOSE_RANGE = 9;
        public static int LONG_RANGE = 10;
        public static int CAMERA_FOV = 11;
        public static int DISTRIBUTION_MEAN = 12;
        public static int DISTRIBUTION_SIGMA = 13;
        public static int REFINEMENT_ITERATION = 14;

        public static int MIN_DISTANCE_TO_WALL_FOR_ROBOT = 15;
        public static int POTENTIAL_SURE_RANGE = 16;
        public static int POTENTIAL_INCREMENTION_RADIUS = 17;
        public static int MAX_PROB_FOR_NAVIGATION = 18;

        public static int VICTIM_RADIUS = 19;

        public static double[] egmParam = new double[] { DEFAULT_RESOLUTION, DEFAULT_DOWNSAMPLE, 
                                                         DEFAULT_START_ANGLE, DEFAULT_WALL_THICKNESS, 
                                                         DEFAULT_BORDER, DEFAULT_POSITIVE_PROB, 
                                                         DEFAULT_P_OCC, DEFAULT_NEGATIVE_PROB, 
                                                         DEFAULT_MAX_PROB, DEFAULT_CLOSE_RANGE, 
                                                         DEFAULT_LONG_RANGE, DEFAULT_CAMERA_FOV,
                                                         DEFAULT_DISTRIBUTION_MEAN, DEFAULT_DISTRIBUTION_SIGMA,
                                                         DEFAULT_REFINEMENT_ITERATION, DEFAULT_MIN_DISTANCE_TO_WALL_FOR_ROBOT,
                                                         DEFAULT_POTENTIAL_SURE_RANGE, DEFAULT_POTENTIAL_INCREMENTION_RADIUS,
                                                         DEFAULT_MAX_PROB_FOR_NAVIGATION, DEFAULT_VICTIM_RADIUS};

        public static string[] param_strings = new string[] { 
                                                              "resolution (m)", "downsample", "start angle", 
                                                              "wall thickness (m)", "border", "occupied evidence", 
                                                              "occupied prior", "empty evidence", "max probability", 
                                                              "close range", "long range", "camera fov",
                                                              "mean of distribution", "sigma of distribution",
                                                              "number of iterations in refinement step", 
                                                              "", "","","","" 
                                                            };

        public static int MAX_SCANS = 1000;

        // map update delegate
        [field: NonSerialized()]
        public event ProjectCommons._geoRefrencedMap_Updated geoReferenceMap_Updated;


        /* Layout */
        internal int map_width;
        internal int map_height;
        internal int canvas_width;
        internal int canvas_height;
        internal double offset_x, offset_y;
        internal double start_x, start_y;

        internal double arena_width = 20.0;
        internal double arena_height = 20.0;
        internal double increment_width = 5.0;
        internal double increment_height = 5.0;

        public List<long> EmapProccesTimeList = new List<long>();
        public List<long> MapGenProccesTimeList = new List<long>();

        //[NonSerialized()]
        //private Thread commitToEGrid = null;

        internal EGrid egrid = null;
        internal ArrayList vict_list = ArrayList.Synchronized(new ArrayList(10));
        internal BlockingCollection<ILaserRangeData> scan_list = new BlockingCollection<ILaserRangeData>();
        internal BlockingCollection<CamView> stored_view_list = new BlockingCollection<CamView>();
        internal CancellationTokenSource cts;

        [NonSerialized()]
        internal Bitmap pixmap = null;
        internal int imgSeq = 0;

        [NonSerialized()]
        internal Task mapGEN = null;

        internal bool bStop = false;
        internal int commitCount = 1;
        internal int updateCount = 1;
        internal int commitedCount = 0;

        internal int num_scans = 0;
        internal bool imageReady = true;
        internal bool bTransparent = false;
        internal bool bSampleAdjust = false;
        internal bool bDebug = false;

        [NonSerialized()]
        internal NormalDistribution randomizer;

        [NonSerialized()]
        internal MotionModel mMotionModel = new MotionModel();

        public int MapWidthSize
        {
            get
            {
                return egrid.WidthSize;
            }

        }

        public int MapheightSize
        {
            get
            {
                return egrid.heightSize;
            }

        }



        public bool Transparent
        {
            get
            {
                return bTransparent;
            }

            set
            {
                if (bTransparent != value)
                {
                    bTransparent = value;
                    updateImage();
                }
            }
        }

        public int DownSample
        {
            set
            {
                if (value >= 1)
                {
                    bSampleAdjust = true;
                    egmParam[DOWNSAMPLE] = value;
                    updateImage();
                }
            }
        }

        public int ImgSeq
        {
            get
            {
                return imgSeq;
            }
        }

        public double ScaleX
        {
            get
            {
                return (canvas_width - 1.0) / (map_width - 1.0) / egmParam[RESOLUTION];
            }
        }

        public double ScaleY
        {
            get
            {
                return (canvas_height - 1.0) / (map_height - 1.0) / egmParam[RESOLUTION];
            }
        }

        public int Map_Width
        {
            get
            {
                return map_width;
            }
        }

        public int Map_Height
        {
            get
            {
                return map_height;
            }
        }

        public EGMap()
        {
            init();
        }

        /// <summary>all the args are in meters  </summary>
        public EGMap(double iniWidth, double iniHeight, double widthStep, double heightStep)
        {

            this.arena_width = iniWidth;
            this.arena_height = iniHeight;
            this.increment_width = widthStep;
            this.increment_height = heightStep;
            init();
        }

        protected internal void init()
        {
            map_width = (int)(arena_width / egmParam[RESOLUTION]);
            //(int) (2.0 * arena_width / params[RESOLUTION]);
            map_height = (int)(arena_height / egmParam[RESOLUTION]);
            //(int) (2.0 * arena_height / params[RESOLUTION]);

            canvas_width = (int)(arena_width / (egmParam[RESOLUTION] * egmParam[DOWNSAMPLE]) + egmParam[BORDER]);
            canvas_height = (int)(arena_height / (egmParam[RESOLUTION] * egmParam[DOWNSAMPLE]) + egmParam[BORDER]);

            egrid = new EGrid(map_width, map_height, egmParam[RESOLUTION], egmParam[START_ANGLE],
                              egmParam[P_OCC], egmParam[POSITIVE_PROB], egmParam[NEGATIVE_PROB],
                              egmParam[MAX_PROB], egmParam[CLOSE_RANGE], egmParam[LONG_RANGE],
                              egmParam[WALL_THICKNESS]);

            randomizer = new NormalDistribution(egmParam[DISTRIBUTION_MEAN], egmParam[DISTRIBUTION_SIGMA]);
        }

        /// <summary>From robot's world pose (in carmen frame) to map canvas pose </summary>
        public Pose2D TransposeRealToCanvas(Pose2D p)
        {
            Pose2D tmp = null;
            lock (egrid)
            {
                tmp = egrid.transPose(p);
            }
            double x = (tmp.X - offset_x) * (canvas_width - 1) / (map_width - 1);
            double y = (1 - (tmp.Y - offset_y) / (map_height - 1)) * (canvas_height - 1);
            return new Pose2D(x, y, tmp.Rotation);
        }

        /// <summary>From map canvas pose to robot's world pose (in carmen frame)</summary>
        public Pose2D TransposeCanvas2Real(Pose2D p)
        {
            double tmp_x = p.X * (map_width - 1) / (canvas_width - 1) + offset_x;
            double tmp_y = (1 - p.Y / (canvas_height - 1)) * (map_height - 1) + offset_y;

            Pose2D tmp = null;
            lock (egrid)
            {
                tmp = egrid.transPose2(new Pose2D(tmp_x, tmp_y, p.Rotation));
            }

            return tmp;
        }

        public virtual float getWindowPotential(Pose2D pWorld, float Radius)
        {
            return this.egrid.getCirclePotential(pWorld, Radius);
        }

        public virtual bool isWall(Pose2D p)
        {
            //Pose pc = USARRobot.ConvertGlobalToCarmen(p);
            //double prob = this.egrid.getOccProb(pc.x, pc.y);
            //return (prob >= this.egrid.max_prob);
            return this.egrid.isPointInWall(p, egmParam[EGMap.MIN_DISTANCE_TO_WALL_FOR_ROBOT],
                                            egmParam[EGMap.MAX_PROB_FOR_NAVIGATION]);
        }

        public virtual Dictionary<Index, float> getOpenRooms(Index mLocation, int mSamplingAngleStep, double mSamplingRadius)
        {
            return egrid.getOpenRooms(mLocation, egmParam[MIN_DISTANCE_TO_WALL_FOR_ROBOT], mSamplingRadius,
                                      mSamplingAngleStep, egmParam[MAX_PROB_FOR_NAVIGATION], vict_list);
        }

        public virtual Dictionary<Index, float> getOpenRoomsWithoutVictimCheck(Index mLocation, int mSamplingAngleStep, double mSamplingRadius)
        {
            return egrid.getOpenRooms(mLocation, egmParam[MIN_DISTANCE_TO_WALL_FOR_ROBOT], mSamplingRadius,
                                      mSamplingAngleStep, egmParam[MAX_PROB_FOR_NAVIGATION], null);
        }

        public void setRobotPotential(Pose2D point)
        {
            try
            {
                egrid.addCirclePotential(point, egmParam[POTENTIAL_INCREMENTION_RADIUS],
                                                egmParam[POTENTIAL_SURE_RANGE],
                                                egmParam[MAX_PROB_FOR_NAVIGATION]);
            }
            catch (Exception e)
            {
                USARLog.println(e.Message, 5, "SetRobotPotential");
            }
        }

        /// <summary>get prob from robot's world position </summary>
        public float getProb(double x, double y)
        {
            return egrid.getOccProb(x, y);
        }

        /// <param name="start,end:">
        /// the start and end point in robot's world pose
        /// threshold: the threshold used to judge where a point is occupied
        /// </param>
        /// <returns> the block point in robot's world pose
        /// </returns>
        public Pose2D isClear(Pose2D start, Pose2D end, double threshold)
        {
            return egrid.isClear(start, end, threshold);
        }
        public virtual Pose2D isClear(Pose2D start, Pose2D end)
        {
            //return egrid.isRoadClear(start, end, egmParam[MAX_PROB_FOR_NAVIGATION],
            //                         egmParam[MIN_DISTANCE_TO_WALL_FOR_ROBOT]);

            return egrid.isRoadClear(start, end, egmParam[MAX_PROB_FOR_NAVIGATION],
                                     egmParam[MIN_DISTANCE_TO_WALL_FOR_ROBOT]);
        }
        public virtual Index isClear(Index start, Index end)
        {
            return egrid.isRoadClear(start, end, egmParam[MAX_PROB_FOR_NAVIGATION],
                                     egmParam[MIN_DISTANCE_TO_WALL_FOR_ROBOT]);
        }

        public Pose2D isRoadClear(Pose2D start, Pose2D goal, double threshold, double roadLenght)
        {
            Index i = egrid.isRoadClear(new Index((int)start.X, (int)start.Y), new Index((int)goal.X, (int)goal.Y), threshold, roadLenght);
            return i != null ? new Pose2D(i.x, i.y, 0) : null;
        }

        //public void CleanUpAndRepairThread()
        //{
        //    commitToEGrid = new Thread(new ThreadStart(new MapGenerator(this).Run));
        //    commitToEGrid.Start();
        //}

        [MethodImpl(MethodImplOptions.Synchronized)]
        private bool updateImage()
        {
            receive++;
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            int x, y, x2, y2;
            int c, rgb;
            double real_prob, obs_prob;

            if (!imageReady)
                return false;

            imageReady = false;
            if (bSampleAdjust)
            {
                canvas_width = (int)(arena_width / (egmParam[RESOLUTION] * egmParam[DOWNSAMPLE]) + egmParam[BORDER]);
                canvas_height = (int)(arena_height / (egmParam[RESOLUTION] * egmParam[DOWNSAMPLE]) + egmParam[BORDER]);
                pixmap = new Bitmap(canvas_width, canvas_height, PixelFormat.Format32bppArgb);
                bSampleAdjust = false;
            }
            else if (pixmap == null)
                pixmap = new Bitmap(canvas_width, canvas_height, PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(pixmap);
            g.Clear(Color.Blue);
            g.Dispose();

            for (x = 0; x < canvas_width; x++)
            {
                for (y = 0; y < canvas_height; y++)
                {
                    x2 = (int)((x / (double)(canvas_width - 1)) * (map_width - 1) + offset_x);
                    y2 = (int)((1.0 - (y / (double)(canvas_height - 1))) * (map_height - 1) + offset_y);
                    real_prob = egrid.GetProb(x2, y2);
                    if (real_prob == -1) continue;
                    if (egrid.GetProb(x2, y2) == -1)
                        rgb = (int)(bTransparent ? 0x800000ff : 0xff0000ff);
                    else
                    {
                        real_prob = egrid.GetProb(x2, y2) - (int)egrid.GetProb(x2, y2);
                        obs_prob = (int)egrid.GetProb(x2, y2) / 10000.0;
                        c = 255 - (int)(real_prob * 255);
                        rgb = ((c << 8 | c) << 8) | c;
                        if (bTransparent)
                            rgb = rgb | unchecked((int)0x80000000);
                        else
                            rgb = rgb | unchecked((int)0xff000000);
                        if (obs_prob > 0 && real_prob < 0.7)
                        {
                            rgb = (200 + (int)(obs_prob * 55)) << 16 | (230 + (int)(obs_prob * 20)) << 8 | (200 + (int)(obs_prob * 55)) | unchecked((int)0xff000000);
                        }
                    }
                    pixmap.SetPixel(x, y, Color.FromArgb(rgb));
                }
            }

            imgSeq += 1;
            imageReady = true;

            if (bDebug)
                USARLog.println("EG::" + imgSeq + "/" + num_scans, 0, this.ToString());

            try
            {
                geoReferenceMap_Updated((Bitmap)pixmap.Clone());
            }
            catch { }

            sw.Stop();

            lock (MapGenProccesTimeList)
                MapGenProccesTimeList.Add(sw.ElapsedMilliseconds);
            return true;
        }

        

        private void sizeFinish()
        {
            if (egrid == null)
                return;

            egrid.finish((int)egmParam[DOWNSAMPLE], (int)egmParam[BORDER]);
            map_width = 2 * egrid.size_x;
            map_height = 2 * egrid.size_y;
            offset_x = 0;
            offset_y = 0;
        }
        int send = 0, receive = 0;
        public bool addScan(ILaserRangeData ls)
        {
            send++;
            if (num_scans == 0)
            {
                start_x = ls.SensorOffset.X;
                start_y = ls.SensorOffset.Y;

                offset_x = 0;
                offset_y = 0;
            }

            //bool added;
            //lock (scan_list)
            //{
            //    added = scan_list.Add(ls) >= 0;
            //}

            //if (added)
            //{
            //    mapGEN.SetScanSignal();
            //} 
            return scan_list.TryAdd(ls);
        }

        public bool addView(Pose2D cv)
        {
            if (num_scans == 0)
            {
                return false;
            }

            return stored_view_list.TryAdd(new CamView(cv, egmParam[CAMERA_FOV]));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void increase(double addWidth, double addHeight)
        {
            if (addWidth > 0)
            {
                arena_width += (int)(addWidth / increment_width + 1) * increment_width * 2;
                map_width = (int)(arena_width / egmParam[RESOLUTION]);
                //(int) (2.0 * arena_width / params[RESOLUTION]);
                canvas_width = (int)(arena_width / (egmParam[RESOLUTION] * egmParam[DOWNSAMPLE]) + egmParam[BORDER]);
                //offset_x = 
                //  (arena_width/2 - start_x) / params[RESOLUTION] + params[BORDER] * params[RESOLUTION];
            }
            if (addHeight > 0)
            {
                arena_height += (int)(addHeight / increment_height + 1) * increment_height * 2;
                map_height = (int)(arena_height / egmParam[RESOLUTION]);
                //(int) (2.0 * arena_height / params[RESOLUTION]);
                canvas_height = (int)(arena_height / (egmParam[RESOLUTION] * egmParam[DOWNSAMPLE]) + egmParam[BORDER]);
                //offset_y = 
                //  (arena_height/2 - start_y) / params[RESOLUTION] + params[BORDER] * params[RESOLUTION];
            }
            imageReady = false;

            pixmap = null; // force to create new image

            lock (egrid)
            {
                egrid.enlarge(map_width, map_height);
            }

            imageReady = true;
        }
        protected internal void commit(ILaserRangeData ls)
        {
            long increaseTime, updateTime, updateImageTime;
            //for (int i = 0; i < commitCount; i++)
            //{
            Stopwatch sw = new Stopwatch();
            sw.Restart();

            //ILaserRangeData ls;
            //lock (scan_list)
            //{
            //    if (scan_list.Count > MAX_SCANS)
            //        while (scan_list.Count > MAX_SCANS / 2)
            //            scan_list.RemoveAt(0);

            //    Object tempObject = scan_list[0];
            //     ls = tempObject as ILaserRangeData;
            //    scan_list.RemoveAt(0);
            //}

            double mRange = Laser.GetMaxRange(ls);
            double addWidth = Math.Abs(ls.SensorOffset.X - start_x) + mRange - arena_width / 2.0;
            double addHeight = Math.Abs(ls.SensorOffset.Y - start_y) + mRange - arena_height / 2.0;

            if (addWidth > 0 || addHeight > 0)
            {
                increase(addWidth, addHeight);
            }
            sw.Stop();
            increaseTime = sw.ElapsedMilliseconds;
            sw.Restart();

            num_scans += 1;
            commitedCount += 1;
            //lock (egrid)
            {
                egrid.update(ls.SensorOffset.X, -ls.SensorOffset.Y, -ls.SensorOffset.Rotation, ls.Range.Length, ls.Range); //, 0.9f);
            }
            sw.Stop();
            updateTime = sw.ElapsedMilliseconds;

            lock (EmapProccesTimeList)
                EmapProccesTimeList.Add(sw.ElapsedMilliseconds);

            //if (commitedCount > updateCount)
            //{
            //    updateImage();
            //    commitedCount = 0;
            //}
            sw.Restart();
            if (updateCount > 1)
            {
                updateImage();
                updateCount = 0;
            }
            else
                updateCount++;
            sw.Stop();
            updateImageTime = sw.ElapsedMilliseconds;

            //}
        }

        private void regenViewedArea()
        {
            USARLog.println("stored_view_list.Count=" + stored_view_list.Count, 1, "EGMap");

            if (stored_view_list.Count == 0)
                return;

            egrid.clearViewedArea();

            //int _viewCount = stored_view_list.Count;

            //if (_viewCount > 0)
            //    for (int i = 0; i < _viewCount; i++)
            //    {
            //        CamView cv = (CamView)stored_view_list[i];
            //        lock (egrid)
            //        {
            //            egrid.obsUpdate(cv.x, -cv.y, -cv.theta, cv.fov, 0.0175);
            //        }
            //    }

            int count = stored_view_list.Count;
            for (int i = 0; i < count; i++)
            {
                CamView cv = (CamView)stored_view_list.Take();
                lock (egrid)
                {
                    egrid.obsUpdate(cv.x, -cv.y, -cv.theta, cv.fov, 0.0175);
                }
            }

        }

        public void start()
        {
            //mapGEN = new MapGenerator(this);
            //commitToEGrid = new Thread(new ThreadStart(mapGEN.Run));
            //commitToEGrid.Start();
            cts = new CancellationTokenSource();
            CancellationToken t = cts.Token;
            mapGEN = Task.Factory.StartNew(() =>
            {
                foreach (var item in scan_list.GetConsumingEnumerable())
                {
                    if (t.IsCancellationRequested)
                        break;

                    commit(item);
                }

            }, t);
        }

        public void stop()
        {
            bStop = true;

            cts.Cancel();
            scan_list.CompleteAdding();
            stored_view_list.CompleteAdding();

            //mapGEN.SetExitSignal();
            //commitToEGrid.Join(2000);
        }

        public bool save(string fName, ImageFormat format)
        {
            updateImage();

            if (pixmap != null)
                lock (pixmap)
                {
                    try
                    {
                        pixmap.Save(fName, format);
                        return true;
                    }
                    catch (Exception e)
                    {
                        USARLog.println(e.Message, 5, this.ToString());
                    }
                }

            return false;
        }

        /// <summary> export the map in GeoTiff format</summary>
        /// <param name="fName">file name</param>
        /// <param name="tags">the victim array</param>
        /// <param name="bBinary">true: export map in binary values; false: export map in possibility value</param>
        public bool exportGeoTiff(string fName, List<VictimShape> victimData, bool bBinary)
        {
            Bitmap mapImg = new Bitmap(map_height, map_width, PixelFormat.Format32bppArgb);
            int c, rgb;
            double real_prob, obs_prob;

            regenViewedArea();

            for (int x = 0; x < map_width; x++)
            {
                for (int y = 0; y < map_height; y++)
                {
                    if (egrid.GetProb(x, y) == -1)
                        rgb = unchecked((int)0xff0000ff);
                    // blue
                    else
                    {
                        real_prob = egrid.GetProb(x, y) - (int)egrid.GetProb(x, y);
                        obs_prob = (int)egrid.GetProb(x, y) / 10000.0;
                        if (bBinary)
                        {
                            if (real_prob > 0.65)
                                rgb = unchecked((int)0xff000000);
                            // black
                            else if (obs_prob > 0 && obs_prob < 0.8)
                                rgb = unchecked((int)0xff00ff00);
                            // green
                            else
                                rgb = unchecked((int)0xffffffff);
                            // white
                        }
                        else
                        {
                            //grayscale
                            c = 255 - (int)(real_prob * 255);
                            rgb = ((c << 8 | c) << 8) | c | unchecked((int)0xff000000);
                        }
                    }
                    mapImg.SetPixel(map_height - 1 - y, map_width - 1 - x, Color.FromArgb(rgb));
                }
            }

            if (victimData.Count != 0)
            {
                int markLineWidth = (int)(0.5 / egmParam[RESOLUTION]);
                int markRadius = (int)(0.5 / egmParam[RESOLUTION]);
                Point markO = new Point(0, 0);
                Point markS = new Point(0, 0);
                Point markE = new Point(0, 0);

                Graphics g2d = Graphics.FromImage(mapImg);

                //g2d.setPaint(Color.RED);
                //SupportClass.GraphicsManager.manager.SetPen(g2d, new System.Drawing.Pen(System.Drawing.Brushes.Black, markLineWidth));

                Pose2D p = null;
                int count = victimData.Count;
                for (int i = 0; i < count; i++)
                {
                    VictimShape vs = victimData[i];
                    {
                        p = ProjectCommons.ConvertGlobalToCarmen(vs.RealPose);
                        p = egrid.transPose(p);
                        markO.X = map_height - 1 - (int)p.Y;
                        markO.Y = map_width - 1 - (int)p.X;
                        markS.X = markO.X - markRadius;
                        if (markS.X < 0)
                            markS.X = 0;
                        markS.Y = markO.Y - markRadius;
                        if (markS.Y < 0)
                            markS.Y = 0;
                        markE.X = markO.X + markRadius;
                        if (markE.X > map_height)
                            markE.X = map_height - 1;
                        markE.Y = markO.Y + markRadius;
                        if (markE.Y > map_width)
                            markE.Y = map_width - 1;

                        g2d.DrawLine(new Pen(Color.Red, markLineWidth), markS.X, markS.Y, markE.X, markE.Y);

                        markS.X = markO.X - markRadius;
                        if (markS.X < 0)
                            markS.X = 0;
                        markS.Y = markO.Y + markRadius;
                        if (markS.Y > map_width)
                            markS.Y = map_width - 1;
                        markE.X = markO.X + markRadius;
                        if (markE.X > map_height)
                            markE.X = map_height - 1;
                        markE.Y = markO.Y - markRadius;
                        if (markE.Y < 0)
                            markE.Y = 0;

                        //g2d.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g2d), markS.X, markS.Y, markE.X, markE.Y);
                        g2d.DrawLine(new Pen(Color.Red, markLineWidth), markS.X, markS.Y, markE.X, markE.Y);
                    }
                }
            }

            try
            {
                // create TIFF file
                mapImg.Save(fName + ".tiff", ImageFormat.Tiff);
                StreamWriter bw = new StreamWriter(fName + ".tfw", false);

                // create TFW file
                //- x resolution
                string info = "" + egmParam[RESOLUTION];
                bw.WriteLine(info);
                //- y rotation
                info = "0.0";
                bw.WriteLine(info);
                //- x rotation
                info = "0.0";
                bw.WriteLine(info);
                //- negtive y resolution
                info = "" + (-egmParam[RESOLUTION]);
                bw.WriteLine(info);
                Pose2D p = egrid.transPose2(new Pose2D(map_width - 1, map_height - 1, 0));
                //- x of upper left pixle
                info = "" + (-p.Y);
                bw.WriteLine(info);
                //- y of upper left pixle
                info = "" + p.X;
                bw.WriteLine(info);
                bw.Close();
                return true;
            }
            catch (Exception e)
            {
                USARLog.println(e.Message, 5, this.ToString());
                return false;
            }
        }

        public Pose2D RefinePose(Pose2D Prev_pose, Pose2D start_pose, ILaserRangeData scan)
        {
            Pose2D currentpos, bestpos;
            double currentdist, startdist;
            double bestdist;
            int counter = 0;

            currentpos = new Pose2D(start_pose);
            currentdist = distance_scan_to_map(scan, currentpos);
            startdist = currentdist;

            //if (currentdist < 100) return start_pose;

            bestpos = new Pose2D(start_pose);
            bestdist = currentdist;

            do
            {
                //currentpos.X = GetNormalDistribution(currentpos.X);
                //currentpos.Y = GetNormalDistribution(currentpos.Y);

                //Use Motion Model 
                currentpos = mMotionModel.GetSample(Prev_pose, start_pose);

                currentdist = distance_scan_to_map(scan, currentpos);

                if (currentdist > bestdist)
                {
                    bestdist = currentdist;
                    bestpos = new Pose2D(currentpos);
                }
                else
                {
                    counter++;
                }
            } while (counter < egmParam[REFINEMENT_ITERATION]);

            //if (bestdist != startdist)
            //    USARLog.println(string.Format("Monte Carlo LiklyHood From : {0}, To : {1}", startdist, bestdist), "Monte Carlo >> ");

            return bestpos;
        }

        private double distance_scan_to_map(ILaserRangeData scan, Pose2D currentpos)
        {
            return egrid.getScanLiklihood(currentpos.X, -currentpos.Y, -currentpos.Rotation,
                                          scan.Range.Length, scan.Range, egmParam[MAX_PROB]);
        }

        private double GetNormalDistribution(double v)
        {
            double ng = randomizer.NextDouble();
            return v + ng;
        }

        public IEGMap Clone()
        {
            MemoryStream mStream = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(mStream, this);
            mStream.Position = 0;

            EGMap m = b.Deserialize(mStream) as EGMap;
            m.egrid.probToken = new ReaderWriterLockSlim();

            return m;
        }
    }

}
