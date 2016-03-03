using System;

using MRL.CustomMath;

namespace MRL.Controller
{
    public class Control
    {
        public Vector3 CheckSpeed(Vector2 PoseStart, Vector2 PoseEnd, double rotation)
        {
            //double rote = rotation - Math.PI / 2.0;
            double rote = -rotation;
            Vector2 vLocal = new Vector2(PoseEnd.X - PoseStart.X, PoseEnd.Y - PoseStart.Y);


            Vector2 localPos = new Vector2();
            localPos.X = vLocal.X * Math.Cos(rote) - vLocal.Y * Math.Sin(rote);
            localPos.Y = vLocal.X * Math.Sin(rote) + vLocal.Y * Math.Cos(rote);

            PoseStart = new Vector2(0, 0);
            PoseEnd = localPos;


            if (PoseEnd.Y > 0)
            {

                Vector3 finalResultPlus = new Vector3();

                double distTzz = Math.Sqrt(Math.Pow((PoseEnd.X - PoseStart.X), 2) + Math.Pow((PoseEnd.Y - PoseStart.Y), 2));
                //double DegB0 = Math.Asin(PoseEnd.Y / distTzz); // zaviye EndPoint ba khate y=0 ya speed(3,3)
                double DegB0 = Math.Abs(Math.Atan2(PoseEnd.Y, PoseEnd.X)); //Math.Asin(Math.Abs(PoseEnd.Y) / distTzz2); // zaviye EndPoint ba khate y=0 ya speed(3,3)


                if ((0 < DegB0) && (DegB0 < 0.225))
                    finalResultPlus = threeOne(PoseStart, PoseEnd);

                else if ((0.225 < DegB0) && (DegB0 < 0.325))
                    finalResultPlus = threeZero(PoseStart, PoseEnd);
                else
                {
                    double tS2E = PoseEnd.X / 6;
                    finalResultPlus = new Vector3(3, -3, tS2E);//Modified by Chakhanechi
                    //finalResultPlus = new Vector3(6, -6, tS2E);

                }

                finalResultPlus.Z = Math.Abs(finalResultPlus.Z);

                return finalResultPlus;
            }

            else if (PoseEnd.Y == 0)
            {
                Vector3 finalResultZero = null;

                if (PoseEnd.X > 0)
                {
                    double tS2E = Math.Abs(PoseEnd.X / 6);
                    finalResultZero = new Vector3(6, 6, tS2E);
                }
                else
                {
                    double tS2E = Math.Abs(PoseEnd.X / 6);
                    finalResultZero = new Vector3(3, -3, tS2E); //Modified by Chakhanechi
                    //finalResultZero = new Vector3(6, -6, tS2E);

                }

                return finalResultZero;
            }
            else
            {
                Vector3 finalResultMinus = new Vector3();

                double distTzz2 = Math.Sqrt(Math.Pow((PoseEnd.X - PoseStart.X), 2) + Math.Pow((PoseEnd.Y - PoseStart.Y), 2));
                double DegB0 = Math.Abs(Math.Atan2(PoseEnd.Y, PoseEnd.X)); //Math.Asin(Math.Abs(PoseEnd.Y) / distTzz2); // zaviye EndPoint ba khate y=0 ya speed(3,3)

                //if (writeDebugMessage)
                //{
                //    ProjectCommons.writeConsoleMessage("DegB0 : " + DegB0, Utils.ConsoleMessageType.Information);
                //}

                
                if ((0 < DegB0) && (DegB0 < 0.2614))
                    finalResultMinus = oneThree(PoseStart, PoseEnd);

                else if ((DegB0 < 0.2614) && ((DegB0 < 0.4614))) //1.096468)))
                    finalResultMinus = zeroThree(PoseStart, PoseEnd);

                //else if ((DegB0 < 1.096468) && ((DegB0 < 1.1552)))
                //    finalResultMinus = zeroThree(PoseStart, PoseEnd);

                //else if ((DegB0 < 1.1552) && ((DegB0 < 1.2597)))
                //    finalResultMinus = MoneThree(PoseStart, PoseEnd);
                else
                {

                    double tS2E = PoseEnd.X / 6;
                    finalResultMinus = new Vector3(-3, 3, tS2E); //Modified by Chakhanechi
                    //finalResultMinus = new Vector3(-6, 6, tS2E);
                }

                //if (writeDebugMessage)
                //{
                //ProjectCommons.writeConsoleMessage("Speed : " + finalResultMinus.ToString(), Utils.ConsoleMessageType.Exclamation);
                //    writeDebugMessage = false;
                //}

                finalResultMinus.Z = Math.Abs(finalResultMinus.Z);

                return finalResultMinus;

            }
        }

