using AntColonyOptimizationTSPSolver.Core.Extensions;
using AntColonyOptimizationTSPSolver.Core.Graph;

namespace AntColonyOptimizationTSPSolver.Core.ACO
{
    internal class Colony
    {
        /// <summary>
        /// Ant whose found best path
        /// </summary>
        public Ant? EmployeeOfTheMonth { get; private set; }

        public List<TspEdge> BestPath { get; private set; } = new List<TspEdge>();

        public void UpdateBestPath(Ant[] ants)
        {
            foreach (var ant in ants)
            {
                var bestPathIsEmpty = !BestPath.Any();
                var antFoundBetterPath = ant.PathDistance <= BestPath.CalculateDistance();
                if (bestPathIsEmpty || antFoundBetterPath)
                {
                    BestPath = ant.Path;
                    EmployeeOfTheMonth = ant;
                }
            }
        }
    }
}
