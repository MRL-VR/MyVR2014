using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.CustomMath;
using MRL.Utils;
using MRL.Commons;

namespace MRL.Controller
{
    public enum RotateMode //Set Robot Rotate mode 
    {
        ROTATE_LEFT, ROTATE_RIGHT, ROTATE_DIR
    }
    public enum ControlMode //Set Robot control mode (in ROT_F-> just go forward and in ROT_FB can go forward and back)
    {
        ROT_F, ROT_FB
    }

    public class WheeledControl
    {
        private const double maxvel = 7;
        private const double minvel = -7;
        private const double rotatvel = 0.4f;
        private const double stopPoint = 1;
        private double distanceinit;
        private Vector2 startP;
        private Vector2 endP;
        private double rotate;
        private ControlMode ctrlMode;
        private double maxvelocity;
        private double minvelocity;
        private bool disMode;
        private double rotateVelocity;
        private RotateMode rotMode;
        public WheeledControl(double maxV, double minV, double rotVelocity, ControlMode controlMode, bool distanceMode)
        {
            ctrlMode = controlMode;
            maxvelocity = maxV;
            minvelocity = minV;
            disMode = distanceMode;
            this.rotateVelocity = rotVelocity;
        }
        public Vector2 CheckSpeed(Vector2 startPoint, Vector2 endPoint, double rotate)
        {
            startP = startPoint;
            endP = endPoint;

            double maxV, minV, dist;
            maxV = maxvelocity;
            minV = minvelocity;
            Vector2 velocity = new Vector2();
            int side; //set side of goal relation with rotate of Robot
            int c_r_t;
            double det;
            double s_r; //set angle between two point
            Vector2 rot_p = new Vector2(); // Set Point for Robot Rotate 

            Vector2 newEndP = new Vector2(endP.X - startP.X, endP.Y - startP.Y);// transmit of end point by Start point
            Vector2 newStartP = new Vector2(startP.X - startP.X, startP.Y - startP.Y);// transmit of start point to (0,0)

            dist = checkDistance(newStartP, newEndP);

            if (disMode == true)
            {
                distanceinit = Math.Sqrt((Math.Pow(newEndP.X, 2) + Math.Pow(newEndP.Y, 2)));
                disMode = false;
            }
            rot_p = set_point(rotate); // get point in vector of rotate
            det = calculateleftturn(newStartP, newEndP, rot_p);// calculate determinan

            side = checkRotatePoint(det);// set side point relate with robot

            s_r = setRotate(rot_p, newEndP); //set value of rotate (set value of angle for rotate and stop) 
            c_r_t = checkRotateRobot(s_r); //is the value of rotate angle equal zero 
            velocity = rotation(side, c_r_t, rot_p, newEndP, dist, distanceinit, minV, maxV, rotateVelocity); //set side of rotate

            return velocity;
        }
        private Vector2 set_point(double p)// create unit vector 
        {
            Vector2 rotPoint = new Vector2();
            rotPoint.X = 1 * Math.Cos(p);
            rotPoint.Y = 1 * Math.Sin(p);
            return rotPoint;
        }
        private double calculateleftturn(Vector2 P_Start, Vector2 P_Goal, Vector2 P_Rotat)
        {
            return P_Start.X * (P_Goal.Y - P_Rotat.Y) - P_Start.Y * (P_Goal.X - P_Rotat.X) + P_Goal.X * P_Rotat.Y - P_Goal.Y * P_Rotat.X;
        }
        private int checkRotatePoint(double D)
        {
            int ans = 0;

            if (D >= 0.1)
            {
                rotMode = RotateMode.ROTATE_LEFT;
                ans = -1;

            }
            else if (D <= -0.1)
            {
                rotMode = RotateMode.ROTATE_RIGHT;
                ans = 1;
            }
            else if ((D > -0.1) && (D < 0.1))
            {
                rotMode = RotateMode.ROTATE_DIR;
                ans = 0;

            }
            return ans;
        }
        private double setRotate(Vector2 rotateP, Vector2 goalP)
        {
            double angle1, angle2;// angle1 zaviyeh goal and angle2 zaviyeh rotate point
            double delta, checkdel;

            angle1 = Math.Atan2(goalP.Y, goalP.X);
            angle2 = Math.Atan2(rotateP.Y, rotateP.X);

            delta = Math.Abs(angle1 - angle2);
            checkdel = Math.Abs(angle1 - angle2);
            if (checkdel > Math.PI)
            {
                delta = Math.Abs(angle1) + Math.Abs(angle2);
            }
            return delta;
        }
        private int checkRotateRobot(double s_r)
        {
            int rot;
            if (s_r <= 0.1)
            {
                rot = 1;
            }
            else
            {
                rot = -1;
            }
            return rot;
        }
        private Vector2 rotation(double side, double c_r_t, Vector2 Rot_p, Vector2 NewEndP, double T, double disinit, double minve, double maxve, double rotvel)
        {
            double angle1, angle2;// angle1 is goal angle  and angle2 is rotate point angle 

            Vector2 velo = new Vector2();
            if (c_r_t == 1)
            {
                velo = forward(maxve, minve, T, disinit);
            }
            else
                if (c_r_t == -1)
                {
                    if (side == 1)
                    {
                        velo = rightSide(rotvel);
                    }
                    else
                        if (side == -1)
                        {
                            velo = leftSide(rotvel);
                        }
                        else
                            if (side == 0)
                            {
                                angle1 = Math.Atan2(NewEndP.Y, NewEndP.X);
                                angle2 = Math.Atan2(Rot_p.Y, Rot_p.X);

                                if ((Math.Abs((angle1 - angle2)) < 0.8))
                                    velo = forward(maxve, minve, T, disinit);
                                else if (ctrlMode == ControlMode.ROT_FB)
                                    velo = backwardST(maxve, minve, T, disinit);
                                else
                                    velo = backwardNST(rotvel);
                            }
                }

            return velo;

        }
        private double checkDistance(Vector2 startPos, Vector2 endPos)
        {

            double dx, dy, dis;
            dx = Math.Pow((endPos.X - startPos.X), 2);
            dy = Math.Pow((endPos.Y - startPos.Y), 2);
            dis = Math.Sqrt(dy + dx);

            return dis;
        }
        private Vector2 backwardST(double maxV, double minV, double dis, double Tdis)
        {
            double t;
            double Xm, Ym;
            if (dis > stopPoint)
            {
                t = 1 / (dis / maxV);
                Xm = -(t + 0.5);
                Ym = -(t + 0.5);
            }
            else
            {
                t = (dis / maxV);
                Xm = -(t + 0.1);
                Ym = -(t + 0.1);
            }
            Vector2 result = new Vector2(Xm, Ym);
            return result;
        }
        private Vector2 forward(double maxV, double minV, double dis, double tDis)
        {
            double t;
            double Xm, Ym;
            if (dis > stopPoint)
            {
                t = 1 / (dis / maxV);
                Xm = t + 0.5;
                Ym = t + 0.5;
            }
            else
            {
                t = (dis / maxV);
                Xm = t + 0.1;
                Ym = t + 0.1;
            }
            ProjectCommons.writeConsoleMessage("DIS : " + dis.ToString() + "Tdis : " + tDis.ToString() + "t : " + t.ToString(), Utils.ConsoleMessageType.Exclamation);
            ProjectCommons.writeConsoleMessage("X : " + Xm.ToString() + "Y : " + Ym.ToString(), Utils.ConsoleMessageType.Exclamation);
            Vector2 result = new Vector2(Xm, Ym);
            return result;
        }
        private Vector2 leftSide(double rotvel)
        {
            double Xm, Ym;

            Xm = -1 * rotvel;
            Ym = -Xm;

            Vector2 result = new Vector2(Xm, Ym);
            return result;
        }
        private Vector2 rightSide(double rotvel)
        {
            double Xm, Ym;

            Xm = rotvel;
            Ym = -Xm;
            Vector2 result = new Vector2(Xm, Ym);

            return result;
        }
        private Vector2 backwardNST(double rotvel)
        {
            double Xm, Ym;

            Xm = rotvel;
            Ym = -Xm;
            Vector2 result = new Vector2(Xm, Ym);
            return result;
        }
    }
}