        private Vector3 threeTwo(Vector2 PoseStart, Vector2 PoseEnd)
        {
            double delta = 0.1936 - 2.356 * (0.012 + (PoseEnd.X + PoseEnd.Y));

            double x1 = (-0.44 + (delta)) / 1.178;
            double x2 = (-0.44 - (delta)) / 1.178;

            double y1 = 0.589 * Math.Pow(x1, 2) - 0.56 * x1 + 0.012;
            double y2 = 0.589 * Math.Pow(x2, 2) - 0.56 * x2 + 0.012;

            double distT1 = Math.Sqrt(Math.Pow((x1 - PoseEnd.X), 2) + Math.Pow((y1 - PoseEnd.Y), 2));
            double distT2 = Math.Sqrt(Math.Pow((x2 - PoseEnd.X), 2) + Math.Pow((y2 - PoseEnd.Y), 2));

            Vector2 Par = (distT1 < distT2) ? new Vector2(x1, y1) : new Vector2(x2, y2);

            double d1 = Math.Sqrt(Math.Pow((PoseEnd.X - Par.X), 2) + Math.Pow((PoseEnd.Y - Par.Y), 2)); ;// fasele end ta mnhani
            double d2 = PoseEnd.Y;// fasele end ta ZZ
            double t;


            if (d1 < d2)
                t = d1 / (d1 + d2); // d1 bayasti
            else
                t = d2 / (d1 + d2);

            double Xm = (1 - t) * (3) + t * 3;
            double Ym = (1 - t) * (3) + t * 2;

            double FT = Math.Sqrt(Math.Pow((PoseEnd.X - PoseStart.X), 2) + Math.Pow((PoseEnd.Y - PoseStart.Y), 2));
            double tS2E = FT / Ym;

            Vector3 result = new Vector3(Xm, Ym, tS2E);

            return result;
        }

        private Vector3 threeOne(Vector2 PoseStart, Vector2 PoseEnd)
        {

            double delta = 0.5776 - 4.8 * (0.09 + (PoseEnd.X + PoseEnd.Y));

            double x1 = (-0.76 + (delta)) / 2.4;
            double x2 = (-0.76 - (delta)) / 2.4;

            double y1 = 1.2 * Math.Pow(x1, 2) - 0.24 * x1 + 0.09;
            double y2 = 1.2 * Math.Pow(x2, 2) - 0.24 * x2 + 0.09;

            double distT1 = Math.Sqrt(Math.Pow((x1 - PoseEnd.X), 2) + Math.Pow((y1 - PoseEnd.Y), 2));
            double distT2 = Math.Sqrt(Math.Pow((x2 - PoseEnd.X), 2) + Math.Pow((y2 - PoseEnd.Y), 2));

            Vector2 Par = (distT1 < distT2) ? new Vector2(x1, y1) : new Vector2(x2, y2);

            double d1 = Math.Sqrt(Math.Pow((PoseEnd.X - Par.X), 2) + Math.Pow((PoseEnd.Y - Par.Y), 2)); ;// fasele end ta monhani
            double d2 = PoseEnd.Y;// fasele end ta ZZ
            double t;

            if (d1 < d2)
                t = d1 / (d1 + d2); // d1 bayasti
            else
                t = d2 / (d1 + d2);

            //Changed By Sanaz

            double Xm = (1 - t) * (4) + t * 4;
            double Ym = (1 - t) * (4) + t * 2;

            //double Xm = (1 - t) * (2) + t * 2;
            //double Ym = (1 - t) * (2) + t * 1;

            double FT = Math.Sqrt(Math.Pow((PoseEnd.X - PoseStart.X), 2) + Math.Pow((PoseEnd.Y - PoseStart.Y), 2));
            double tS2E = FT / Ym;

            Vector3 result = new Vector3(Xm, Ym, tS2E);

            return result;
        }

