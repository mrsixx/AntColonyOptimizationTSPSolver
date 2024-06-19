using QuikGraph;

namespace AntColonyOptimizationTSPSolver.Core.Graph
{
    public class TspEdge : IEdge<int>
    {
        public TspEdge(int source, int target, double weight)
        {
            Source = source;
            Target = target; 
            Weight = weight;
        }

        public int Source { get; }


        public int Target { get; }

        public double Weight { get; }
    }
}
