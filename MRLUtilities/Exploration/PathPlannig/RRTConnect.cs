using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using MRL.Commons;
using MRL.Commons.Tree;
using MRL.CustomMath;
using MRL.Mapping;
using MRL.Communication.External_Commands;
using System.Linq;
using ClipperLib;

namespace MRL.Exploration.PathPlannig
{
    public class GraphLine
    {
        public Pose2D Head { set; get; }
        public Pose2D Tail { set; get; }
    }

    public class RRTConnect
    {

        private Pose2D _MapMinPos;
        private Pose2D _MapMaxPos;
        private float _Epsilon = 0.1f;
        private double _threshold;

        private KdTree<Vector<double>, double> startTree;
        private KdTree<Vector<double>, double> goalTree;
        private GrowState rrtTreeState = GrowState.ADVANCED;
        private Vector<double> srcPoint;
        private Vector<double> goalPoint;
        private bool swap = false;
        private KdTreeNode<Vector<double>> parent;
        private NormalDistribution normalRandom = new NormalDistribution();
        public List<GraphLine> myMap;
        public List<GraphLine> myGraph;
        public List<Pose2D> OriginalMapPolygon;
        public List<List<Pose2D>> BorderedMapPolygon;

        public Pose2D MapMinPos
        {
            get { return _MapMinPos; }
            set { _MapMinPos = value; }
        }

        public Pose2D MapMaxPos
        {
            get { return _MapMaxPos; }
            set { _MapMaxPos = value; }
        }

        public double Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }
        private enum GrowState
        {
            // no progress has been made
            TRAPPED,
            // progress has been made towards the randomly sampled state
            ADVANCED,
            // the randomly sampled state was reached
            REACHED
        };

        public RRTConnect()
        {
            myMap = new List<GraphLine>();
        }

        public List<Pose2D> FindPathRRTConnect(Pose2D src, Pose2D goal, Laser laser, int cycle)
        {
            Pose2D tmp = Vector2.FromAngleSize(src.Rotation, 0.5d) + src;
            srcPoint = Pose2DToVector(tmp);
            goalPoint = Pose2DToVector(goal);


            Vector<double> lastStartTreePoint = srcPoint;
            Vector<double> lastGoalTreePoint = goalPoint;
            Vector<double> randPoint = null;

            if (laser == null)
                return null;
            OriginalMapPolygon = new List<Pose2D>();
            for (int i = 0; i < laser.fRanges.Length; i++)
            {
                Pose2D newPos = laser.GetBeamPos(src, i);
                OriginalMapPolygon.Add(newPos);
            }

            BorderedMapPolygon = ScalePolygon(OriginalMapPolygon.ToArray(), 0.25);

            bool state = false;
            for (int i = 0; i < BorderedMapPolygon.Count; i++)
                state |= isPointInPolygon(goal, BorderedMapPolygon[i].ToArray());

            if (!state)
            {
                double minDis = double.MaxValue;
                Pose2D nPos = null;
                for (int i = 0; i < BorderedMapPolygon.Count; i++)
                {
                    for (int j = 0; j < BorderedMapPolygon[i].Count; j++)
                    {
                        double d = goal.DistanceFrom(BorderedMapPolygon[i][j]);

                        if (d < minDis)
                        {
                            minDis = d;
                            nPos = BorderedMapPolygon[i][j];
                        }
                    }
                }

                Utils.GMath.Position2D g = new Utils.GMath.Position2D(goal);
                Utils.GMath.Position2D n = new Utils.GMath.Position2D(nPos);
                Utils.GMath.Line l = new Utils.GMath.Line(g, n);
                Pose2D newPos = n + Utils.GMath.Vector2D.FromAngleSize(l.Angle, 0.01);

                goalPoint = Pose2DToVector(newPos);
            }

            startTree = KdTree<Vector<double>, double>.Construct(2, new[] { srcPoint });
            goalTree = KdTree<Vector<double>, double>.Construct(2, new[] { goalPoint });

            myMap = createMyMap(BorderedMapPolygon);

            myGraph = new List<GraphLine>();

            swap = false;

            for (int i = 0; i < cycle; i++)
            {
                randPoint = rand(goal);

                if (!swap)
                {
                    Vector<double> newPoint = Extend(startTree, randPoint, lastGoalTreePoint, swap);

                    if (newPoint != null)
                        lastStartTreePoint = newPoint;
                }
                else
                {
                    Vector<double> newPoint = Extend(goalTree, randPoint, lastStartTreePoint, swap);

                    if (newPoint != null)
                        lastGoalTreePoint = newPoint;
                }

                if (rrtTreeState == GrowState.REACHED)
                {

                    return smoothPath(getPath(lastStartTreePoint, lastGoalTreePoint));
                }
                swap = !swap;
            }

            return null;
        }

