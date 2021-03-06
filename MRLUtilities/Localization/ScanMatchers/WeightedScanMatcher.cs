﻿using System;
using System.Collections.Generic;
using MRL.Commons;
using MRL.CustomMath;
using MRL.Exploration.ScanMatchers.Base;

namespace MRL.Exploration.ScanMatchers
{

    public class WeightedScanMatcher : IcpScanMatcher
    {

        private const double IANGLE_THRESHOLD = System.Math.PI / 8;

        //for tuning the noise error covariance
        private const double DEFAULT_SIGMAANG = 0.002;
        private const double DEFAULT_SIGMADIST = 10;

        //for tuning the corresponde error covariance
        private const double SIGMA_CORRELATED = 10;

        private double F = 1;

        public WeightedScanMatcher()
            : base()
        {
            base.MAX_CORRELATIONDISTANCE = 2000 / 4;
            base.MAX_ITERATIONS = 80;
            base.RESULTS_CONSIDERED = 10;    //max num of results to consider 
            base.RESULTS_CONVERGED = 8;      //how many good results before converged
            base.RESULTS_MAXERROR = 0.01; //consider an iteration converged when rms below this
        }


        protected internal override MatchResult MatchPoints(Vector2[] points1, Vector2[] points2, Pose2D seed, double dangle, bool[] filter1, bool[] filter2)
        {
            //Debug.DrawPoints(points1, 0);

            //Vector2[] pt2Local = this.ToLocal(points2, seed);
            //Debug.DrawPoints(pt2Local, 1);

            int startTime = Environment.TickCount;

            Matrix2 J  = new Matrix2(0, -1, 1, 0);
            Matrix2 Jt = J.Transpose();

            double[] iangles1 = this.ComputeIncidenceAngles(points1);
            double[] iangles2 = this.ComputeIncidenceAngles(points2);

            //covariance matrixes
            Matrix2[] ecovs1 = new Matrix2[points1.Length];
            Matrix2[] ecovs2 = new Matrix2[points2.Length];

            for(int i = 0 ; i < ecovs1.Length; i++)
                ecovs1[i] = this.ComputeErrorCovariance(points1[i], iangles1[i], dangle);
            
            Pose2D rawdpose = seed;
            Pose2D curdpose = rawdpose;
            Vector2[] curpoints2 = null;
            Correlation<Vector2>[] curpairs = null;

            //indexers, for fast access to covariance matrices
            int index = 0;
            int[] index1 = null;
            int[] index2 = null;

            Queue<double> results = new Queue<double>();
            int nconverged = 0;

            Matrix2 suminvsumcov = new Matrix2();
            Vector2 midpoint = new Vector2();
            Matrix2 sumsumcov = new Matrix2();
            Vector2 transest = new Vector2();
            double rotest = 0;

            int n = 0;
            for (n = 1; n <= MAX_ITERATIONS; n++)
            {

                //recompute relative coords for range-scan 2 given the new dpose estimate
                curpoints2 = this.ToLocal(points2, curdpose);
                //Debug.DrawPoints(curpoints2, 2);

                //recompute covariances for curpoints2 (= covs2)
                for(int i = 0 ; i < ecovs2.Length; i++)
                    ecovs2[i] = this.ComputeErrorCovariance(curpoints2[i], iangles2[i], dangle);

                //find all correlated point-pairs
                Heap assoc = this.CorrelatePointsQT(points1, curpoints2, filter1, filter2);
                curpairs = getFilteredCorrespondances(assoc);
                //Debug.DrawPointRelations(curpairs);

                if(curpairs.Length > 0)
                {
                    //recompute indexes only once per iteration
                    index1 = new int[curpairs.Length];
                    index2 = new int[curpairs.Length];
                    index = 0;

                    foreach(Correlation<Vector2> pair in curpairs)
                    {
                        index1[index] = Array.IndexOf(points1, pair.Point1);
                        index2[index] = Array.IndexOf(curpoints2, pair.Point2);
                        index++;
                    }

                    //these vars will hold summaries for a single iteration
                    Vector2 sumipoint1 = new Vector2();
                    Vector2 sumipoint2 = new Vector2();
                    Matrix2 sumirotmid = new Matrix2();
                    Vector2 sumrotmid = new Vector2();
                    double sumerror = 0;
                    suminvsumcov = new Matrix2();

                    index = 0;
                    foreach(Correlation<Vector2> pair in curpairs)
                    {
                        //get corresponding points and error covariances ...
                        Vector2 point1 = pair.Point1;
                        Vector2 point2 = pair.Point2;
                        Matrix2 cov1 = ecovs1[index1[index]];
                        Matrix2 cov2 = ecovs2[index2[index]];

                        //translation estimate ...
                        Matrix2 sumcov = cov1 + cov2;
                        Matrix2 invsumcov = sumcov.Invert();

                        suminvsumcov = suminvsumcov + invsumcov;
                        sumipoint1 = sumipoint1 + invsumcov * point1;
                        sumipoint2 = sumipoint2 + invsumcov * point2;

                        //error metric ...
                        Vector2 dpoint = point1 - point2;
                        sumerror = sumerror + (dpoint * invsumcov * dpoint);

                        //rotation estimate ...
                        Matrix2 irotmid = Jt * invsumcov * J;
                        sumirotmid = sumirotmid + 2 * irotmid;
                        sumrotmid = sumrotmid + irotmid * point1 + irotmid * point2;

                        index++;
                    }

                    //get rotational midpoint and translation estimate
                    midpoint = sumirotmid.Invert() * sumrotmid;
                    sumsumcov = suminvsumcov.Invert();
                    transest = sumsumcov * (sumipoint2 - sumipoint1);



                    double sumnval = 0;
                    double sumdval = 0;

                    index = 0;
                    foreach(Correlation<Vector2> pair in curpairs)
                    {
                        //get corresponding points and error covariances ...
                        Vector2 point1 = pair.Point1;
                        Vector2 point2 = pair.Point2;
                        Matrix2 cov1 = ecovs1[index1[index]];
                        Matrix2 cov2 = ecovs2[index2[index]];

                        //apply estimated midpoint and translation
                        point1 = point1 - midpoint;
                        point2 = point2 - transest - midpoint;

                        Matrix2 sumcov = cov1 + cov2;
                        Matrix2 invsumcov = sumcov.Invert();

                        //update numerator and denominator summaries
                        Vector2 dpoint = point2 - point1;
                        double nval = dpoint * invsumcov * J * point2;
                        double dval = dpoint * invsumcov * point2 + point2 * J * invsumcov * J * point2;

                        sumnval = sumnval + nval;
                        sumdval = sumdval + dval;

                        index++;
                    }

                    //get rotational estimate
                    rotest = -sumnval / sumdval;

                    //update odometry estimate
                    Vector2 vdpose = curdpose.Position - transest - midpoint;
                    TMatrix2D rotmx = new TMatrix2D(-rotest);
                    vdpose = rotmx * vdpose;
                    vdpose = vdpose + midpoint;
                    curdpose = new Pose2D(vdpose, curdpose.Rotation - rotest);

                    //check for convergence
                    bool converged = false;
                    foreach(double result in results)
                    {
                        if (sumerror == 0)
                            converged = converged || (result < RESULTS_MAXERROR);
                        else
                            converged = converged || (System.Math.Abs(result / sumerror - 1) < RESULTS_MAXERROR);
                    }

                    results.Enqueue(sumerror);
                    while (results.Count > RESULTS_CONSIDERED)
                        results.Dequeue();

                    if (converged)
                        nconverged++;
                }

                if (nconverged >= RESULTS_CONVERGED)
                    break;
            }

            int untilTime = Environment.TickCount;
            double duration = (untilTime - startTime) / 1000.0;

            int num = (curpairs != null ? curpairs.Length : -1);
            return new MatchResult(rawdpose, curdpose, n, duration, this.HasConverged(points1.Length, num, n));
        }