        private Vector3 threeZero(Vector2 PoseStart, Vector2 PoseEnd)
        {

            double delta = 0.5625 - 6 * (0.13 + (PoseEnd.X + PoseEnd.Y));

            double x1 = (-0.75 + (delta)) / 3;
            double x2 = (-0.75 - (delta)) / 3;

            double y1 = 1.5 * Math.Pow(x1, 2) - 0.25 * x1 + 0.13;
            double y2 = 1.5 * Math.Pow(x2, 2) - 0.25 * x2 + 0.13;

            double distT1 = Math.Sqrt(Math.Pow((x1 - PoseEnd.X), 2) + Math.Pow((y1 - PoseEnd.Y), 2));
            double distT2 = Math.Sqrt(Math.Pow((x2 - PoseEnd.X), 2) + Math.Pow((y2 - PoseEnd.Y), 2));

            Vector2 Par = (distT1 < distT2) ? new Vector2(x1, y1) : new Vector2(x2, y2);

            double d1 = Math.Sqrt(Math.Pow((PoseEnd.X - Par.X), 2) + Math.Pow((PoseEnd.Y - Par.Y), 2)); ;// fasele end ta monhani
            double d2 = PoseEnd.Y;// fasele end ta ZZ
            double t;

            if (d1 < d2)
                t = d1 / (d1 + d2); // d1 bayasti
            else
                t = d2 / (d1 + d2);

            //Changed By Sanaz

            double Xm = (1 - t) * (4) + t * 4;
            double Ym = (1 - t) * (2) + t * (0);

            //double Xm = (1 - t) * (2) + t * 2;
            //double Ym = (1 - t) * (1) + t * (0);

            double FT = Math.Sqrt(Math.Pow((PoseEnd.X - PoseStart.X), 2) + Math.Pow((PoseEnd.Y - PoseStart.Y), 2));
            double tS2E = FT / Ym;

            Vector3 result = new Vector3(Xm, Ym, tS2E);

            return result;
        }

        private Vector3 threeMOne(Vector2 PoseStart, Vector2 PoseEnd)
        {

            double delta = 0.5476 - 2.84 * (0.17 + (PoseEnd.X + PoseEnd.Y));

            double x1 = (-0.74 + (delta)) / 3.42;
            double x2 = (-0.74 - (delta)) / 3.42;

            double y1 = 1.71 * Math.Pow(x1, 2) - 0.26 * x1 + 0.17;
            double y2 = 1.71 * Math.Pow(x2, 2) - 0.26 * x2 + 0.17;

            double distT1 = Math.Sqrt(Math.Pow((x1 - PoseEnd.X), 2) + Math.Pow((y1 - PoseEnd.Y), 2));
            double distT2 = Math.Sqrt(Math.Pow((x2 - PoseEnd.X), 2) + Math.Pow((y2 - PoseEnd.Y), 2));

            Vector2 Par = (distT1 < distT2) ? new Vector2(x1, y1) : new Vector2(x2, y2);

            double d1 = Math.Sqrt(Math.Pow((PoseEnd.X - Par.X), 2) + Math.Pow((PoseEnd.Y - Par.Y), 2)); ;// fasele end ta monhani
            double d2 = PoseEnd.Y;// fasele end ta ZZ
            double t;


            if (d1 < d2)
                t = d1 / (d1 + d2); // d1 bayasti
            else
                t = d2 / (d1 + d2);

            double Xm = (1 - t) * (3) + t * 3;
            double Ym = (1 - t) * (0) + t * (-1);

            double FT = Math.Sqrt(Math.Pow((PoseEnd.X - PoseStart.X), 2) + Math.Pow((PoseEnd.Y - PoseStart.Y), 2));
            double tS2E = FT / Ym;

            Vector3 result = new Vector3(Xm, Ym, tS2E);

            return result;
        }