        private List<GraphLine> createMyMap(List<List<Pose2D>> borderedMap)
        {
            List<GraphLine> map = new List<GraphLine>();
            List<Pose2D> mapPolygon = new List<Pose2D>();


            for (int i = 0; i < borderedMap.Count; i++)
            {
                for (int j = 0; j < borderedMap[i].Count - 1; j++)
                {
                    GraphLine newGL = new GraphLine();
                    newGL.Head = borderedMap[i][j];
                    newGL.Tail = borderedMap[i][j + 1];
                    map.Add(newGL);
                }
                map.Add(new GraphLine() { Head = borderedMap[i][borderedMap[i].Count - 1], Tail = borderedMap[i][0] });
            }


            double Min_X = map.Min(x => x.Head.X);
            double Min_Y = map.Min(x => x.Head.Y);

            double Max_X = map.Max(x => x.Head.X);
            double Max_Y = map.Max(x => x.Head.Y);
            MapMinPos = new Pose2D(Min_X, Min_Y, 0.0d);
            MapMaxPos = new Pose2D(Max_X, Max_Y, 0.0d);

            return map;
        }

        private Vector<double> Extend(KdTree<Vector<double>, double> searchTree, Vector<double> randPoint, Vector<double> goalPoint, bool tState)
        {
            Vector<double> nearPoint = searchTree.FindNearestNeighbor(randPoint);
            parent = searchTree.Find(nearPoint);
            Vector<double> newPoint = newConfig(nearPoint, randPoint);

            Pose2D nearPointCarmen = VectorToPose2D(nearPoint);
            Pose2D newPointCarmen = VectorToPose2D(newPoint);
            Pose2D goalPointCarmen = VectorToPose2D(goalPoint);

            if (isClear(nearPointCarmen, newPointCarmen))
            {
                if (!isClear(newPointCarmen, goalPointCarmen))
                {
                    searchTree.Add(newPoint.Clone(), parent);
                    rrtTreeState = GrowState.ADVANCED;
                    myGraph.Add(new GraphLine() { Head = VecotrToPose2D(newPoint), Tail = KdTreeNodeToPose2D(parent) });
                    return newPoint;
                }
                else
                {
                    searchTree.Add(newPoint.Clone(), parent);
                    rrtTreeState = GrowState.REACHED;
                    myGraph.Add(new GraphLine() { Head = VecotrToPose2D(newPoint), Tail = KdTreeNodeToPose2D(parent) });
                    myGraph.Add(new GraphLine() { Head = VecotrToPose2D(newPoint), Tail = VecotrToPose2D(goalPoint) });
                    return newPoint;
                }
            }

            return null;
        }
        private List<Pose2D> getPath(Vector<double> lastStartPoint, Vector<double> lastGoalPoint)
        {
            List<Pose2D> path = new List<Pose2D>();

            KdTreeNode<Vector<double>> node = startTree.Find(lastStartPoint);

            while (node != null)
            {
                path.Add(KdTreeNodeToPose2D(node));

                node = node.Parent;
            }

            path.Reverse();


            node = goalTree.Find(lastGoalPoint);

            while (node != null)
            {
                path.Add(KdTreeNodeToPose2D(node));

                node = node.Parent;
            }

            //return smoothPath(path);
            return path;
        }