        private Correlation<Vector2>[] getFilteredCorrespondances(Heap assoc)
        {
            int cnew = ((int)(assoc.Count * 100 * F)) / 100;
            Correlation<Vector2>[] ret = new Correlation<Vector2>[cnew];
            for (int i = 0; i < cnew; i++)
            {
                ret[i] = (Correlation<Vector2>)assoc[assoc.Count - i - 1];
            }

            return ret;
        }


        protected double[] ComputeIncidenceAngles(Vector2[] points)
        {
            Vector2[] dpoints = new Vector2[points.Length - 1];
            double[] dangles = new double[points.Length - 1];

            Vector2 dpoint = new Vector2();

            for(int i = 0; i < dangles.Length; i++)
            {
                dpoint = points[i + 1] - points[i];
                dpoints[i] = dpoint;
                dangles[i] = System.Math.Atan2(dpoint.Y, dpoint.X);
            }

            double[] cmpangles = new double[dpoints.Length - 1];
            double[] avgangles = new double[dpoints.Length - 1];
            double dangle;
            int iwhole;

            for(int i = 0; i < cmpangles.Length; i++)
            {
                dangle = dangles[i] - dangles[i + 1];
                iwhole = (int)(System.Math.Floor((dangle + System.Math.PI) / (2 * System.Math.PI)));
                dangle = dangle - 2 * System.Math.PI * iwhole;
                cmpangles[i] = dangle;
                avgangles[i] = dangles[i] - 0.5 * dangle;
            }

            double[] iangles = new double[points.Length];
            double iangle;

            Vector2 point;
            double angle;

            for(int i = 1; i < iangles.Length - 1; i++)
            {
                if (System.Math.Abs(cmpangles[i - 1]) < IANGLE_THRESHOLD)
                {
                    point = points[i];
                    angle = System.Math.Atan2(point.Y, point.X);
                    iangle = angle - avgangles[i - 1];
                    iangle = iangle - 2 * System.Math.PI * System.Math.Floor((iangle + System.Math.PI) / (2 * System.Math.PI));
                    iangle = iangle - System.Math.PI * System.Math.Floor((iangle + System.Math.PI / 2) / (System.Math.PI));
                }else
                    iangle = 0;
                

                iangles[i] = iangle;
            }

            return iangles;
        }

