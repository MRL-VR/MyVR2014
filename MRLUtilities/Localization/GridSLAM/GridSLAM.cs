using System;
using System.Collections.Generic;
using System.Drawing;
using MRL.Commons;
using MRL.Communication.External_Commands;
using MRL.CustomMath;
using MRL.Exploration.GridSLAM.Base;
using MRL.Mapping;


namespace MRL.Exploration.GridSLAM
{
    class GridSLAM
    {
        private static double effectiveWeightPercent = 0.5;
        private static double informationReduction = 0.08;
        private static int numParticles = 200;
        private double[] effectiveWeightHistory;
        private double[] resamplingHistory;
        private double[] rmsErrorHistory;
        private double obstacleClearance = 10;
        private bool checkSLAMNoResampling = false;
        private int step = 0;
        private Pose2D[] states = new Pose2D[numParticles];
        private EGMap[] maps = new EGMap[numParticles];
        private double[] weights = new double[numParticles];
        private double[] aggregateWeights = new double[numParticles];
        private MotionModel OMM = new MotionModel();
        private double[] mapWeights = new double[numParticles];//Check the size is true .real is weights.lenght

        public Pose2D _RobotPose;
        public ILaserRangeData currentLaserRange;
        public Laser _currentLaser;


        public GridSLAM()
        {
            //SANAZ CHECK IT 
            step++;

        }

        public Pose2D setCurrentPose
        {
            get
            {
                return _RobotPose;
            }
            set
            {
                _RobotPose = value;
            }
        }

        public Laser setCurrentLaser
        {
            get
            {
                return _currentLaser;
            }
            set
            {
                _currentLaser = value;
            }
        }


        #region GetSafeShapeAsPoints
        /// <summary>
        /// Returns a list of the robot's corners plus the clearance.
        /// </summary>
        /// <param name="state">The robot's state.</param>
        /// <returns>The corners.</returns>
        public List<Point> GetSafeShapeAsPoints(Pose2D state, EGMap m)
        {

            double cosTheta = Math.Cos(state.Rotation);
            double sinTheta = Math.Sin(state.Rotation);

            //Check the height and width 
            double yDist = (m.map_height / 2) + obstacleClearance;
            double xDist = (m.map_width / 2) + obstacleClearance;

            double dx = state.X + (cosTheta * xDist - sinTheta * yDist);
            double dy = state.Y + (sinTheta * xDist + cosTheta * yDist);

            List<Point> points = new List<Point>();
            points.Add(new Point((int)Math.Round(dx / m.ScaleX), (int)Math.Round(dy / m.ScaleY)));

            dx = state.X + (cosTheta * xDist - sinTheta * -yDist);
            dy = state.Y + (sinTheta * xDist + cosTheta * -yDist);
            points.Add(new Point((int)Math.Round(dx / m.ScaleX), (int)Math.Round(dy / m.ScaleY)));

            dx = state.X + (cosTheta * -xDist - sinTheta * -yDist);
            dy = state.Y + (sinTheta * -xDist + cosTheta * -yDist);
            points.Add(new Point((int)Math.Round(dx / m.ScaleX), (int)Math.Round(dy / m.ScaleY)));

            dx = state.X + (cosTheta * -xDist - sinTheta * yDist);
            dy = state.Y + (sinTheta * -xDist + cosTheta * yDist);
            points.Add(new Point((int)Math.Round(dx / m.ScaleX), (int)Math.Round(dy / m.ScaleY)));

            return points;
        }
        #endregion


        #region IsValidState
        /// <summary>
        /// Tests a given map for overlap between occupied cells and the robot's safe outline.
        /// </summary>
        /// <param name="m">The map you want to test.</param>
        /// <param name="testState">The state of the robot.</param>
        /// <returns>True if there is no overlap.</returns>
        public bool IsValidState(EGMap m, Pose2D testState)
        {
            if (m.getProb(testState.X, testState.Y) > m.egrid.max_prob)
            {
                return false;
            }
            else
            {
                List<Point> corners = GetSafeShapeAsPoints(testState, m);
                bool safe = true;
                corners.ForEach(delegate(Point p)
                {
                    if (m.getProb(p.X, p.Y) > m.egrid.max_prob)
                    {
                        safe = false;
                    }
                });
                return safe;
            }
        }
        #endregion


        //private double distance_scan_to_map(ILaserRangeData scan, Pose2D currentpos)
        //{
        //    //EGrid ee = new EGrid();
        //    //return ee.getScanLiklihood(currentpos.X, currentpos.Y, currentpos.Rotation,
        //    //                  scan.Range.Length, scan.Range, 0.01);
        //}



