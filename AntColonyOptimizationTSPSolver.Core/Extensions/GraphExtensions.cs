using AntColonyOptimizationTSPSolver.Core.Graph;

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
    }
}
