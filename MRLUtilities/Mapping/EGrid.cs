using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using MRL.Commons;
using MRL.Communication.External_Commands;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Mapping
{

    [Serializable()]
    public class EGrid
    {
        internal double resolution;
        internal volatile int size_x, size_y;
        internal double theta_offset;
        internal volatile float prior_occ;
        internal double occ_evidence, emp_evidence, max_prob;
        internal double max_sure_range, max_range;
        internal double wall_thickness;

        private float[,] prob;
        private int distance_table_size;
        private double[,] distance_table;

        private bool first;
        private double start_x, start_y, start_theta = 0.0;

        [NonSerialized()]
        public ReaderWriterLockSlim probToken = new ReaderWriterLockSlim();

        public int WidthSize
        {
            get
            {
                return size_x;
            }

        }

        public int heightSize
        {
            get
            {
                return size_y;
            }

        }

        public float GetProb(int i, int j)
        {
            try
            {
                probToken.EnterReadLock();
                return prob[i, j];
            }
            finally
            {
                probToken.ExitReadLock();
            }
        }
        public void SetProb(int i, int j, float value)
        {
            try
            {
                probToken.EnterWriteLock();
                prob[i, j] = value;
            }
            finally
            {
                probToken.ExitWriteLock();
            }
        }

        [NonSerialized()]
        private ReaderWriterLockSlim distToken = new ReaderWriterLockSlim();

        public double GetDistance(int i, int j)
        {
            try
            {
                //distToken.EnterReadLock();
                return distance_table[i, j];
            }
            finally
            {
                //distToken.ExitReadLock();
            }
        }
        public void SetDistance(int i, int j, double value)
        {
            try
            {
                //distToken.EnterWriteLock();
                distance_table[i, j] = value;
            }
            finally
            {
                //distToken.ExitWriteLock();
            }
        }

        public EGrid(int size_x, int size_y, double resolution, double theta_offset, double prior_occ, double occ_evidence, double emp_evidence, double max_prob, double max_sure_range, double max_range, double wall_thickness)
        {
            int x, y;

            this.size_x = size_x;
            this.size_y = size_y;
            this.resolution = resolution;
            this.theta_offset = theta_offset; // what if it's pi/2 ??? over flow?
            this.prior_occ = (float)prior_occ;
            this.occ_evidence = occ_evidence;
            this.emp_evidence = emp_evidence;
            this.max_prob = max_prob;
            this.max_sure_range = max_sure_range;
            this.max_range = max_range;
            this.wall_thickness = wall_thickness;

            try
            {
                probToken.EnterWriteLock();
                this.prob = new float[size_x, size_y];
            }
            finally
            {
                probToken.ExitWriteLock();
            }

            for (x = 0; x < size_x; x++)
                for (y = 0; y < size_y; y++)
                    SetProb(x, y, -1f);

            this.distance_table_size = (int)(max_range / resolution);
            try
            {
                distToken.EnterWriteLock();
                this.distance_table = new double[distance_table_size, distance_table_size];
            }
            finally
            {
                distToken.ExitWriteLock();
            }

            for (x = 0; x < distance_table_size; x++)
                for (y = 0; y < distance_table_size; y++)
                    SetDistance(x, y, Math.Sqrt(x * x + y * y) * resolution);

            this.first = true;
        }

        public void clear()
        {
            int x, y;

            for (x = 0; x < this.size_x; x++)
                for (y = 0; y < this.size_y; y++)
                    this.SetProb(x, y, this.prior_occ);
        }

        public void clearViewedArea()
        {
            int x, y;

            for (x = 0; x < this.size_x; x++)
                for (y = 0; y < this.size_y; y++)
                    if (GetProb(x, y) >= 1)
                    {
                        SetProb(x, y, GetProb(x, y) - (int)GetProb(x, y));
                    }
        }

        public void enlarge(int size_x, int size_y)
        {
            if (size_x <= this.size_x && size_y <= this.size_y)
                return;

            int dx = 0, dy = 0;

            if (size_x > this.size_x)
            {
                dx = (size_x - this.size_x) / 2;
                this.size_x = size_x;
            }
            if (size_y > this.size_y)
            {
                dy = (size_y - this.size_y) / 2;
                this.size_y = size_y;
            }

            float[,] newProb = new float[this.size_x, this.size_y];

            int x = 0, y = 0;
            for (x = 0; x < this.size_x; x++)
            {
                if (x < dx || x >= size_x - dx)
                {
                    for (y = 0; y < this.size_y; y++)
                        newProb[x, y] = -1;
                }
                else
                {
                    for (y = 0; y < this.size_y; y++)
                    {
                        if (y < dy || y >= size_y - dy)
                            newProb[x, y] = -1;
                        else
                        {
                            try
                            {
                                newProb[x, y] = GetProb(x - dx, y - dy);
                            }
                            catch { }
                        }
                    }
                }
            }
            try
            {
                probToken.EnterWriteLock();
                prob = newProb;
            }
            finally
            {
                probToken.ExitWriteLock();
            }

            //USARLog.println("EG:: enlarge to" + size_x + "," + size_y, 0, this.ToString());
        }

        public Pose2D transPose(Pose2D p)
        {
            double correction_angle = 0;
            double laser_x = 0, laser_y = 0, laser_theta = 0;
            double temp_laser_x, temp_laser_y;

            if (this.first)
                return new Pose2D();

            correction_angle = this.theta_offset - this.start_theta;

            laser_theta = p.Rotation + correction_angle;
            temp_laser_x = p.X * Math.Cos(correction_angle) - p.Y * Math.Sin(correction_angle);
            temp_laser_y = p.X * Math.Sin(correction_angle) + p.Y * Math.Cos(correction_angle);

            laser_x = this.size_x / 2.0 + (temp_laser_x / this.resolution - this.start_x);
            laser_y = this.size_y / 2.0 + (temp_laser_y / this.resolution - this.start_y);

            return new Pose2D(laser_x, laser_y, laser_theta - Math.PI / 2.0);
        }

        public Pose2D transPose2(Pose2D p)
        {
            double correction_angle = this.theta_offset - this.start_theta;

            double temp_laser_x = (p.X - this.size_x / 2.0 + this.start_x) * this.resolution;
            double temp_laser_y = (p.Y - this.size_y / 2.0 + this.start_y) * this.resolution;
            double laser_x = temp_laser_x * Math.Cos(correction_angle) + temp_laser_y * Math.Sin(correction_angle);
            double laser_y = (-temp_laser_y) * Math.Sin(correction_angle) + temp_laser_y * Math.Cos(correction_angle);

            return new Pose2D(laser_x, laser_y, p.Rotation - correction_angle + Math.PI / 2.0);
        }

        /// <summary>
        /// return index of world point in prob grid
        /// </summary>
        /// <param name="point">world point</param>
        /// <returns></returns>
        public Index getIndex(Pose2D point)
        {
            double temp_x, temp_y;
            double correction_angle = 0;

            correction_angle = this.theta_offset - this.start_theta;

            temp_x = point.X * Math.Cos(correction_angle) - point.Y * Math.Sin(correction_angle);
            temp_y = point.X * Math.Sin(correction_angle) + point.Y * Math.Cos(correction_angle);

            temp_x = this.size_x / 2.0 + (temp_x / this.resolution - this.start_x);
            temp_y = this.size_y / 2.0 + (temp_y / this.resolution - this.start_y);

            int xx = (int)Math.Floor(temp_x);
            int yy = (int)Math.Floor(temp_y);

            return new Index(xx, yy);
        }

        /// <summary>
        /// return world point of Index in prob grid
        /// </summary>
        /// <param name="point">index</param>
        /// <returns></returns>
        public Pose2D getReal(Index indx)
        {
            double temp_x, temp_y;
            double correction_angle = this.theta_offset - this.start_theta;

            temp_x = (indx.x - this.size_x / 2.0 + this.start_x) * this.resolution;
            temp_y = (indx.y - this.size_y / 2.0 + this.start_y) * this.resolution;
            double x = temp_x * Math.Cos(correction_angle) + temp_y * Math.Sin(correction_angle);
            double y = (-temp_y) * Math.Sin(correction_angle) + temp_y * Math.Cos(correction_angle);

            return new Pose2D(x, y, 0);
        }

        public float getRawProb(double x, double y)
        {
            double correction_angle = this.theta_offset - this.start_theta;
            double temp_x = x * Math.Cos(correction_angle) - y * Math.Sin(correction_angle);
            double temp_y = x * Math.Sin(correction_angle) + y * Math.Cos(correction_angle);
            x = this.size_x / 2.0 + (temp_x / this.resolution - this.start_x);
            y = this.size_y / 2.0 + (temp_y / this.resolution - this.start_y);

            int x_int = (int)Math.Floor(x);
            if (x_int < 0)
                x_int = 0;
            else if (x_int >= this.size_x)
                x_int = this.size_x - 1;

            int y_int = (int)Math.Floor(y);
            if (y_int < 0)
                y_int = 0;
            else if (y_int >= this.size_y)
                y_int = this.size_y - 1;

            return GetProb(x_int, y_int);
        }

        public float getOccProb(double x, double y)
        {
            double correction_angle = this.theta_offset - this.start_theta;
            double temp_x = x * Math.Cos(correction_angle) - y * Math.Sin(correction_angle);
            double temp_y = x * Math.Sin(correction_angle) + y * Math.Cos(correction_angle);
            x = this.size_x / 2.0 + (temp_x / this.resolution - this.start_x);
            y = this.size_y / 2.0 + (temp_y / this.resolution - this.start_y);

            int x_int = (int)Math.Floor(x);
            if (x_int < 0)
                x_int = 0;
            else if (x_int >= this.size_x)
                x_int = this.size_x - 1;

            int y_int = (int)Math.Floor(y);
            if (y_int < 0)
                y_int = 0;
            else if (y_int >= this.size_y)
                y_int = this.size_y - 1;

            if (GetProb(x_int, y_int) < 1)
                return GetProb(x_int, y_int);
            else
            {
                return GetProb(x_int, y_int) - (int)GetProb(x_int, y_int);
            }
        }

        public float getObsProb(double x, double y)
        {
            double correction_angle = this.theta_offset - this.start_theta;
            double temp_x = x * Math.Cos(correction_angle) - y * Math.Sin(correction_angle);
            double temp_y = x * Math.Sin(correction_angle) + y * Math.Cos(correction_angle);
            x = this.size_x / 2.0 + (temp_x / this.resolution - this.start_x);
            y = this.size_y / 2.0 + (temp_y / this.resolution - this.start_y);

            int x_int = (int)Math.Floor(x);
            if (x_int < 0)
                x_int = 0;
            else if (x_int >= this.size_x)
                x_int = this.size_x - 1;

            int y_int = (int)Math.Floor(y);
            if (y_int < 0)
                y_int = 0;
            else if (y_int >= this.size_y)
                y_int = this.size_y - 1;

            if (GetProb(x_int, y_int) < 1)
                return 0;
            else
            {
                return (int)GetProb(x_int, y_int) / 10000.0f;
            }
        }

        public Dictionary<Index, float> getOpenRooms(Index mLocation, double minDistanceFromWall, double windowLenght, int stepAngle, double threshold, ArrayList Victims)
        {
            Dictionary<Index, float> dicOpens = new Dictionary<Index, float>();

            //lock (this)
            {
                int wlRoomCount = (int)Math.Floor(windowLenght / this.resolution);
                //int mdfWall = (int)Math.Floor(minDistanceFromWall / this.resolution);

                int temp_x, temp_y;
                float temp_theta;

                //USARLog.println(string.Format("{0},{1}", mLocation.x, mLocation.y), 4, "");
                //USARLog.println("window lenght :" + mdfWall.ToString(), 3, "---------->");

                for (int angle = 0; angle < 360; angle += stepAngle)
                {
                    temp_theta = MathHelper.DegToRad(angle);
                    temp_x = mLocation.x + (int)Math.Round(wlRoomCount * Math.Cos(temp_theta));
                    temp_y = mLocation.y + (int)Math.Round(wlRoomCount * Math.Sin(temp_theta));

                    if (temp_x >= 0 && temp_x < this.size_x)
                    {
                        if (temp_y >= 0 && temp_y < this.size_y)
                        {
                            float real_Prob = GetProb(temp_x, temp_y) - (int)GetProb(temp_x, temp_y);

                            if (real_Prob <= threshold)
                            {
                                if (isRoadClear(mLocation, new Index(temp_x, temp_y), threshold, minDistanceFromWall) == null &&
                                    !isInVictimRange(new Index(temp_x, temp_y), Victims))
                                {
                                    if (real_Prob < 0) real_Prob = 0.7f;
                                    dicOpens.Add(new Index(temp_x, temp_y), real_Prob);
                                }
                            }
                        }
                    }
                }
            }
            return dicOpens;
        }

        private bool isInVictimRange(Index index, ArrayList Victims)
        {
            foreach (VictRFID v in Victims)
            {
                Index posArray = this.getIndex(new Pose2D(v.meanPos.X, -v.meanPos.Y, 0f));

                int windowL = (int)Math.Floor(0.3 / this.resolution);

                int minX = Math.Max(0, posArray.x - windowL),
                    minY = Math.Max(0, posArray.y - windowL),
                    maxY = Math.Min(this.size_y - 1, posArray.y + windowL),
                    maxX = Math.Min(this.size_x - 1, posArray.x + windowL);

                Rectangle r = Rectangle.FromLTRB(minX, minY, maxX, maxY);
                Rectangle r2 = Rectangle.FromLTRB(index.x - 1, index.y - 1, index.x + 1, index.y + 1);

                if (r.IntersectsWith(r2))
                    return true;
            }
            return false;
        }

        private bool isCircleClear(int indX, int indY, int windowL, double threshold)
        {
            int temp_x, temp_y;
            float temp_theta;

            for (int angle = 0; angle < 360; angle += 15)
            {
                temp_theta = MathHelper.DegToRad(angle);
                temp_x = indX + (int)Math.Round(windowL * Math.Cos(temp_theta));
                temp_y = indY + (int)Math.Round(windowL * Math.Sin(temp_theta));

                if (temp_x >= 0 && temp_x < this.size_x)
                {
                    if (temp_y >= 0 && temp_y < this.size_y)
                    {
                        float real_Prob = GetProb(temp_x, temp_y) - (int)GetProb(temp_x, temp_y);

                        if (real_Prob > threshold)
                            return false;
                    }
                }
            }
            return true;
        }

        private bool isWindowClear(int indX, int indY, int windowL, double threshold)
        {
            int minX = Math.Max(0, indX - windowL),
                minY = Math.Max(0, indY - windowL),
                maxY = Math.Min(this.size_y - 1, indY + windowL),
                maxX = Math.Min(this.size_x - 1, indX + windowL);
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    float real_Prob = GetProb(x, y) - (int)GetProb(x, y);

                    if (real_Prob > threshold)
                        return false;
                }
            }
            return true;
        }

        private float getWindowProb(int indX, int indY, int windowL, double wallThreshold)
        {
            float sum = 0f;

            int minX = Math.Max(0, indX - windowL),
                minY = Math.Max(0, indY - windowL),
                maxY = Math.Min(this.size_y - 1, indY + windowL),
                maxX = Math.Min(this.size_x - 1, indX + windowL);

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    float real_Prob = GetProb(x, y) - (int)GetProb(x, y);

                    if (real_Prob <= wallThreshold && real_Prob != 0f)
                    {
                        sum += real_Prob;
                        //count++;
                    }
                }
            }

            return sum;
            //if (count != 0 && sum != 0f)
            //    return sum / (float)count;
            //else
            //    return 0f;
        }

        /// <summary>
        /// is in wall ?
        /// </summary>
        /// <param name="p">world frame</param>
        /// <param name="radius"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public bool isPointInWall(Pose2D p, double radius, double threshold)
        {
            Index i = this.getIndex(p);
            return (isWindowClear(i, (int)(radius / this.resolution), threshold) != null);
        }
        //StreamWriter sw = new StreamWriter("E:\\egridLog.log");
        private Index isWindowClear(Index indx, int windowL, double threshold)
        {
            int minX = Math.Max(0, indx.x - windowL),
                minY = Math.Max(0, indx.y - windowL),
                maxY = Math.Min(this.size_y - 1, indx.y + windowL),
                maxX = Math.Min(this.size_x - 1, indx.x + windowL);

            //string cmd = "Egrid Data minX = " + minX + " minY = " + minY + " maxX = " + maxX + " maxY = " + maxY + Environment.NewLine;

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    float currP = GetProb(x, y);
                    float real_Prob = currP - (int)currP;
                    //cmd += " " + currP;

                    //if (real_Prob > threshold)
                    //    return new Index(x, y);

                    if (real_Prob > threshold || currP < 0)
                    {
                        //cmd += Environment.NewLine;
                        //USARLog.println(cmd, "");
                        return new Index(x, y);
                    }
                }
            }
            //sw.WriteLine(cmd);
            //sw.Flush();
            return null;
        }

        public virtual bool isLineOccupied(Index start, Index end, double threshold)
        {
            lock (this)
            {
                double sx = start.x;
                double sy = start.y;

                int sx_int = (int)Math.Floor(sx);
                if (sx_int < 0)
                    sx_int = 0;
                else if (sx_int >= this.size_x)
                    sx_int = this.size_x - 1;

                int sy_int = (int)Math.Floor(sy);
                if (sy_int < 0)
                    sy_int = 0;
                else if (sy_int >= this.size_y)
                    sy_int = this.size_y - 1;

                double dx = end.x;
                double dy = end.y;

                int dx_int = (int)Math.Floor(dx);
                if (dx_int < 0)
                    dx_int = 0;
                else if (dx_int >= this.size_x)
                    dx_int = this.size_x - 1;

                int dy_int = (int)Math.Floor(dy);
                if (dy_int < 0)
                    dy_int = 0;
                else if (dy_int >= this.size_y)
                    dy_int = this.size_y - 1;

                BresenhamParam b_params = new BresenhamParam(sx_int, sy_int, dx_int, dy_int);
                do
                {
                    Point curP = b_params.CurrentPoint;

                    // treat unexplored area (prob=-1) as traversable
                    Index occupedIndex = new Index(curP.X, curP.Y);

                    float real_Prob = GetProb(occupedIndex.x, occupedIndex.y) -
                                      (int)GetProb(occupedIndex.x, occupedIndex.y);

                    if (real_Prob > threshold)
                    {
                        return true;
                    }
                }
                while (b_params.nextPoint());
                return false;
            }
        }

        public virtual Pose2D  isRoadClear(Pose2D start, Pose2D end, double threshold, double roadLenght)
        {
            lock (this)
            {
                int rl = (int)Math.Floor(roadLenght / this.resolution);

                double correction_angle = this.theta_offset - this.start_theta;
                double temp_x = start.X * Math.Cos(correction_angle) - start.Y * Math.Sin(correction_angle);
                double temp_y = start.X * Math.Sin(correction_angle) + start.Y * Math.Cos(correction_angle);
                double sx = this.size_x / 2.0 + (temp_x / this.resolution - this.start_x);
                double sy = this.size_y / 2.0 + (temp_y / this.resolution - this.start_y);

                int sx_int = (int)Math.Floor(sx);
                if (sx_int < 0)
                    sx_int = 0;
                else if (sx_int >= this.size_x)
                    sx_int = this.size_x - 1;

                int sy_int = (int)Math.Floor(sy);
                if (sy_int < 0)
                    sy_int = 0;
                else if (sy_int >= this.size_y)
                    sy_int = this.size_y - 1;

                temp_x = end.X * Math.Cos(correction_angle) - end.Y * Math.Sin(correction_angle);
                temp_y = end.X * Math.Sin(correction_angle) + end.Y * Math.Cos(correction_angle);
                double dx = this.size_x / 2.0 + (temp_x / this.resolution - this.start_x);
                double dy = this.size_y / 2.0 + (temp_y / this.resolution - this.start_y);

                int dx_int = (int)Math.Floor(dx);
                if (dx_int < 0)
                    dx_int = 0;
                else if (dx_int >= this.size_x)
                    dx_int = this.size_x - 1;

                int dy_int = (int)Math.Floor(dy);
                if (dy_int < 0)
                    dy_int = 0;
                else if (dy_int >= this.size_y)
                    dy_int = this.size_y - 1;

                BresenhamParam b_params = new BresenhamParam(sx_int, sy_int, dx_int, dy_int);
                do
                {
                    Point curP = b_params.CurrentPoint;

                    // treat unexplored area (prob=-1) as traversable
                    Index occupedIndex = isWindowClear(new Index(curP.X, curP.Y), rl, threshold);
                    if (occupedIndex != null)
                    {
                        temp_x = (curP.X - this.size_x / 2.0 + this.start_x) * this.resolution;
                        temp_y = (curP.Y - this.size_y / 2.0 + this.start_y) * this.resolution;
                        double x = temp_x * Math.Cos(correction_angle) + temp_y * Math.Sin(correction_angle);
                        double y = (-temp_y) * Math.Sin(correction_angle) + temp_y * Math.Cos(correction_angle);
                        return new Pose2D(x, y, end.Rotation);
                    }
                }
                while (b_params.nextPoint());
                return null;
            }
        }

        public virtual Index isRoadClear(Index start, Index end, double threshold, double roadLenght)
        {
            lock (this)
            {
                int rl = (int)Math.Floor(roadLenght / this.resolution);

                BresenhamParam b_params = new BresenhamParam(start.x, start.y, end.x, end.y);
                do
                {
                    Point curP = b_params.CurrentPoint;

                    // treat unexplored area (prob=-1) as traversable
                    Index occupedIndex = isWindowClear(new Index(curP.X, curP.Y), rl, threshold);

                    if (occupedIndex != null) return occupedIndex;
                }
                while (b_params.nextPoint());

                return null;
            }
        }

        public virtual Pose2D isClear(Pose2D start, Pose2D end, double threshold)
        {
            double correction_angle = this.theta_offset - this.start_theta;
            double temp_x = start.X * Math.Cos(correction_angle) - start.Y * Math.Sin(correction_angle);
            double temp_y = start.X * Math.Sin(correction_angle) + start.Y * Math.Cos(correction_angle);
            double sx = this.size_x / 2.0 + (temp_x / this.resolution - this.start_x);
            double sy = this.size_y / 2.0 + (temp_y / this.resolution - this.start_y);

            int sx_int = (int)Math.Floor(sx);
            if (sx_int < 0)
                sx_int = 0;
            else if (sx_int >= this.size_x)
                sx_int = this.size_x - 1;

            int sy_int = (int)Math.Floor(sy);
            if (sy_int < 0)
                sy_int = 0;
            else if (sy_int >= this.size_y)
                sy_int = this.size_y - 1;

            temp_x = end.X * Math.Cos(correction_angle) - end.Y * Math.Sin(correction_angle);
            temp_y = end.X * Math.Sin(correction_angle) + end.Y * Math.Cos(correction_angle);
            double dx = this.size_x / 2.0 + (temp_x / this.resolution - this.start_x);
            double dy = this.size_y / 2.0 + (temp_y / this.resolution - this.start_y);

            int dx_int = (int)Math.Floor(dx);
            if (dx_int < 0)
                dx_int = 0;
            else if (dx_int >= this.size_x)
                dx_int = this.size_x - 1;

            int dy_int = (int)Math.Floor(dy);
            if (dy_int < 0)
                dy_int = 0;
            else if (dy_int >= this.size_y)
                dy_int = this.size_y - 1;

            BresenhamParam b_params = new BresenhamParam(sx_int, sy_int, dx_int, dy_int);
            float real_Prob;
            do
            {
                Point curP = b_params.CurrentPoint;

                // treat unexplored area (prob=-1) as traversable
                float GProb = GetProb(curP.X, curP.Y);
                real_Prob = GProb - (int)GProb;

                USARLog.println("prob of road is : " + real_Prob.ToString(), 2, "isClear------>");

                if (real_Prob > threshold)
                {
                    temp_x = (curP.X - this.size_x / 2.0 + this.start_x) * this.resolution;
                    temp_y = (curP.Y - this.size_y / 2.0 + this.start_y) * this.resolution;
                    double x = temp_x * Math.Cos(correction_angle) + temp_y * Math.Sin(correction_angle);
                    double y = (-temp_y) * Math.Sin(correction_angle) + temp_y * Math.Cos(correction_angle);
                    return new Pose2D(x, y, end.Rotation);
                }
            }
            while (b_params.nextPoint());
            return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void update(double laser_x, double laser_y, double laser_theta, int num_readings, double[] laser_range)
        {
            int i;
            int x1int, y1int, x2int, y2int, current_x, current_y, x_diff, y_diff;
            double d, theta, delta_theta, p_filled, temp_laser_x, temp_laser_y;
            BresenhamParam b_params;
            double new_prob, real_prob;
            double correction_angle = 0;

            if (this.first)
            {
                this.start_x = laser_x / this.resolution;
                this.start_y = laser_y / this.resolution;
                this.first = false;
            }

            correction_angle = this.theta_offset - this.start_theta;

            laser_theta = laser_theta + correction_angle;
            temp_laser_x = laser_x * Math.Cos(correction_angle) - laser_y * Math.Sin(correction_angle);
            temp_laser_y = laser_x * Math.Sin(correction_angle) + laser_y * Math.Cos(correction_angle);

            laser_x = this.size_x / 2.0 + (temp_laser_x / this.resolution - this.start_x);
            laser_y = this.size_y / 2.0 + (temp_laser_y / this.resolution - this.start_y);

            x1int = (int)Math.Floor(laser_x);
            y1int = (int)Math.Floor(laser_y);

            theta = laser_theta - Math.PI / 2.0;
            if (num_readings == USARConstant.DUALLASER_READINGS)
            {
                delta_theta = 2 * Math.PI / (double)(num_readings - 1); // vary important to dual side lasers
            }
            else
            {
                delta_theta = Math.PI / (double)(num_readings - 1); // vary important to one side lasers
            }

            for (i = 0; i < num_readings; i++)
            {
                if (laser_range[i] < Laser.max_range)
                {
                    x2int = (int)(laser_x + (laser_range[i] + this.wall_thickness) * Math.Cos(theta) / this.resolution);
                    y2int = (int)(laser_y + (laser_range[i] + this.wall_thickness) * Math.Sin(theta) / this.resolution);
                    b_params = new BresenhamParam(x1int, y1int, x2int, y2int);

                    bool bContinue = true;
                    do
                    {
                        if (!bContinue) break;

                        Point curP = b_params.CurrentPoint;
                        current_x = curP.X;
                        current_y = curP.Y;

                        if (current_x >= 0 && current_x < this.size_x && current_y >= 0 && current_y < this.size_y)
                        {
                            try
                            {
                                if (this.GetProb(current_x, current_y) > 0.8f)
                                {
                                    bContinue = false;
                                    continue;
                                }
                            }
                            catch { }

                            /* Calculate distance from laser */
                            x_diff = Math.Abs(current_x - x1int);
                            y_diff = Math.Abs(current_y - y1int);
                            if (x_diff >= this.distance_table_size || y_diff >= this.distance_table_size)
                                d = 1e6;
                            else
                                d = this.GetDistance(x_diff, y_diff);

                            if (d < laser_range[i])
                            {
                                /* Free observation */
                                if (d < this.max_sure_range)
                                    p_filled = this.emp_evidence;
                                else if (d < this.max_range)
                                    p_filled = this.emp_evidence + (d - this.max_sure_range) / this.max_range * (this.prior_occ - this.emp_evidence);
                                else
                                    break;
                            }
                            else
                            {
                                /* Filled observation */
                                if (d < this.max_sure_range)
                                    p_filled = this.occ_evidence;
                                else if (d < this.max_range)
                                    p_filled = this.occ_evidence + (d - this.max_sure_range) / this.max_range * (this.prior_occ - this.occ_evidence);
                                else
                                    break;
                            }
                            try
                            {
                                if (this.GetProb(current_x, current_y) == -1)
                                    this.SetProb(current_x, current_y, this.prior_occ);
                            }
                            catch (Exception e)
                            {
                                USARLog.println("[" + current_x + "," + current_y + "] [" + this.size_x + "," + this.size_y + "]", 5, this.ToString());
                            }

                            /* Adjust the map */
                            try
                            {
                                real_prob = GetProb(current_x, current_y);
                                if (real_prob >= 1)
                                {
                                    real_prob -= (int)GetProb(current_x, current_y);
                                }
                                new_prob = 1 - 1 / (1 + (1 - this.prior_occ) / this.prior_occ * p_filled / (1 - p_filled) * real_prob / (1 - real_prob));
                                this.SetProb(current_x, current_y, (float)new_prob + (int)GetProb(current_x, current_y));
                            }
                            catch { }
                        }
                    }
                    while (b_params.nextPoint());
                }
                theta += delta_theta;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void updateWALL(double laser_x, double laser_y, double laser_theta, int num_readings, double[] laser_range, float max_navi_prob)
        {
            int i;
            int x1int, y1int, x2int, y2int, x2int_WALL, y2int_WALL, current_x, current_y;
            double d, theta, delta_theta, temp_laser_x, temp_laser_y;
            BresenhamParam b_params;
            double new_prob, real_prob;
            double correction_angle = 0;

            if (this.first)
            {
                this.start_x = laser_x / this.resolution;
                this.start_y = laser_y / this.resolution;
                this.first = false;
            }

            Pose2D laserCenter = new Pose2D(laser_x, laser_y, laser_theta);

            correction_angle = this.theta_offset - this.start_theta;

            laser_theta = laser_theta + correction_angle;
            temp_laser_x = laser_x * Math.Cos(correction_angle) - laser_y * Math.Sin(correction_angle);
            temp_laser_y = laser_x * Math.Sin(correction_angle) + laser_y * Math.Cos(correction_angle);

            laser_x = this.size_x / 2.0 + (temp_laser_x / this.resolution - this.start_x);
            laser_y = this.size_y / 2.0 + (temp_laser_y / this.resolution - this.start_y);

            x1int = (int)Math.Floor(laser_x);
            y1int = (int)Math.Floor(laser_y);

            theta = laser_theta - Math.PI / 2.0;
            if (num_readings == USARConstant.DUALLASER_READINGS)
            {
                delta_theta = 2 * Math.PI / (double)(num_readings - 1); // vary important to dual side lasers
            }
            else
            {
                delta_theta = Math.PI / (double)(num_readings - 1); // vary important to one side lasers
            }


            for (i = 0; i < num_readings; i++)
            {
                if (laser_range[i] < Laser.max_range)
                {
                    float r = (float)laser_range[i];
                    if (r <= 10)
                    {
                        x2int_WALL = (int)(laser_x + (r) * Math.Cos(theta) / this.resolution);
                        y2int_WALL = (int)(laser_y + (r) * Math.Sin(theta) / this.resolution);

                        x2int = (int)(laser_x + (r + this.wall_thickness) * Math.Cos(theta) / this.resolution);
                        y2int = (int)(laser_y + (r + this.wall_thickness) * Math.Sin(theta) / this.resolution);

                        #region "iterating points between"

                        //b_params = new BresenhamParam(x1int, y1int, x2int_WALL, y2int_WALL);
                        //do
                        //{
                        //    Point curP = b_params.CurrentPoint;
                        //    current_x = curP.X;
                        //    current_y = curP.Y;
                        //    if (current_x >= 0 && current_x < this.size_x && current_y >= 0 && current_y < this.size_y)
                        //    {
                        //        /* Adjust the map */
                        //        real_prob = GetProb(current_x, current_y);
                        //        if (real_prob >= 1)
                        //        {
                        //            real_prob -= (int)GetProb(current_x, current_y);
                        //        }

                        //        if (real_prob > 0)
                        //        {
                        //            if (this.max_prob - real_prob < 0.01)
                        //            {
                        //                this.SetProb(current_x, current_y, -1 + (int)this.GetProb(current_x, current_y));
                        //            }
                        //        }
                        //    }
                        //}
                        //while (b_params.nextPoint());

                        #endregion

                        #region "iterating wall thickness"

                        b_params = new BresenhamParam(x2int_WALL, y2int_WALL, x2int, y2int);
                        do
                        {
                            Point curP = b_params.CurrentPoint;
                            current_x = curP.X;
                            current_y = curP.Y;
                            if (current_x >= 0 && current_x < this.size_x && current_y >= 0 && current_y < this.size_y)
                            {
                                new_prob = this.max_prob;
                                this.SetProb(current_x, current_y, (float)new_prob + (int)GetProb(current_x, current_y));
                                this.addCirclePotential(new Index(current_x, current_y), 0.5, 0.3, max_navi_prob);
                            }
                        }
                        while (b_params.nextPoint());

                        #endregion
                    }
                }
                theta += delta_theta;
            }
        }

        public void addDotPotential(Index current, Index startPos, double max_incremention_radius, double sure_inc_range, double max_potential)
        {
            int x1int, y1int, current_x, current_y, x_diff, y_diff;
            double d, p_filled;
            double new_prob, real_prob;

            real_prob = GetProb(current.x, current.y);
            if (real_prob >= 1)
            {
                real_prob -= (int)GetProb(current.x, current.y);
            }

            if (real_prob >= max_potential)
                return;


            current_x = current.x;
            current_y = current.y;

            x1int = startPos.x;
            y1int = startPos.y;

            x_diff = Math.Abs(current_x - x1int);
            y_diff = Math.Abs(current_y - y1int);

            if (x_diff >= this.distance_table_size || y_diff >= this.distance_table_size)
                d = 1e6;
            else
                d = this.GetDistance(x_diff, y_diff);

            /* Filled observation */
            p_filled = this.occ_evidence + (d - sure_inc_range) / max_incremention_radius * (this.prior_occ - this.occ_evidence);

            try
            {
                if (this.GetProb(current_x, current_y) == -1)
                    this.SetProb(current_x, current_y, this.prior_occ);
            }
            catch (Exception e)
            {
                USARLog.println("[" + current_x + "," + current_y + "] [" + this.size_x + "," + this.size_y + "]", 5, this.ToString());
            }

            ///* Adjust the map */
            real_prob = GetProb(current_x, current_y);
            if (real_prob >= 1)
            {
                real_prob -= (int)GetProb(current_x, current_y);
            }

            new_prob = max_potential - ((max_potential - real_prob) * 0.98);
            //new_prob = 1 - 1 / (1 + (1 - this.prior_occ) / this.prior_occ * p_filled / (1 - p_filled) * real_prob / (1 - real_prob));

            //new_prob = (max_incremention_radius - d) / (max_incremention_radius);  
            //new_prob *= new_prob / 5;

            if (new_prob > max_potential)
            {
                new_prob = max_potential;
            }
            this.SetProb(current_x, current_y, (float)new_prob + (int)GetProb(current_x, current_y));
        }

        public float getCirclePotential(Pose2D point, double Radius)
        {
            Index posArray = this.getIndex(point);

            int rad = (int)Math.Floor(Radius / this.resolution);

            int minX = posArray.x - rad,
                maxX = posArray.x + rad,
                minY = posArray.y - rad,
                maxY = posArray.y + rad;

            if (minX < 0)
                minX = 0;
            if (maxX >= this.size_x)
                maxX = this.size_x - 1;

            if (minY < 0)
                minY = 0;
            if (maxY >= this.size_y)
                maxY = this.size_y - 1;

            float dist;
            float sum = 0f;
            int count = 0;

            for (int x = minX; x < maxX; x++)
                for (int y = minY; y < maxY; y++)
                {
                    dist = posArray.getDistance(new Index(x, y)) * (float)this.resolution;

                    if (dist > Radius)
                        continue;

                    float real_Prob = GetProb(x, y) - (int)GetProb(x, y);

                    if (real_Prob != 0f)
                    {
                        sum += real_Prob;
                        count++;
                    }
                }

            if (count != 0 && sum != 0f)
                return sum / (float)count;
            else
                return 0f;
        }

        public float getCircleSumPotential(Pose2D point, double Radius)
        {
            Index posArray = this.getIndex(point);

            int rad = (int)Math.Floor(Radius / this.resolution);

            int minX = posArray.x - rad,
                maxX = posArray.x + rad,
                minY = posArray.y - rad,
                maxY = posArray.y + rad;

            if (minX < 0)
                minX = 0;
            if (maxX >= this.size_x)
                maxX = this.size_x - 1;

            if (minY < 0)
                minY = 0;
            if (maxY >= this.size_y)
                maxY = this.size_y - 1;

            float dist;
            float sum = 0f;
            int count = 0;

            for (int x = minX; x < maxX; x++)
                for (int y = minY; y < maxY; y++)
                {
                    dist = posArray.getDistance(new Index(x, y)) * (float)this.resolution;

                    if (dist > Radius)
                        continue;

                    float real_Prob = GetProb(x, y) - (int)GetProb(x, y);

                    sum += real_Prob;
                }

            return sum;
        }

        public void addCirclePotential(Pose2D point, double Radius, double sureRange, double max_potential)
        {
            Index posArray = this.getIndex(point);

            int rad = (int)Math.Floor(Radius / this.resolution);

            float real_prob = this.getOccProb(point.X, point.Y);

            // this is no wall 
            if (real_prob > max_potential)
                return;

            int minX = posArray.x - rad,
                maxX = posArray.x + rad,
                minY = posArray.y - rad,
                maxY = posArray.y + rad;

            if (minX < 0)
                minX = 0;
            if (maxX >= this.size_x)
                maxX = this.size_x - 1;

            if (minY < 0)
                minY = 0;
            if (maxY >= this.size_y)
                maxY = this.size_y - 1;

            float dist;

            for (int x = minX; x < maxX; x++)
                for (int y = minY; y < maxY; y++)
                {
                    dist = posArray.getDistance(new Index(x, y)) * (float)this.resolution;

                    if (dist > Radius)
                        continue;

                    if (isLineOccupied(posArray, new Index(x, y), 0.9f))
                        continue;

                    addDotPotential(new Index(x, y), posArray, Radius, sureRange, max_potential);
                }
        }

        public void addCirclePotential(Index posArray, double Radius, double sureRange, double max_potential)
        {
            int rad = (int)Math.Floor(Radius / this.resolution);

            float real_prob = GetProb(posArray.x, posArray.y) - (int)GetProb(posArray.x, posArray.y);

            // this is no wall 
            if (real_prob > max_potential)
                return;

            int minX = posArray.x - rad,
                maxX = posArray.x + rad,
                minY = posArray.y - rad,
                maxY = posArray.y + rad;

            if (minX < 0)
                minX = 0;
            if (maxX >= this.size_x)
                maxX = this.size_x - 1;

            if (minY < 0)
                minY = 0;
            if (maxY >= this.size_y)
                maxY = this.size_y - 1;

            float dist;

            for (int x = minX; x < maxX; x++)
                for (int y = minY; y < maxY; y++)
                {
                    dist = posArray.getDistance(new Index(x, y)) * (float)this.resolution;

                    if (dist > Radius)
                        continue;

                    addDotPotential(new Index(x, y), posArray, Radius, sureRange, max_potential);
                }
        }

        public void addCircleWall(Pose2D point, double Radius, double maxNaviProb)
        {
            Index posArray = this.getIndex(point);
            //USARLog.println("index of CW : " + posArray.ToString(), 0, this.ToString());

            int rad = (int)Math.Floor(Radius / this.resolution);

            float real_prob = this.getOccProb(point.X, point.Y);

            //// this is no wall 
            if (real_prob > this.max_prob)
                return;

            int minX = posArray.x - rad,
                maxX = posArray.x + rad,
                minY = posArray.y - rad,
                maxY = posArray.y + rad;

            if (minX < 0)
                minX = 0;
            if (maxX >= this.size_x)
                maxX = this.size_x - 1;

            if (minY < 0)
                minY = 0;
            if (maxY >= this.size_y)
                maxY = this.size_y - 1;

            float dist;

            for (int x = minX; x < maxX; x++)
                for (int y = minY; y < maxY; y++)
                {
                    dist = posArray.getDistance(new Index(x, y)) * (float)this.resolution;

                    if (dist > Radius)
                        continue;

                    //double new_prob = this.max_prob;
                    //this.prob[x, y] = (float)new_prob + (int)prob[x, y];
                    addDotPotential(new Index(x, y), posArray, Radius, Radius, maxNaviProb);
                }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void obsUpdate(double cam_x, double cam_y, double cam_theta, double cam_fov, double cam_resolution)
        {
            int i;
            int x1int, y1int, x2int, y2int, current_x, current_y, x_diff, y_diff;
            double d, theta, delta_theta, p_filled, temp_cam_x, temp_cam_y;
            BresenhamParam b_params;
            double new_prob, real_prob;
            double correction_angle = 0;

            correction_angle = this.theta_offset - this.start_theta;

            cam_theta = cam_theta + correction_angle;
            temp_cam_x = cam_x * Math.Cos(correction_angle) - cam_y * Math.Sin(correction_angle);
            temp_cam_y = cam_x * Math.Sin(correction_angle) + cam_y * Math.Cos(correction_angle);

            cam_x = this.size_x / 2.0 + (temp_cam_x / this.resolution - this.start_x);
            cam_y = this.size_y / 2.0 + (temp_cam_y / this.resolution - this.start_y);

            x1int = (int)Math.Floor(cam_x);
            y1int = (int)Math.Floor(cam_y);
            theta = cam_theta - cam_fov / 2.0;
            delta_theta = cam_resolution;

            for (theta = cam_theta + cam_fov / 2.0; theta > cam_theta - cam_fov / 2.0 - delta_theta; theta -= delta_theta)
            {
                x2int = (int)(cam_x + (this.max_range + this.wall_thickness) * Math.Cos(theta) / this.resolution);
                y2int = (int)(cam_y + (this.max_range + this.wall_thickness) * Math.Sin(theta) / this.resolution);
                b_params = new BresenhamParam(x1int, y1int, x2int, y2int);
                do
                {
                    Point curP = b_params.CurrentPoint;
                    current_x = curP.X;
                    current_y = curP.Y;
                    if (current_x >= 0 && current_x < this.size_x && current_y >= 0 && current_y < this.size_y)
                    {
                        // detecting walls
                        real_prob = GetProb(current_x, current_y);
                        if (real_prob >= 1)
                        {
                            real_prob = real_prob - (int)real_prob;
                        }
                        if (real_prob > 0.7 || real_prob == -1)
                            break;

                        /* Calculate distance from camera */
                        x_diff = Math.Abs(current_x - x1int);
                        y_diff = Math.Abs(current_y - y1int);
                        if (x_diff >= this.distance_table_size || y_diff >= this.distance_table_size)
                            break;
                        else
                            d = this.GetDistance(x_diff, y_diff);

                        /* 0->extensivly observed; 1->ignored area */
                        if (d < this.max_sure_range)
                            p_filled = this.emp_evidence;
                        else
                            p_filled = this.emp_evidence + (d - this.max_sure_range) / this.max_range * (this.prior_occ - this.emp_evidence);

                        /* Adjust the map */
                        if (GetProb(current_x, current_y) >= 1)
                        {
                            real_prob = (int)GetProb(current_x, current_y) / 10000.0;
                        }
                        else
                            real_prob = this.prior_occ;

                        new_prob = 1 - 1 / (1 + (1 - this.prior_occ) / this.prior_occ * p_filled / (1 - p_filled) * real_prob / (1 - real_prob));
                        if (new_prob < 0.0001)
                            new_prob = 0.0001;

                        new_prob = (int)(new_prob * 10000.0) + GetProb(current_x, current_y) - (int)GetProb(current_x, current_y);
                        this.SetProb(current_x, current_y, (float)new_prob);
                    }
                }
                while (b_params.nextPoint());
            }
        }

        public double getRangePotential(double cam_x, double cam_y, double cam_theta, double cam_fov, double cam_resolution, double avgRange)
        {
            int x1int, y1int, x2int, y2int, current_x, current_y;
            double theta, delta_theta, temp_cam_x, temp_cam_y;
            BresenhamParam b_params;
            double real_prob;
            double correction_angle = 0;

            correction_angle = this.theta_offset - this.start_theta;

            cam_theta = cam_theta + correction_angle;
            temp_cam_x = cam_x * Math.Cos(correction_angle) - cam_y * Math.Sin(correction_angle);
            temp_cam_y = cam_x * Math.Sin(correction_angle) + cam_y * Math.Cos(correction_angle);

            cam_x = this.size_x / 2.0 + (temp_cam_x / this.resolution - this.start_x);
            cam_y = this.size_y / 2.0 + (temp_cam_y / this.resolution - this.start_y);

            x1int = (int)Math.Floor(cam_x);
            y1int = (int)Math.Floor(cam_y);
            theta = cam_theta - cam_fov / 2.0;
            delta_theta = cam_resolution;

            double sum = 0f;
            int count = 0;

            for (theta = cam_theta + cam_fov / 2.0; theta > cam_theta - cam_fov / 2.0 - delta_theta; theta -= delta_theta)
            {
                x2int = (int)(cam_x + (avgRange) * Math.Cos(theta) / this.resolution);
                y2int = (int)(cam_y + (avgRange) * Math.Sin(theta) / this.resolution);
                b_params = new BresenhamParam(x1int, y1int, x2int, y2int);
                do
                {
                    Point curP = b_params.CurrentPoint;
                    current_x = curP.X;
                    current_y = curP.Y;
                    if (current_x >= 0 && current_x < this.size_x && current_y >= 0 && current_y < this.size_y)
                    {
                        real_prob = GetProb(current_x, current_y);
                        if (real_prob >= 1)
                        {
                            real_prob = real_prob - (int)real_prob;
                        }

                        if (real_prob > 0.7 || real_prob == -1)
                            break;

                        sum += real_prob;
                        count++;
                    }
                }
                while (b_params.nextPoint());
            }

            if (count == 0) count = 1;

            return sum / (double)count;
        }

        public void finish(int downsample, int border)
        {
            int min_x = 0, min_y = 0, max_x = 0, max_y = 0;
            int x, y, i, j, count, size_x, size_y;
            double sum;
            float[,] prob2;

            size_x = this.size_x / downsample;
            size_y = this.size_y / downsample;
            prob2 = new float[size_x, size_y];

            for (x = 0; x < size_x; x++)
                for (y = 0; y < size_y; y++)
                {
                    count = 0;
                    sum = 0;
                    for (i = x * downsample; i < (x + 1) * downsample; i++)
                        for (j = y * downsample; j < (y + 1) * downsample; j++)
                            if (i < this.size_x && j < this.size_y && this.GetProb(i, j) != -1)
                            {
                                count++;
                                sum += this.GetProb(i, j);
                            }
                    if (count == 0)
                        prob2[x, y] = -1;
                    else
                    {
                        prob2[x, y] = (float)(sum / count);
                    }
                }

            min_x = size_x - border - 1;
            min_y = size_y - border - 1;
            max_x = border;
            max_y = border;

            for (x = 0; x < size_x; x++)
                for (y = 0; y < size_y; y++)
                    if (prob2[x, y] != -1)
                    {
                        if (x < min_x)
                            min_x = x;
                        if (x > max_x)
                            max_x = x;
                        if (y < min_y)
                            min_y = y;
                        if (y > max_y)
                            max_y = y;
                    }
            min_x -= border;
            if (min_x < 0)
                min_x = 0;
            min_y -= border;
            if (min_y < 0)
                min_y = 0;
            max_x += border;
            if (max_x >= size_x)
                max_x = size_x - 1;
            max_y += border;
            if (max_y >= size_y)
                max_y = size_y - 1;

            this.size_x = max_x - min_x;
            this.size_y = max_y - min_y;
            this.resolution *= downsample;

            try
            {
                probToken.EnterWriteLock();
                this.prob = new float[this.size_x, this.size_y];
            }
            finally
            {
                probToken.ExitWriteLock();
            }

            for (x = 0; x < this.size_x; x++)
                for (y = 0; y < this.size_y; y++)
                    this.SetProb(x, y, prob2[x + min_x, y + min_y]);
        }

        public float getAreaAverageProbs(Pose2D pCenter, float startAngle, float deltaAngle, float samplingRadius, float windowLength, double maxNaviProb)
        {
            int x1int, y1int, x2int, y2int;
            double laser_x, laser_y, laser_theta;
            double theta, temp_laser_x, temp_laser_y;
            double correction_angle = 0;

            int rad = (int)Math.Floor(windowLength / this.resolution);
            float destAngle = startAngle + deltaAngle / 2f;

            laser_x = pCenter.X;
            laser_y = pCenter.Y;
            laser_theta = pCenter.Rotation;

            correction_angle = this.theta_offset - this.start_theta;

            laser_theta = laser_theta + correction_angle;
            temp_laser_x = laser_x * Math.Cos(correction_angle) - laser_y * Math.Sin(correction_angle);
            temp_laser_y = laser_x * Math.Sin(correction_angle) + laser_y * Math.Cos(correction_angle);

            laser_x = this.size_x / 2.0 + (temp_laser_x / this.resolution - this.start_x);
            laser_y = this.size_y / 2.0 + (temp_laser_y / this.resolution - this.start_y);

            x1int = (int)Math.Floor(laser_x);
            y1int = (int)Math.Floor(laser_y);

            theta = pCenter.Rotation - Math.PI / 2.0;
            destAngle = MathHelper.DegToRad(destAngle);

            theta += destAngle;

            x2int = (int)(laser_x + (samplingRadius) * Math.Cos(theta) / this.resolution);
            y2int = (int)(laser_y + (samplingRadius) * Math.Sin(theta) / this.resolution);

            BresenhamParam pW = new BresenhamParam(x1int, y1int, x2int, y2int);

            //USARLog.println(string.Format("{0},{1}", x1int, y1int), 4, "");
            //USARLog.println(string.Format("{0},{1}", x2int, y2int), 4, "");

            int count = 0;
            float sumP = 0f;

            do
            {
                Index pInd = new Index(pW.CurrentPoint.X, pW.CurrentPoint.Y);
                if (pInd.x >= 0 && pInd.x < this.size_x && pInd.y >= 0 && pInd.y < this.size_y)
                {
                    //USARLog.println(string.Format("{0},{1}", pInd.x, pInd.y), 4, "");
                    float real_Prob = this.getWindowProb(pInd.x, pInd.y, rad, maxNaviProb);

                    if (real_Prob != 0)
                    {
                        sumP += real_Prob;
                        count++;
                    }
                }
            } while (pW.nextPoint());

            //USARLog.println("sumP : " + sumP, 4, "egrid");

            if (count != 0 && sumP != 0f)
                return sumP / (float)count;
            else
                return 0f;
        }

        public float getAveragePotentialBetween(Index start, Index end)
        {
            lock (this)
            {
                double sx = start.x;
                double sy = start.y;

                int sx_int = (int)Math.Floor(sx);
                if (sx_int < 0)
                    sx_int = 0;
                else if (sx_int >= this.size_x)
                    sx_int = this.size_x - 1;

                int sy_int = (int)Math.Floor(sy);
                if (sy_int < 0)
                    sy_int = 0;
                else if (sy_int >= this.size_y)
                    sy_int = this.size_y - 1;

                double dx = end.x;
                double dy = end.y;

                int dx_int = (int)Math.Floor(dx);
                if (dx_int < 0)
                    dx_int = 0;
                else if (dx_int >= this.size_x)
                    dx_int = this.size_x - 1;

                int dy_int = (int)Math.Floor(dy);
                if (dy_int < 0)
                    dy_int = 0;
                else if (dy_int >= this.size_y)
                    dy_int = this.size_y - 1;

                BresenhamParam b_params = new BresenhamParam(sx_int, sy_int, dx_int, dy_int);

                //USARLog.println(string.Format("{0},{1}", x1int, y1int), 4, "");
                //USARLog.println(string.Format("{0},{1}", x2int, y2int), 4, "");

                int count = 0;
                float sumP = 0f;

                do
                {
                    Index pInd = new Index(b_params.CurrentPoint.X, b_params.CurrentPoint.Y);
                    if (pInd.x >= 0 && pInd.x < this.size_x && pInd.y >= 0 && pInd.y < this.size_y)
                    {
                        //USARLog.println(string.Format("{0},{1}", pInd.x, pInd.y), 4, "");
                        float real_Prob = GetProb(pInd.x, pInd.y) - (int)GetProb(pInd.x, pInd.y);

                        if (real_Prob != 0)
                        {
                            sumP += real_Prob;
                            count++;
                        }
                    }
                } while (b_params.nextPoint());

                //USARLog.println("sumP : " + sumP, 4, "egrid");

                if (count != 0 && sumP != 0f)
                    return sumP / count;
                else
                    return 0f;
            }
        }

        public float getRoadPotential(Pose2D start, Pose2D end, double roadLenght)
        {
            lock (this)
            {
                int rl = (int)Math.Floor(roadLenght / this.resolution);

                double correction_angle = this.theta_offset - this.start_theta;
                double temp_x = start.X * Math.Cos(correction_angle) - start.Y * Math.Sin(correction_angle);
                double temp_y = start.X * Math.Sin(correction_angle) + start.Y * Math.Cos(correction_angle);
                double sx = this.size_x / 2.0 + (temp_x / this.resolution - this.start_x);
                double sy = this.size_y / 2.0 + (temp_y / this.resolution - this.start_y);

                int sx_int = (int)Math.Floor(sx);
                if (sx_int < 0)
                    sx_int = 0;
                else if (sx_int >= this.size_x)
                    sx_int = this.size_x - 1;

                int sy_int = (int)Math.Floor(sy);
                if (sy_int < 0)
                    sy_int = 0;
                else if (sy_int >= this.size_y)
                    sy_int = this.size_y - 1;

                temp_x = end.X * Math.Cos(correction_angle) - end.Y * Math.Sin(correction_angle);
                temp_y = end.X * Math.Sin(correction_angle) + end.Y * Math.Cos(correction_angle);
                double dx = this.size_x / 2.0 + (temp_x / this.resolution - this.start_x);
                double dy = this.size_y / 2.0 + (temp_y / this.resolution - this.start_y);

                int dx_int = (int)Math.Floor(dx);
                if (dx_int < 0)
                    dx_int = 0;
                else if (dx_int >= this.size_x)
                    dx_int = this.size_x - 1;

                int dy_int = (int)Math.Floor(dy);
                if (dy_int < 0)
                    dy_int = 0;
                else if (dy_int >= this.size_y)
                    dy_int = this.size_y - 1;

                BresenhamParam b_params = new BresenhamParam(sx_int, sy_int, dx_int, dy_int);

                int count = 0;
                float sumP = 0f;

                do
                {
                    Point curP = b_params.CurrentPoint;

                    float windowProb = getWindowProb(curP.X, curP.Y, rl, max_prob);

                    if (windowProb != 0)
                    {
                        sumP += windowProb;
                        //count++;
                    }
                }
                while (b_params.nextPoint());

                return sumP;
                //if (count != 0 && sumP != 0f)
                //    return sumP / count;
                //else
                //    return 0f;
            }
        }

        public void addCircleWallPotential(Pose2D point, double Radius, double sureRange, double max_potential)
        {
            Index posArray = this.getIndex(point);
            //USARLog.println("index of CW : " + posArray.ToString(), 0, this.ToString());

            int rad = (int)Math.Floor(Radius / this.resolution);

            float real_prob = this.getOccProb(point.X, point.Y);

            //// this is no wall 
            if (real_prob > this.max_prob)
                return;

            int minX = posArray.x - rad,
                maxX = posArray.x + rad,
                minY = posArray.y - rad,
                maxY = posArray.y + rad;

            if (minX < 0)
                minX = 0;
            if (maxX >= this.size_x)
                maxX = this.size_x - 1;

            if (minY < 0)
                minY = 0;
            if (maxY >= this.size_y)
                maxY = this.size_y - 1;

            float dist;

            double new_prob = this.max_prob;
            this.SetProb(posArray.x, posArray.y, (float)new_prob + (int)GetProb(posArray.x, posArray.y));

            for (int x = minX; x < maxX; x++)
                for (int y = minY; y < maxY; y++)
                {
                    Index curIndx = new Index(x, y);
                    dist = posArray.getDistance(curIndx) * (float)this.resolution;

                    if (dist > Radius)
                        continue;

                    if (posArray.Equals(curIndx))
                        continue;

                    new_prob = this.max_prob;
                    this.SetProb(x, y, (float)new_prob + (int)GetProb(x, y));

                    //addDotPotential(curIndx, posArray, Radius, sureRange, max_potential);
                }
        }

        public double getRemainedUnexplored(List<Index> posArray, double radius)
        {
            int totalRooms = this.size_x * this.size_y;
            int totalUnexploredRooms = 0;

            for (int x = 0; x < this.size_x; x++)
                for (int y = 0; y < this.size_y; y++)
                {
                    double minDist = double.MaxValue;
                    foreach (Index historyPoint in posArray)
                    {
                        double dist = historyPoint.getDistance(new Index(x, y)) * (float)this.resolution;
                        if (dist < minDist)
                            minDist = dist;
                    }

                    if (minDist > radius)
                    {
                        float real_prob = GetProb(x, y) - (int)GetProb(x, y);

                        if (real_prob <= 0)
                            totalUnexploredRooms++;
                    }
                }

            if (totalRooms == 0) totalRooms = 1;

            return (totalUnexploredRooms / totalRooms);
        }

        public int getUnexploredCount()
        {
            int totalUnexploredRooms = 0;
            for (int x = 0; x < this.size_x; x++)
                for (int y = 0; y < this.size_y; y++)
                {
                    float real_prob = GetProb(x, y) - (int)GetProb(x, y);

                    if (real_prob <= 0)
                        totalUnexploredRooms++;
                }

            return totalUnexploredRooms;
        }

        public int getHypoxploredCount(Index posArray, double Radius)
        {
            int totalHypoexploredCount = 0;

            int rad = (int)Math.Floor(Radius / this.resolution);

            int minX = posArray.x - rad,
                maxX = posArray.x + rad,
                minY = posArray.y - rad,
                maxY = posArray.y + rad;

            if (minX < 0)
                minX = 0;
            if (maxX >= this.size_x)
                maxX = this.size_x - 1;

            if (minY < 0)
                minY = 0;
            if (maxY >= this.size_y)
                maxY = this.size_y - 1;

            float dist = 0;

            for (int x = minX; x < maxX; x++)
                for (int y = minY; y < maxY; y++)
                {
                    dist = posArray.getDistance(new Index(x, y)) * (float)this.resolution;

                    if (dist > Radius)
                        continue;

                    float real_prob = GetProb(x, y) - (int)GetProb(x, y);

                    if (real_prob <= 0)
                        totalHypoexploredCount++;
                }

            return totalHypoexploredCount;
        }

        public int getHypoxploredListCount(List<Index> posArray, double Radius)
        {
            int totalHypoexploredCount = 0;

            int rad = (int)Math.Floor(Radius / this.resolution);

            int MinXA = int.MaxValue;
            int MinYA = int.MaxValue;
            int MaxXA = int.MinValue;
            int MaxYA = int.MinValue;
            foreach (Index historyPoint in posArray)
            {
                if (MinXA > historyPoint.x) MinXA = historyPoint.x;
                if (MinYA > historyPoint.y) MinYA = historyPoint.y;
                if (MaxXA < historyPoint.x) MaxXA = historyPoint.x;
                if (MaxYA < historyPoint.y) MaxYA = historyPoint.y;
            }

            int minX = MinXA - rad,
                maxX = MaxXA + rad,
                minY = MinYA - rad,
                maxY = MaxYA + rad;

            if (minX < 0)
                minX = 0;
            if (maxX >= this.size_x)
                maxX = this.size_x - 1;

            if (minY < 0)
                minY = 0;
            if (maxY >= this.size_y)
                maxY = this.size_y - 1;

            float dist = 0, minDist = 0;

            for (int x = minX; x < maxX; x++)
                for (int y = minY; y < maxY; y++)
                {
                    minDist = float.MaxValue;
                    foreach (Index historyPoint in posArray)
                    {
                        dist = historyPoint.getDistance(new Index(x, y)) * (float)this.resolution;
                        if (dist < minDist)
                            minDist = dist;
                    }

                    if (minDist > Radius)
                        continue;

                    float real_prob = GetProb(x, y) - (int)GetProb(x, y);

                    if (real_prob <= 0)
                        totalHypoexploredCount++;
                }

            return totalHypoexploredCount;
        }

        public int countObstacle(Pose2D start, Pose2D end, double threshold)
        {
            int count = 0;
            double prevObsX = -1;
            double prevObsY = -1;

            double correction_angle = this.theta_offset - this.start_theta;
            double temp_x = start.X * Math.Cos(correction_angle) - start.Y * Math.Sin(correction_angle);
            double temp_y = start.X * Math.Sin(correction_angle) + start.Y * Math.Cos(correction_angle);
            double sx = this.size_x / 2.0 + (temp_x / this.resolution - this.start_x);
            double sy = this.size_y / 2.0 + (temp_y / this.resolution - this.start_y);

            int sx_int = (int)Math.Floor(sx);
            if (sx_int < 0)
                sx_int = 0;
            else if (sx_int >= this.size_x)
                sx_int = this.size_x - 1;

            int sy_int = (int)Math.Floor(sy);
            if (sy_int < 0)
                sy_int = 0;
            else if (sy_int >= this.size_y)
                sy_int = this.size_y - 1;

            temp_x = end.X * Math.Cos(correction_angle) - end.Y * Math.Sin(correction_angle);
            temp_y = end.X * Math.Sin(correction_angle) + end.Y * Math.Cos(correction_angle);
            double dx = this.size_x / 2.0 + (temp_x / this.resolution - this.start_x);
            double dy = this.size_y / 2.0 + (temp_y / this.resolution - this.start_y);

            int dx_int = (int)Math.Floor(dx);
            if (dx_int < 0)
                dx_int = 0;
            else if (dx_int >= this.size_x)
                dx_int = this.size_x - 1;

            int dy_int = (int)Math.Floor(dy);
            if (dy_int < 0)
                dy_int = 0;
            else if (dy_int >= this.size_y)
                dy_int = this.size_y - 1;

            BresenhamParam b_params = new BresenhamParam(sx_int, sy_int, dx_int, dy_int);
            float real_Prob;
            do
            {
                Point curP = b_params.CurrentPoint;

                // treat unexplored area (prob=-1) as traversable
                real_Prob = GetProb(curP.X, curP.Y) - (int)GetProb(curP.X, curP.Y);

                USARLog.println("prob of road is : " + real_Prob.ToString(), 2, "isClear------>");

                if (real_Prob > threshold)
                {

                    temp_x = (curP.X - this.size_x / 2.0 + this.start_x) * this.resolution;
                    temp_y = (curP.Y - this.size_y / 2.0 + this.start_y) * this.resolution;
                    double x = temp_x * Math.Cos(correction_angle) + temp_y * Math.Sin(correction_angle);
                    double y = (-temp_y) * Math.Sin(correction_angle) + temp_y * Math.Cos(correction_angle);

                    if (prevObsX == -1)
                    {
                        prevObsX = x;
                        prevObsY = y;
                        count++;
                    }
                    else
                    {
                        double dist = MathHelper.getDistance(prevObsX, prevObsY, x, y);
                        if (dist > this.wall_thickness)
                            count++;

                        prevObsX = x;
                        prevObsY = y;
                    }

                    //return new Pose2D(x, y, end.theta);
                }
            }
            while (b_params.nextPoint());
            //return null;
            return count;
        }

        public double getScanLiklihood(double laser_x, double laser_y, double laser_theta,
                                       int num_readings, double[] laser_range, double max_potential)
        {
            int i;
            int x1int, y1int, x2int, y2int, x2int_WALL, y2int_WALL, current_x, current_y, x_diff, y_diff;
            double d, theta, delta_theta, temp_laser_x, temp_laser_y;
            BresenhamParam b_params;
            double real_prob;
            double correction_angle = 0;

            if (this.first)
            {
                this.start_x = laser_x / this.resolution;
                this.start_y = laser_y / this.resolution;
                this.first = false;
            }

            correction_angle = this.theta_offset - this.start_theta;

            laser_theta = laser_theta + correction_angle;
            temp_laser_x = laser_x * Math.Cos(correction_angle) - laser_y * Math.Sin(correction_angle);
            temp_laser_y = laser_x * Math.Sin(correction_angle) + laser_y * Math.Cos(correction_angle);

            laser_x = this.size_x / 2.0 + (temp_laser_x / this.resolution - this.start_x);
            laser_y = this.size_y / 2.0 + (temp_laser_y / this.resolution - this.start_y);

            x1int = (int)Math.Floor(laser_x);
            y1int = (int)Math.Floor(laser_y);

            theta = laser_theta - Math.PI / 2.0;
            if (num_readings == USARConstant.DUALLASER_READINGS)
            {
                delta_theta = 2 * Math.PI / (double)(num_readings - 1); // vary important to dual side lasers
            }
            else
            {
                delta_theta = Math.PI / (double)(num_readings - 1); // vary important to one side lasers
            }

            double sum = 0;
            double nb_points = 0;

            for (i = 0; i < num_readings; i++)
            {
                double r = laser_range[i];
                if (laser_range[i] < Laser.max_range)
                {
                    x2int_WALL = (int)(laser_x + r * Math.Cos(theta) / this.resolution);
                    y2int_WALL = (int)(laser_y + r * Math.Sin(theta) / this.resolution);

                    x2int = (int)(laser_x + (r + this.wall_thickness) * Math.Cos(theta) / this.resolution);
                    y2int = (int)(laser_y + (r + this.wall_thickness) * Math.Sin(theta) / this.resolution);

                    b_params = new BresenhamParam(x2int_WALL, y2int_WALL, x2int, y2int);
                    do
                    {
                        Point curP = b_params.CurrentPoint;
                        current_x = curP.X;
                        current_y = curP.Y;

                        if (current_x >= 0 && current_x < this.size_x && current_y >= 0 && current_y < this.size_y)
                        {
                            try
                            {
                                real_prob = GetProb(current_x, current_y);
                                if (real_prob >= 1)
                                {
                                    real_prob -= (int)GetProb(current_x, current_y);
                                }

                                if (real_prob > max_potential)
                                    nb_points++;

                            }
                            catch { }
                        }
                    }
                    while (b_params.nextPoint());
                }
                theta += delta_theta;
            }

            return nb_points;
        }

    }

}
