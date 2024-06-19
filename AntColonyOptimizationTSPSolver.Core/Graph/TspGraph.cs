using QuikGraph;

namespace AntColonyOptimizationTSPSolver.Core.Graph
{
    public class TspGraph : AdjacencyGraph<int, TspEdge>
    {
        public TspGraph() : base(allowParallelEdges: true)
        {
        }
    }
}
