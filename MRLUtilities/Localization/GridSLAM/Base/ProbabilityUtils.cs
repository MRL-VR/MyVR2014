using System;

using MRL.CustomMath;

namespace MRL.Exploration.GridSLAM.Base
{

    /// <summary>
    /// Defines the type of normalization to do.
    /// </summary>
    public enum Normalization
    {
        None,
        MaxToOne,
        IntegralToOne
    }

    /// <summary>
    /// Common probabilistic functions: normalization, gaussians and log odds.
    /// </summary>
    public static class ProbabilityUtils
    {
        private static Random _random;
        static ProbabilityUtils()
        {
            _random = new Random();
        }

        /// <summary>
        /// Gets a sample from a gaussian distribution
        /// </summary>
        /// <param name="variance"></param>
        /// <returns></returns>
        public static double RandomFromGaussian(double variance)
        {
            double deviation = Math.Sqrt(variance);
            double tot = 0;
            for (int i = 0; i < 12; i++)
            {
                tot += (_random.NextDouble() * deviation * 2) - deviation;
            }
            return tot / 2;
        }


        public static double GetEffectiveSampleSize(double[] weights)
        {
            double tot = 0;
            foreach (double d in weights)
            {
                tot += Math.Pow(d, 2);
            }
            return 1 / (tot * weights.Length);
        }

        /// <summary>
        /// Normalizes a distribution
        /// </summary>
        /// <param name="distribution"></param>
        /// <param name="normalization"></param>
        /// <returns></returns>
        public static double[] Normalize(double[] distribution, Normalization normalization)
        {
            switch (normalization)
            {
                case Normalization.None:
                    break;
                case Normalization.MaxToOne:
                    double max = GetMax(distribution);
                    if (max > 0)
                    {
                        for (int i = 0; i < distribution.Length; i++)
                        {
                            distribution[i] = distribution[i] / max;
                        }
                    }
                    else
                    {
                        return distribution;
                    }
                    break;
                case Normalization.IntegralToOne:
                    double tot = GetTotal(distribution);
                    if (tot > 0)
                    {
                        for (int i = 0; i < distribution.Length; i++)
                        {
                            distribution[i] = distribution[i] / tot;
                        }
                    }
                    else
                    {
                        return distribution;
                    }
                    break;
            }
            return distribution;
        }


        /// <summary>
        /// Gets the max value of the distribution
        /// </summary>
        /// <param name="distribution"></param>
        /// <returns></returns>
        public static double GetMax(double[] distribution)
        {
            double max = double.MinValue;
            foreach (double d in distribution)
            {
                if (d > max)
                {
                    max = d;
                }
            }
            return max;
        }

        /// <summary>
        /// Gets the max value of a 2D distribution
        /// </summary>
        /// <param name="distribution"></param>
        /// <returns></returns>
        public static double GetMax(double[,] distribution)
        {
            double max = double.MinValue;
            for (int x = 0; x < distribution.GetLength(0); x++)
            {
                for (int y = 0; y < distribution.GetLength(1); y++)
                {
                    if (distribution[x, y] > max)
                    {
                        max = distribution[x, y];
                    }
                }
            }
            return max;
        }

        /// <summary>
        /// Gets the sum of the entire distribution
        /// </summary>
        /// <param name="distribution"></param>
        /// <returns></returns>
        public static double GetTotal(double[] distribution)
        {
            double tot = 0;
            foreach (double d in distribution)
            {
                tot += d;
            }
            return tot;
        }


        /// <summary>
        /// Does 'low variance re-sampling'
        /// Expects a set where the integral is one
        /// </summary>
        /// <param name="weights">The weights of current particles</param>
        /// <returns>The indexes of particles in the new set</returns>
        public static int[] ReSample(double[] weights)
        {
            int[] returnSamples = new int[weights.Length];
            int s = 0;

            double r = _random.NextDouble() / weights.Length;
            double c = weights[0];
            int i = 0;

            for (int m = 0; m < weights.Length; m++)
            {
                double U = (m > 0) ? r + ((m * 1.0) / weights.Length) : r;
                while (U > c)
                {
                    i = (i >= weights.Length - 1) ? 0 : i + 1;
                    c = c + weights[i];
                }
                returnSamples[s++] = i;
            }
            return returnSamples;
        }


        /// <summary>
        /// Returns the root of the mean square error between a perfect state and an array of imperfect states.
        /// </summary>
        /// <param name="states"></param>
        /// <param name="perfectState"></param>
        /// <returns></returns>
        public static double GetRMSError(Pose2D[] states, Pose2D perfectState)
        {
            // DDState meanState = GetMeanState(states);

            double d = 0;
            foreach (Pose2D state in states)
            {
                d += Math.Pow(Math.Sqrt(Math.Pow(state.X - perfectState.X, 2) + Math.Pow(state.Y - perfectState.Y, 2)), 2);

            }
            d = d / states.Length;
            return d;
        }
    }
}