        private Vector3 threeMTwo(Vector2 PoseStart, Vector2 PoseEnd)
        {

            double delta = 0.3721 - 11.08 * (0.08 + (PoseEnd.X + PoseEnd.Y));

            double x1 = (-0.61 + (delta)) / 5.54;
            double x2 = (-0.61 - (delta)) / 5.54;

            double y1 = 2.77 * Math.Pow(x1, 2) - 0.39 * x1 + 0.08;
            double y2 = 2.77 * Math.Pow(x2, 2) - 0.39 * x2 + 0.08;

            double distT1 = Math.Sqrt(Math.Pow((x1 - PoseEnd.X), 2) + Math.Pow((y1 - PoseEnd.Y), 2));
            double distT2 = Math.Sqrt(Math.Pow((x2 - PoseEnd.X), 2) + Math.Pow((y2 - PoseEnd.Y), 2));

            Vector2 Par = (distT1 < distT2) ? new Vector2(x1, y1) : new Vector2(x2, y2);

            double d1 = Math.Sqrt(Math.Pow((PoseEnd.X - Par.X), 2) + Math.Pow((PoseEnd.Y - Par.Y), 2)); ;// fasele end ta monhani
            double d2 = PoseEnd.Y;// fasele end ta ZZ
            double t;

            if (d1 < d2)
                t = d1 / (d1 + d2); // d1 bayasti
            else
                t = d2 / (d1 + d2);



            double Xm = (1 - t) * (3) + t * 3;
            double Ym = (1 - t) * (-1) + t * (-2);

            double FT = Math.Sqrt(Math.Pow((PoseEnd.X - PoseStart.X), 2) + Math.Pow((PoseEnd.Y - PoseStart.Y), 2));
            double tS2E = FT / Ym;

            Vector3 result = new Vector3(Xm, Ym, tS2E);

            return result;
        }

        private Vector3 threeMThree(Vector2 PoseStart, Vector2 PoseEnd)
        {

            double delta = 0.3249 - 14 * (0.04 + (PoseEnd.X + PoseEnd.Y));

            double x1 = (-0.57 + (delta)) / 7;
            double x2 = (-0.57 - (delta)) / 7;

            double y1 = 3.5 * Math.Pow(x1, 2) - 0.43 * x1 + 0.04;
            double y2 = 3.5 * Math.Pow(x2, 2) - 0.43 * x2 + 0.04;

            double distT1 = Math.Sqrt(Math.Pow((x1 - PoseEnd.X), 2) + Math.Pow((y1 - PoseEnd.Y), 2));
            double distT2 = Math.Sqrt(Math.Pow((x2 - PoseEnd.X), 2) + Math.Pow((y2 - PoseEnd.Y), 2));

            Vector2 Par = (distT1 < distT2) ? new Vector2(x1, y1) : new Vector2(x2, y2);

            double d1 = Math.Sqrt(Math.Pow((PoseEnd.X - Par.X), 2) + Math.Pow((PoseEnd.Y - Par.Y), 2)); ;// fasele end ta monhani
            double d2 = PoseEnd.Y;// fasele end ta ZZ
            double t;

            if (d1 < d2)
                t = d1 / (d1 + d2); // d1 bayasti
            else
                t = d2 / (d1 + d2);

            double Xm = (1 - t) * (3) + t * 3;
            double Ym = (1 - t) * (-2) + t * (-3);

            double FT = Math.Sqrt(Math.Pow((PoseEnd.X - PoseStart.X), 2) + Math.Pow((PoseEnd.Y - PoseStart.Y), 2));
            double tS2E = FT / Ym;

            Vector3 result = new Vector3(Xm, Ym, tS2E);

            return result;
        }


