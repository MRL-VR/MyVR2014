using System;
using System.Collections.Generic;
using MRL.CustomMath;
using MRL.Utils;

namespace MRL.Exploration
{
    public enum EdgeType { Corner, EndOfWall, Intersection };
    public class Edge
    {
        public Pose3D point;
        private Pose3D firstObservedRobotPos = null;
        public EdgeType type;
        public List<Line> lines;
        public float angleBetweenLines = -1f;

        ///////////////////////////////
        public float corrolation = 0f;
        ///////////////////////////////
        public int lastUpdatedTimeCycle = 0;
        public int timeCycleCreated = 0;

        public Edge(Pose3D point, Pose3D robotPos, EdgeType type, List<Line> connectedLines, int timeCycle)
        {
            this.type = type;
            this.point = point;
            this.lines = connectedLines;
            this.firstObservedRobotPos = robotPos;
            updateAngleBetweenLines();
            this.nearestPointRobotObserved = firstObservedRobotPos;
            nearestDistanceUpdated = firstObservedRobotPos.getDistance2D(point);
            newObservationAccuracyP = getAccuracyDistance(nearestDistanceUpdated);
            updateTime++;
            updateMaxDistanceToMatch();
            timeCycleCreated = timeCycle;
            lastUpdatedTimeCycle = timeCycle;
        }

        public Edge(Pose3D point, EdgeType type, List<Line> lines, float accuracyP)
        {
            this.lines = null;
            this.type = type;
            this.point = point;
            this.newObservationAccuracyP = accuracyP;
            this.lines = lines;
            updateAngleBetweenLines();
        }

        private void updateAngleBetweenLines()
        {
            if (lines != null && lines.Count == 2)
            {
                Pose3D t1 = lines[0].head - lines[0].tail,
                    t2 = lines[1].head - lines[1].tail;
                //angleBetweenLines = Math.Abs(MathHelper.VectorRadian(t1, t2));
                angleBetweenLines = MathHelper.AngleBetweenLines(t1, t2);
            }
            else
            {
                angleBetweenLines = -1f;
            }

        }
        //public override bool Equals(object obj)
        //{
        //    if (this.GetType() != obj.GetType())
        //        return false;
        //    return point.Equals(((Edge)obj).point);
        //}

        public int updateTime = 0;
        private float nearestDistanceUpdated = float.MaxValue;
        //private List<Pose3D> pointsObserved; // CURRENTLY DISABLED
        private Pose3D nearestPointRobotObserved = null;
        private float oldAccuracyDist = 0;
        private float oldAccuracyP = 0.1f;
        private float oldAccuracyUT = 0;
        private float newObservationAccuracyP = 0;
        private float maxDistanceToMatch = 0;
        private float getAccuracyDistance(float dist)
        {
            return -0.4f * (float)Math.Atan(0.2f * dist - 0.2f) + 0.6f;
        }

        public float AccuracyOfEdgeP
        {
            get { return oldAccuracyP; }
        }

        public float AccuracyOfNewObservation
        {
            get { return newObservationAccuracyP; }
        }

        public float NearestDistanceRobotObserved
        {
            get { return nearestDistanceUpdated; }
        }
        public void updateObservation(Pose3D robotPos, Pose3D newPosition, int timeCycle)
        {
            updatePosition(newPosition);
            updateTime++;
            float newDist = robotPos.getDistance2D(point);
            if (newDist < nearestDistanceUpdated)
            {
                nearestPointRobotObserved = robotPos;
                nearestDistanceUpdated = newDist;
                oldAccuracyDist = getAccuracyDistance(newDist);
            }

            oldAccuracyUT = getAccuracyUpdateTime(updateTime);
            oldAccuracyP = (float)Math.Sqrt(oldAccuracyP * oldAccuracyUT);
            //oldAccuracyP = oldAccuracyP * oldAccuracyUT;
            updateMaxDistanceToMatch();
            lastUpdatedTimeCycle = timeCycle;
        }

        private float getAccuracyUpdateTime(int updateTime)
        {
            return 1f - (float)Math.Pow(1.1, -(float)updateTime);
        }

        private void updatePosition(Pose3D newPos)
        {
            double x = (point.X * (float)updateTime + newPos.X) / (float)(updateTime + 1);
            double y = (point.Y * (float)updateTime + newPos.Y) / (float)(updateTime + 1);
            point.X = x;
            point.Y = y;
        }

        public float MaxDistanceToMatch
        {
            get { return maxDistanceToMatch; }
        }

        private const float ERR_TOLERANCE_MATCH = 0.1f;
        private void updateMaxDistanceToMatch() // 20 M Laser -> 20 CM Err, 2 ObS
        {
            maxDistanceToMatch = nearestDistanceUpdated / 50f + ERR_TOLERANCE_MATCH;
        }

