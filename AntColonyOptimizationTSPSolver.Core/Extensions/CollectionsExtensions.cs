namespace AntColonyOptimizationTSPSolver.Core.Extensions
{
    internal static class CollectionsExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> @list) => !@list.Any();
    }
}