        // ************************* new bara ...,3 ******************************

        private Vector3 twoThree(Vector2 PoseStart, Vector2 PoseEnd)
        {

            double delta = 10.89 - 5.88 * (-1.45 + (PoseEnd.X + PoseEnd.Y));

            double x1 = (-3.39 + (delta)) / (-2.94);
            double x2 = (-3.39 - (delta)) / (-2.94);

            double y1 = -(1.47) * Math.Pow(x1, 2) - 2.39 * x1 - 1.45;
            double y2 = -(1.47) * Math.Pow(x2, 2) - 2.39 * x2 - 1.45;

            double distT1 = Math.Sqrt(Math.Pow((x1 - PoseEnd.X), 2) + Math.Pow((y1 - PoseEnd.Y), 2));
            double distT2 = Math.Sqrt(Math.Pow((x2 - PoseEnd.X), 2) + Math.Pow((y2 - PoseEnd.Y), 2));

            Vector2 Par = (distT1 < distT2) ? new Vector2(x1, y1) : new Vector2(x2, y2);

            double d1 = Math.Sqrt(Math.Pow((PoseEnd.X - Par.X), 2) + Math.Pow((PoseEnd.Y - Par.Y), 2)); ;// fasele end ta monhani
            double d2 = Math.Abs(PoseEnd.Y);// fasele end ta ZZ
            double t;

            if (d1 < d2)
                t = d1 / (d1 + d2); // d1 bayasti
            else
                t = d2 / (d1 + d2);

            double Xm = (1 - t) * (2) + t * 3;
            double Ym = (1 - t) * (3) + t * (3);

            double FT = Math.Sqrt(Math.Pow((PoseEnd.X - PoseStart.X), 2) + Math.Pow((PoseEnd.Y - PoseStart.Y), 2));
            double tS2E = FT / Xm;

            Vector3 result = new Vector3(Xm, Ym, tS2E);

            return result;
        }

        private Vector3 oneThree(Vector2 PoseStart, Vector2 PoseEnd)
        {

            double delta = 4.3264 + 10.68 * (-0.18 + (PoseEnd.X + PoseEnd.Y));

            double x1 = (-2.08 + (delta)) / (-5.34);
            double x2 = (-2.08 - (delta)) / (-5.34);

            double y1 = -(2.67) * Math.Pow(x1, 2) - 1.08 * x1 - 0.18;
            double y2 = -(1.47) * Math.Pow(x2, 2) - 1.08 * x2 - 1.18;

            double distT1 = Math.Sqrt(Math.Pow((x1 - PoseEnd.X), 2) + Math.Pow((y1 - PoseEnd.Y), 2));
            double distT2 = Math.Sqrt(Math.Pow((x2 - PoseEnd.X), 2) + Math.Pow((y2 - PoseEnd.Y), 2));

            Vector2 Par = (distT1 < distT2) ? new Vector2(x1, y1) : new Vector2(x2, y2);

            double d1 = Math.Sqrt(Math.Pow((PoseEnd.X - Par.X), 2) + Math.Pow((PoseEnd.Y - Par.Y), 2)); ;// fasele end ta monhani
            double d2 = Math.Abs(PoseEnd.Y);// fasele end ta ZZ
            double t;

            if (d1 < d2)
                t = d1 / (d1 + d2); // d1 bayasti
            else
                t = d2 / (d1 + d2);

            double Xm = (1 - t) * (1) + t * 2;
            double Ym = (1 - t) * (2) + t * (2);

            double FT = Math.Sqrt(Math.Pow((PoseEnd.X - PoseStart.X), 2) + Math.Pow((PoseEnd.Y - PoseStart.Y), 2));
            double tS2E = FT / Xm;

            Vector3 result = new Vector3(Xm, Ym, tS2E);

            return result;
        }