        protected Matrix2 ComputeErrorCovariance(Vector2 point, double iangle, double dangle)
        {
            Matrix2 nCov = this.ComputeNoiseErrorCovariance(point);
            Matrix2 cCov = this.ComputeCorrespondenceErrorCovariance(point, iangle, dangle);
            Matrix2 eCov = nCov + cCov;
            return eCov;
        }

        protected Matrix2 ComputeNoiseErrorCovariance(Vector2 point)
        {
            double angle = System.Math.Atan2(point.Y, point.X);
            double dist = System.Math.Sqrt(System.Math.Pow(point.X, 2) + System.Math.Pow(point.Y, 2));

            double c = System.Math.Cos(angle);
            double s = System.Math.Sin(angle);
            double a = System.Math.Pow(DEFAULT_SIGMADIST, 2);
            double b = System.Math.Pow(DEFAULT_SIGMAANG, 2) * System.Math.Pow(dist, 2);

            Matrix2 cov = new Matrix2();
            cov[0, 0] = System.Math.Pow(c, 2) * a + System.Math.Pow(s, 2) * b;
            cov[0, 1] = c * s * a - c * s * b;
            cov[1, 0] = cov[0, 1];
            cov[1, 1] = System.Math.Pow(s, 2) * a + System.Math.Pow(c, 2) * b;

            return cov;
        }

        protected Matrix2 ComputeCorrespondenceErrorCovariance(Vector2 point, double iangle, double dangle)
        {
            bool wasZero = false;
            if (iangle == 0)
            {
                wasZero = true;
                iangle = IANGLE_THRESHOLD;
            }

            double angle = System.Math.Atan2(point.Y, point.X);
            double dist = System.Math.Sqrt(System.Math.Pow(point.X, 2) + System.Math.Pow(point.Y, 2));
            double dmin = (dist * System.Math.Sin(dangle)) / System.Math.Sin(iangle + dangle);
            double dmax = (dist * System.Math.Sin(dangle)) / System.Math.Sin(iangle - dangle);
            double dsum = dmin + dmax;
            double cerr = SIGMA_CORRELATED * (System.Math.Pow(dmin, 3) + System.Math.Pow(dmax, 3)) / (3 * dsum);

            Matrix2 cov = new Matrix2();
            if (wasZero)
            {
                cov[0, 0] = cerr;
                cov[0, 1] = 0;
                cov[1, 0] = 0;
                cov[1, 1] = cerr;

            }else{
                double dang = angle - iangle;
                double c = System.Math.Cos(dang);
                double s = System.Math.Sin(dang);

                cov[0, 0] = System.Math.Pow(c, 2) * cerr;
                cov[0, 1] = c * s * cerr;
                cov[1, 0] = cov[0, 1];
                cov[1, 1] = System.Math.Pow(s, 2) * cerr;

            }

            return cov;
        }
    }

}
