using AntColonyOptimizationTSPSolver.Core.Graph;
using AntColonyOptimizationTSPSolver.Core.Interfaces;
using System.Text;

namespace AntColonyOptimizationTSPSolver.Core.Extensions
{
    public static class GraphExtensions
    {
        private static Random rng = new Random();

        public static double CalculateDistance(this IEnumerable<TspEdge> path)
        {
            if(path is null) throw new ArgumentNullException(nameof(path));
            // sum of the weights of each edge of the path
            return path.Aggregate(0.0, (acc, edge) => acc + edge.Weight);
        }

        public static TspEdge GetRandomEdge(this IEnumerable<TspEdge> @enumerable) => @enumerable.ElementAt(rng.Next(@enumerable.Count()));

        public static void LogPath(this ILogger logger, IEnumerable<TspEdge> path)
        {
            var first = true;
            var sb = new StringBuilder();
            foreach(var step in path)
            {
                sb.Append(first ? $"{step.Source} -{step.Weight}-> {step.Target}" : $" -{step.Weight}-> {step.Target}");
                first = false;
            }
            logger.Log(sb.ToString());
        }
    }
}