        private Vector3 zeroThree(Vector2 PoseStart, Vector2 PoseEnd)
        {

            double delta = 1.0465 + 4.84 * (-0.0675 + (PoseEnd.X + PoseEnd.Y));

            double x1 = (-1.023 + (delta)) / (-2.42);
            double x2 = (-1.023 - (delta)) / (-2.42);

            double y1 = -(1.21) * Math.Pow(x1, 2) - 0.023 * x1 - 0.0675;
            double y2 = -(1.21) * Math.Pow(x2, 2) - 0.023 * x2 - 0.0675;

            double distT1 = Math.Sqrt(Math.Pow((x1 - PoseEnd.X), 2) + Math.Pow((y1 - PoseEnd.Y), 2));
            double distT2 = Math.Sqrt(Math.Pow((x2 - PoseEnd.X), 2) + Math.Pow((y2 - PoseEnd.Y), 2));

            Vector2 Par = (distT1 < distT2) ? new Vector2(x1, y1) : new Vector2(x2, y2);

            double d1 = Math.Sqrt(Math.Pow((PoseEnd.X - Par.X), 2) + Math.Pow((PoseEnd.Y - Par.Y), 2)); ;// fasele end ta monhani
            double d2 = Math.Abs(PoseEnd.Y);// fasele end ta ZZ
            double t;

            if (d1 < d2)
                t = d1 / (d1 + d2); // d1 bayasti
            else
                t = d2 / (d1 + d2);

            double Xm = (1 - t) * (0) + t * 1;
            double Ym = (1 - t) * (2) + t * (2);

            double FT = Math.Sqrt(Math.Pow((PoseEnd.X - PoseStart.X), 2) + Math.Pow((PoseEnd.Y - PoseStart.Y), 2));
            double tS2E = FT / Xm;

            Vector3 result = new Vector3(Xm, Ym, tS2E); // Sorate charkhe rast, Sorate charkhe rast, time Strat to End

            return result;
        }

        private Vector3 MoneThree(Vector2 PoseStart, Vector2 PoseEnd)
        {

            double delta = 4.3264 + 15.36 * (1.01 + (PoseEnd.X + PoseEnd.Y));

            double x1 = (-2.08 + (delta)) / (-7.68);
            double x2 = (-2.08 - (delta)) / (-7.68);

            double y1 = -(3.84) * Math.Pow(x1, 2) + 1.08 * x1 + 0.01;
            double y2 = -(3.84) * Math.Pow(x2, 2) + 1.08 * x2 + 0.01;

            double distT1 = Math.Sqrt(Math.Pow((x1 - PoseEnd.X), 2) + Math.Pow((y1 - PoseEnd.Y), 2));
            double distT2 = Math.Sqrt(Math.Pow((x2 - PoseEnd.X), 2) + Math.Pow((y2 - PoseEnd.Y), 2));

            Vector2 Par = (distT1 < distT2) ? new Vector2(x1, y1) : new Vector2(x2, y2);

            double d1 = Math.Sqrt(Math.Pow((PoseEnd.X - Par.X), 2) + Math.Pow((PoseEnd.Y - Par.Y), 2)); ;// fasele end ta monhani
            double d2 = Math.Abs(PoseEnd.Y);// fasele end ta ZZ
            double t;

            if (d1 < d2)
                t = d1 / (d1 + d2); // d1 bayasti
            else
                t = d2 / (d1 + d2);

            double Xm = (1 - t) * (-1);
            double Ym = (1 - t) * (3) + t * (3);

            double FT = Math.Sqrt(Math.Pow((PoseEnd.X - PoseStart.X), 2) + Math.Pow((PoseEnd.Y - PoseStart.Y), 2));
            double tS2E = FT / Xm;

            Vector3 result = new Vector3(Xm, Ym, tS2E);

            return result;
        }
    }
}