        public void DoStep()
        {

            for (int i = 0; i < numParticles; i++)
            {
                //Creates a copy of this state
                states[i] = _RobotPose;
                maps[i] = new EGMap();//it is not need to have array of maps
                weights[i] = 1.0;// numP articles;
            }


            // Loop around all the particles, moving them and calculating their likelyhood
            for (int i = 0; i < numParticles; i++)
            {

                Pose2D oldState = (Pose2D)states[i];
                bool isAllowedByMap = false;

                // Get Sample applies the Motion model with added noise
                int mapFailCount = 0;
                while (!isAllowedByMap && mapFailCount < 10)
                {
                    mapFailCount++;
                    //states[i] = OMM.GetSample(oldState);
                    isAllowedByMap = IsValidState(maps[i], states[i]);
                } // this might allow a dead particle through, but it should get filtered out.


                if (mapFailCount == 10)
                {
                    // Added By SANAZ
                    //Here tell the Problemmm For Goint To DIE!!!!
                }


                double probabilityOfScan = 0;

                // probabilityOfScan = distance_scan_to_map(currentLaserRange, states[i]);

                mapWeights[i] = probabilityOfScan;

                if (probabilityOfScan == 0)
                {
                    //Write It TO Know 
                }


            }//end of for

            // prepare the weights for resampling by normalizing to a total prob of one.
            mapWeights = ProbabilityUtils.Normalize(mapWeights, Normalization.MaxToOne);
            // get a new the effective weight
            effectiveWeightHistory[step] = ProbabilityUtils.GetEffectiveSampleSize(mapWeights);

            // do the information reduction
            for (int i = 0; i < mapWeights.Length; i++)
            {
                mapWeights[i] = Math.Pow(mapWeights[i], informationReduction);
            }

            // normalize again!
            mapWeights = ProbabilityUtils.Normalize(mapWeights, Normalization.IntegralToOne);


            // The root of the mean square error 
            rmsErrorHistory[step] = ProbabilityUtils.GetRMSError(states, _RobotPose);

            // A generic low variance resampler, that works with a normalized array of weights.
            // The indexes can be used to determine which particles are needed,
            // therefore which maps need updating.
            int[] indexes = ProbabilityUtils.ReSample(mapWeights);

            // now step through the particles an update the weights using baysian approach
            weights = mapWeights;// ProbabilityUtils.BayesUpdate(weights, mapWeights);

            if (checkSLAMNoResampling || effectiveWeightHistory[step] > (effectiveWeightPercent))
            {
                // no resampling, so apply the readings to each map
                for (int i = 0; i < maps.Length; i++)
                {
                    //How to add new Pose and Map To global map
                    //maps[i].AddReadings(states[i], globalMapReadings);

                }
                resamplingHistory[step] = 0;
            }
            else
            {

                #region Create resampled sets

                //List<int> AllreadyCopied = new List<int>();
                //Pose2D[] newStates = new Pose2D[indexes.Length];
                //EGMap[] newMaps = new EGMap[indexes.Length];
                //double[] newAggregates = new double[indexes.Length];
                //double[] newWeights = new double[indexes.Length];
                //int o = 0;

                //foreach (int eee in indexes)
                //{
                //    if (AllreadyCopied.Contains(eee))
                //    {
                //        // alas this particle needs cloning.
                //        newStates[o] = (Pose2D) states[eee];
                //        newMaps[o] = maps[eee];
                //        newWeights[o] = weights[eee];
                //        newAggregates[o] = aggregateWeights[eee];

                //        // keeps a track of lost particles (i.e. duplicates)
                //        resamplingHistory[step]++;
                //    }
                //    else
                //    {
                //        // assignment is much faster than cloning
                //        AllreadyCopied.Add(eee);
                //        newStates[o] = states[eee];
                //        //maps[eee].AddReadings(states[eee], globalMapReadings);
                //        //maps[eee].ClearRobotPosition(states[eee]);
                //        newMaps[o] = maps[eee];
                //        newWeights[o] = weights[eee];
                //        newAggregates[o] = aggregateWeights[eee];
                //    }
                //    o++;
                //}
                //resamplingHistory[step] = resamplingHistory[step]/numParticles;
                //states = newStates;
                //maps = newMaps;
                //weights = newWeights;
                //aggregateWeights = newAggregates;

                #endregion
            }

            for (int i = 0; i < weights.Length; i++)
            {
                aggregateWeights[i] += weights[i];
            }


            //SANAZ Remain selecting the best particle and add to global map

            double maxAggregate = double.MinValue;
            double minAggregate = double.MaxValue;
            int indexOfMax = 0;
            int indexOfWorst = 0;
            for (int i = 0; i < numParticles; i++)
            {
                if (aggregateWeights[i] > maxAggregate)
                {
                    maxAggregate = aggregateWeights[i];
                    indexOfMax = i;
                }
                if (aggregateWeights[i] < minAggregate)
                {
                    minAggregate = aggregateWeights[i];
                    indexOfWorst = i;
                }
            }


        }


    }
}

    