        public bool isPointInPolygon(Pose2D p, Pose2D[] polygon)
        {

            int k, j = polygon.Length - 1;
            bool oddNodes = false;
            for (k = 0; k < polygon.Length; k++)
            {
                Pose2D polyK = polygon[k];
                Pose2D polyJ = polygon[j];

                if (((polyK.Y > p.Y) != (polyJ.Y > p.Y)) &&
                 (p.X < (polyJ.X - polyK.X) * (p.Y - polyK.Y) / (polyJ.Y - polyK.Y) + polyK.X))
                    oddNodes = !oddNodes;
                j = k;
            }

            if (oddNodes)
                return true;
            return false;
        }

        private List<Pose2D> smoothPath(List<Pose2D> path)
        {
            List<Pose2D> SmoothPath = new List<Pose2D>();

            int firstNode = 0;
            SmoothPath.Add(path[firstNode]);

            int count = 1, i;
            int pathLenght = path.Count;

            for (i = count; i < pathLenght; i++)
            {
                if (isClear(path[firstNode], path[i]))
                    continue;
                else
                {
                    SmoothPath.Add(path[i - 1]);
                    firstNode = i - 1;
                }

            }

            SmoothPath.Add(path[i - 1]);

            if (SmoothPath.Count > 1)
            {
                List<Pose2D> segmentedPath = new List<Pose2D>();
                for (int j = 0; j < SmoothPath.Count - 1; j++)
                {
                    MRL.Utils.GMath.Position2D p1 = new Utils.GMath.Position2D(SmoothPath[j]);
                    MRL.Utils.GMath.Position2D p2 = new Utils.GMath.Position2D(SmoothPath[j + 1]);
                    MRL.Utils.GMath.Line line = new Utils.GMath.Line(p1, p2);

                    double d = p2.DistanceFrom(p1);
                    int n = (int)Math.Floor(d / 0.5d);

                    float ep = 0.0f;
                    double diff = 90.0d * Math.PI / 180; ;
                    for (int k = 0; k <= n; k++)
                    {
                        MRL.Utils.GMath.Position2D nPos = p1 + MRL.Utils.GMath.Vector2D.FromAngleSize(line.Angle + diff, ep);
                        segmentedPath.Add(new Pose2D(nPos));
                        ep += 0.5f;
                    }

                    Pose2D lPos = segmentedPath.Last();
                    if (lPos.DistanceFrom(p2) > 0.1f)
                        segmentedPath.Add(p2);
                }
                return segmentedPath;
            }
            else
                return SmoothPath;
        }

        private bool isClear(Pose2D src, Pose2D dst)
        {
            for (int i = 0; i < BorderedMapPolygon.Count; i++)
            {
                bool srcState = isPointInPolygon(src, BorderedMapPolygon[i].ToArray());
                bool dstState = isPointInPolygon(dst, BorderedMapPolygon[i].ToArray());

                if ((srcState && !dstState) || (!srcState && dstState))
                    return false;

                if (srcState && dstState)
                {
                    Utils.GMath.Position2D h = new Utils.GMath.Position2D(src.X, src.Y);
                    Utils.GMath.Position2D t = new Utils.GMath.Position2D(dst.X, dst.Y);
                    Utils.GMath.PartLine l1 = new Utils.GMath.PartLine(h, t);
                    for (int j = 0; j < myMap.Count; j++)
                    {
                        Utils.GMath.Position2D hm = new Utils.GMath.Position2D(myMap[j].Head.X, myMap[j].Head.Y);
                        Utils.GMath.Position2D tm = new Utils.GMath.Position2D(myMap[j].Tail.X, myMap[j].Tail.Y);
                        Utils.GMath.PartLine l2 = new Utils.GMath.PartLine(hm, tm);
                        if (l1.IntersectWithPartLine(l2) != null)
                            return false;
                    }
                }
            }



            return true;
        }

        public List<List<Pose2D>> ScalePolygon(Pose2D[] points, double scale)
        {
            return ExecuteOffset(-scale * 10000f, points.ToList());
        }