        private const float ERR_TOLERANCE_POINT_EQUAL = 0.01f;
        public static Edge applyRotateShift_OLD(Edge edge, float theta, Pose3D rotateCenter, Pose3D shift)
        {
            Pose3D newP, newOtherP, otherP, nearP, newNearP;
            newP = Pose3D.applyRotateShift(edge.point, theta, rotateCenter, shift);
            Line newLine;
            List<Line> lines = null;
            bool pointIsHead;
            float distTail, distHead;
            if (edge.lines == null)
            {
                throw new Exception("WTF");
            }
            else
            {
                lines = new List<Line>();

                foreach (Line line in edge.lines)
                {
                    distHead = line.head.getDistance2D(edge.point);
                    distTail = line.tail.getDistance2D(edge.point);
                    //if (distHead < ERR_TOLERANCE_POINT_EQUAL) // line.head == edge.point
                    //{
                    //    otherP = line.tail;
                    //    pointIsHead = true;
                    //}
                    //else if (distTail < ERR_TOLERANCE_POINT_EQUAL)
                    //{
                    //    otherP = line.head;
                    //    pointIsHead = false;
                    //}
                    //else
                    //{
                    //    throw new Exception("WTF");
                    //}
                    if (distHead < distTail) // line.head ~ edge.point
                    {
                        otherP = line.tail;
                        nearP = line.head;
                        pointIsHead = true;
                    }
                    else
                    {
                        otherP = line.head;
                        nearP = line.tail;
                        pointIsHead = false;
                    }
                    newOtherP = Pose3D.applyRotateShift(otherP, theta, rotateCenter, shift);
                    newNearP = Pose3D.applyRotateShift(nearP, theta, rotateCenter, shift);
                    newLine = new Line();
                    if (pointIsHead)
                    {
                        //newLine.head = newP;
                        newLine.tail = newOtherP;
                        newLine.head = newNearP;
                    }
                    else
                    {
                        newLine.head = newOtherP;
                        //newLine.tail = newP;
                        newLine.tail = newNearP;
                    }
                    lines.Add(newLine);
                }
            }
            return new Edge(newP, edge.type, lines, edge.AccuracyOfNewObservation);
        }

        public static Edge applyRotateShift(Edge edge, float theta, Pose3D rotateCenter,Pose3D shift)
        {
            Pose3D newP, newT, newH;
            newP = Pose3D.applyRotateShift(edge.point, theta, rotateCenter, shift);
            Line newLine;
            List<Line> lines = null;
            bool pointIsHead;
            float distTail, distHead;
            if (edge.lines == null)
            {
                throw new Exception("WTF");
            }
            else
            {
                lines = new List<Line>();

                foreach (Line line in edge.lines)
                {
                    newH = Pose3D.applyRotateShift(line.head, theta, rotateCenter, shift);
                    newT = Pose3D.applyRotateShift(line.tail, theta, rotateCenter, shift);
                    newLine = new Line();
                    newLine.head = newH;
                    newLine.tail = newT;
                    lines.Add(newLine);
                }
            }
            return new Edge(newP, edge.type, lines, edge.AccuracyOfNewObservation);
        }

        private static float getMinAngleBetweenLines(Edge e1, Edge e2)
        {
            float a, best = float.MaxValue;
            foreach (Line l1 in e1.lines)
            {
                foreach (Line l2 in e2.lines)
                {
                    a = MathHelper.AngleBetweenLines(l1, l2);
                    if (a < best)
                        best = a;
                }
            }
            return best;
        }

        private static float getAvgAngleBetweenLines(Edge e1, Edge e2)
        {
            float a11, a12, a21, a22, r1, r2;
            a11 = MathHelper.AngleBetweenLines(e1.lines[0], e2.lines[0]);
            a12 = MathHelper.AngleBetweenLines(e1.lines[1], e2.lines[1]);

            a21 = MathHelper.AngleBetweenLines(e1.lines[0], e2.lines[1]);
            a22 = MathHelper.AngleBetweenLines(e1.lines[1], e2.lines[0]);

            r1 = (a11 + a12) / 2f;
            r2 = (a21 + a22) / 2f;

            return Math.Min(r1, r2);
        }

        public static float getAngleBetweenLines(Edge e1, Edge e2)
        {
            if (e1.lines.Count == 2 && e2.lines.Count == 2)
            {
                return getAvgAngleBetweenLines(e1, e2);
            }
            else
            {
                return getMinAngleBetweenLines(e1, e2);
            }
        }

        public string ToMatlabString(bool point, bool type, bool lines, bool additionalInfo)
        {
            string res = "";
            if (point)
            {
                res += this.point.ToMatlabString() + " ";
            }
            if (type)
            {
                if (this.type == EdgeType.Corner)
                    res += "1 ";
                else if (this.type == EdgeType.EndOfWall)
                    res += "2 ";
                else
                    res += "3 ";
                //res += this.type.ToString() + " ";
            }
            if (lines)
            {
                res += this.lines.Count.ToString() + " ";
                foreach (Line l in this.lines)
                {
                    res += l.ToMatlabString() + " ";
                }
            }

            if (additionalInfo)
            {
                res += updateTime.ToString() + " " +
                    NearestDistanceRobotObserved.ToString() + " " +
                    AccuracyOfEdgeP.ToString() + " ";
            }
            res.TrimEnd();
            return res;
        }

        public override string ToString()
        {
            return "Point : " + point.ToFormattedString() + " , Type : " + type.ToString() + " , Update Time : " + updateTime.ToString() +
                " , Accuracy " + AccuracyOfEdgeP.ToString() + " , Nearest Distance Observed : " + nearestDistanceUpdated.ToString() +
                " , Connected Lines : " + lines.Count.ToString();
        }

    }

}
