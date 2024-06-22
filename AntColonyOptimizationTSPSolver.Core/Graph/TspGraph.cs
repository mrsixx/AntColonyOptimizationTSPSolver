using QuikGraph;

namespace AntColonyOptimizationTSPSolver.Core.Graph
{
    public class TspGraph : AdjacencyGraph<int, TspEdge>
    {
        public TspGraph() : base(allowParallelEdges: true) { }

        public void SetInitialPheromoneAmount(double amount)
        {
            foreach(var edge in Edges)
                edge.DepositPheromone(amount);
        }
    }
}
