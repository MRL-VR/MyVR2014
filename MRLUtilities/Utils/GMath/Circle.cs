using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MRL.Utils.GMath
{
    /// <summary>
    /// Representing a circle with a Position2D Center and a Radiouse
    /// </summary>
    public class Circle
    {
        public bool PenIsChanged { get; set; }
        public bool IsFill { get; set; }
        public float Opacity { get; set; }
        private Position2D _center;
        /// <summary>
        /// Center of Circle
        /// </summary>
        public Position2D Center
        {
            get { return _center; }
            set { _center = value; }
        }
        private double _radious;
        /// <summary>
        /// Radiouse of Circle
        /// </summary>
        public double Radious
        {
            get { return _radious; }
            set { _radious = value; }
        }
        /// <summary>
        /// Representing a circle with a Position2D Center and a Radiouse
        /// </summary>
        /// <param name="center"> Center </param>
        /// <param name="radious"> Radiouse</param>
        public Circle(Position2D center, double radious)
        {
            _center = center;
            _radious = radious;
            _drawPen = new Pen(Color.Black, 0.01f);
            PenIsChanged = true;
        }
        /// <summary>
        /// Construct withot any input
        /// </summary>
        public Circle()
        {
        }
        /// <summary>
        /// Representing a circle with a Position2D Center, a Radiouse and Draw pen
        /// </summary>
        /// <param name="center">Center</param>
        /// <param name="radious">Radiuse</param>
        /// <param name="pen">Pen</param>
        public Circle(Position2D center, double radious, Pen pen, bool isShown)
        {
            _isShown = isShown;
            _center = center;
            _radious = radious;
            _drawPen = pen;
            IsFill = false;
            PenIsChanged = true;
        }
        public Circle(Position2D center, double radious, Pen pen)
        {

            _center = center;
            _radious = radious;
            _drawPen = pen;
            IsFill = false;
            PenIsChanged = true;
        }
        public Circle(Position2D center, double radious, Pen pen, bool isFill, float opacity, bool isShown)
        {
            _isShown = isShown;
            _center = center;
            _radious = radious;
            _drawPen = pen;
            IsFill = isFill;
            Opacity = opacity;
            PenIsChanged = true;
        }
        private Pen _drawPen;
        /// <summary>
        /// Pen that using for drawing the circle
        /// </summary>
        public Pen DrawPen
        {
            get { return _drawPen; }
            set { _drawPen = value; }
        }
        private bool _isShown = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsShown
        {
            get { return _isShown; }
            set { _isShown = value; }
        }
        /// <summary>
        /// String of Circle
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("C={0},R={1}", _center, _radious);
        }
        /// <summary>
        /// Calculate intersections between a line and a circle
        /// </summary>
        /// <param name="l">Target Line</param>
        /// <returns>if there is any intersection,List of Position2D </returns>
        public List<Position2D> Intersect(Line l)
        {
            Line perp = l.PerpenducilarLineToPoint(_center);
            Position2D perpfoot = l.IntersectWithLine(perp).Value;
            double dist = (perpfoot - _center).Size;
            List<Position2D> intersections = new List<Position2D>();

            if (dist < 0.001)
            {
                Vector2D r = l.Tail - l.Head;
                intersections.Add(_center + r.GetNormalizeToCopy(_radious));
                //intersections.Add(_center - r.GetNormalizeToCopy(_radious));
            }

            else if (dist < _radious)
            {
                double perpAngle = (perpfoot - _center).AngleInRadians;
                double openingAngle = Math.Acos(dist / _radious);
                intersections.Add(_center + Vector2D.FromAngleSize(perpAngle + openingAngle, _radious));
                intersections.Add(_center + Vector2D.FromAngleSize(perpAngle - openingAngle, _radious));
            }
            else if (dist == _radious)
                intersections.Add(perpfoot);
            return intersections;
        }
        /// <summary>
        /// Tangant ?
        /// </summary>
        /// <param name="P"></param>
        /// <param name="TangentLines"></param>
        /// <param name="TangentPoints"></param>
        /// <returns></returns>
        public int GetTangent(Position2D P, out List<Line> TangentLines, out List<Position2D> TangentPoints)
        {
            Vector2D vect = _center - P;
            double dist = vect.Size;
            TangentLines = new List<Line>();
            TangentPoints = new List<Position2D>();
            if (dist >= _radious)
            {
                Line l = new Line(P, _center);
                if (dist == _radious)
                {
                    TangentPoints.Add(P);
                    TangentLines.Add(l.PerpenducilarLineToPoint(_center));
                    return 1;
                }
                else
                {
                    double lineAngle = vect.AngleInRadians;
                    double openingAngle = Math.Asin(_radious / dist);
                    double tangentDist = Math.Sqrt(dist * dist - _radious * _radious);
                    Vector2D v1 = Vector2D.FromAngleSize(lineAngle + openingAngle, tangentDist);
                    TangentLines.Add(new Line(P, P + v1));
                    TangentPoints.Add(P + v1);

                    v1 = Vector2D.FromAngleSize(lineAngle - openingAngle, tangentDist);
                    TangentLines.Add(new Line(P, P + v1));
                    TangentPoints.Add(P + v1);
                    return 2;
                }
            }
            else
                return 0;
        }

        public List<Position2D> Intersect(Circle circle)
        {
            List<Position2D> ret = new List<Position2D>();
            double d = this.Center.DistanceFrom(circle.Center);
            if ((d > this.Radious + circle.Radious) || (d < Math.Abs(this.Radious - circle.Radious)))
            {
                return new List<Position2D>();
            }
            else if (d == 0 && this.Radious == circle.Radious)
            {
                return new List<Position2D>();
            }
            else
            {
                double rA = this.Radious;
                double rB = circle.Radious;
                double xA = this.Center.X;
                double yA = this.Center.Y;
                double xB = circle.Center.X;
                double yB = circle.Center.Y;


                double K = (1 / 4.0) * Math.Sqrt((Math.Pow((rA + rB), 2) - d * d) * (d * d - Math.Pow((rA - rB), 2)));
                double X1 = (1 / 2.0) * (xB + xA) + (1 / 2.0) * (xB - xA) * (rA * rA - rB * rB) / (d * d) + 2 * (yB - yA) * K / (d * d);
                double Y1 = (1 / 2.0) * (yB + yA) + (1 / 2.0) * (yB - yA) * (rA * rA - rB * rB) / (d * d) - 2 * (xB - xA) * K / (d * d);

                double X2 = (1 / 2.0) * (xB + xA) + (1 / 2.0) * (xB - xA) * (rA * rA - rB * rB) / (d * d) - 2 * (yB - yA) * K / (d * d);
                double Y2 = (1 / 2.0) * (yB + yA) + (1 / 2.0) * (yB - yA) * (rA * rA - rB * rB) / (d * d) + 2 * (xB - xA) * K / (d * d);

                ret.Add(new Position2D(X1, Y1));
                ret.Add(new Position2D(X2, Y2));
            }
            return ret;
        }

    }
}
