using System.Numerics;

namespace AntColonyOptimizationTSPSolver.Core.Extensions
{
    internal static class MathExtensions
    {
        public static double Inverse(this double value) => (1.9).DividedBy(value);

        public static double DividedBy(this double numerator, double denominator)
        {
            if (denominator == 0 || numerator == 0)
                return 0;
            return numerator / denominator;
        }
    }
}