        private List<List<Pose2D>> ExecuteOffset(double Offset, List<Pose2D> points)
        {
            List<IntPoint> mPolygon = new List<IntPoint>();
            foreach (var item in points)
                mPolygon.Add(new IntPoint(item.X * 10000f, item.Y * 10000f));


            ClipperOffset co = new ClipperOffset();
            co.AddPath(mPolygon, JoinType.jtRound, EndType.etClosedPolygon);
            List<List<IntPoint>> ans = new List<List<IntPoint>>();
            co.Execute(ref ans, Offset);


            return ans.Select(w => w.Select(Q => new Pose2D((double)Q.X / 10000f, (double)Q.Y / 10000f, 0)).ToList()).ToList();
        }
        public static Pose2D Compute2DPolygonCentroid(Pose2D[] vertices)
        {
            Pose2D centroid = new Pose2D() { X = 0.0, Y = 0.0 };
            double signedArea = 0.0;
            double x0 = 0.0; // Current vertex X
            double y0 = 0.0; // Current vertex Y
            double x1 = 0.0; // Next vertex X
            double y1 = 0.0; // Next vertex Y
            double a = 0.0;  // Partial signed area

            // For all vertices except last
            int i = 0;
            for (i = 0; i < vertices.Length - 1; ++i)
            {
                x0 = vertices[i].X;
                y0 = vertices[i].Y;
                x1 = vertices[i + 1].X;
                y1 = vertices[i + 1].Y;
                a = x0 * y1 - x1 * y0;
                signedArea += a;
                centroid.X += (x0 + x1) * a;
                centroid.Y += (y0 + y1) * a;
            }

            // Do last vertex
            x0 = vertices[i].X;
            y0 = vertices[i].Y;
            x1 = vertices[0].X;
            y1 = vertices[0].Y;
            a = x0 * y1 - x1 * y0;
            signedArea += a;
            centroid.X += (x0 + x1) * a;
            centroid.Y += (y0 + y1) * a;

            signedArea *= 0.5;
            centroid.X /= (6 * signedArea);
            centroid.Y /= (6 * signedArea);

            return centroid;
        }

        Random r = new Random(DateTime.Now.Millisecond);

        private Vector<double> rand(Pose2D dst)
        {
            //normalRandom.Mu = dst.X;
            //normalRandom.Sigma = (dst.X + (MapMaxPos.X - MapMinPos.X)) / 2.0f;
            //double w = normalRandom.NextDouble();

            //normalRandom.Mu = dst.Y;
            //normalRandom.Sigma = (dst.Y + (MapMaxPos.Y - MapMinPos.Y))/ 2.0f;
            //double h = normalRandom.NextDouble();

            double w = 0, h = 0;
            w = r.Next((int)(MapMinPos.X * 100), (int)(MapMaxPos.X * 100));
            h = r.Next((int)(MapMinPos.Y * 100), (int)(MapMaxPos.Y * 100));

            return new DenseVector(new[] { Math.Floor(w / 100.0d), Math.Floor(h / 100.0d) });
        }

        private Vector<double> newConfig(Vector<double> nPoint, Vector<double> rPoint)
        {
            double a = rPoint[0] - nPoint[0];
            double b = rPoint[1] - nPoint[1];

            double l = Math.Sqrt(a * a + b * b);

            a = a / l;
            b = b / l;

            double x = nPoint[0] + a * _Epsilon;
            double y = nPoint[1] + b * _Epsilon;

            return new DenseVector(new[] { x, y });
        }

        /* private Vector<double> newConfig(Vector<double> nPoint, Vector<double> rPoint)
         {

             double a = rPoint[0] - nPoint[0];
             double b = rPoint[1] - nPoint[1];

             float d = (float)(b / a);

             double x = nPoint[0] + d * _Epsilon;
             double y = nPoint[1] + d * (x - nPoint[0]);
           

             return new DenseVector(new[] { x, y });
         }*/

        private Pose2D KdTreeNodeToPose2D(KdTreeNode<Vector<double>> v)
        {
            return new Pose2D(v.Value[0], v.Value[1], 0);
        }
        private Pose2D VecotrToPose2D(Vector<double> v)
        {
            return new Pose2D(v[0], v[1], 0);
        }
        private Pose2D VectorToPose2D(Vector<double> v)
        {
            return new Pose2D(v[0], v[1], 0);
        }
        private Vector<double> Pose2DToVector(Pose2D p)
        {
            return new DenseVector(new[] { (double)p.X, (double)p.Y });
        }
    }
}
