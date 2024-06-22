using AntColonyOptimizationTSPSolver.Core.Graph;

namespace AntColonyOptimizationTSPSolver.Core.Interfaces
{
    internal interface IAntColonyOptimizationAlgorithm
    {
        IList<TspEdge> Solve();
    }
}
