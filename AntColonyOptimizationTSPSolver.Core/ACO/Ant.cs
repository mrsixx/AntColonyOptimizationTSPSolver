using AntColonyOptimizationTSPSolver.Core.Extensions;
using AntColonyOptimizationTSPSolver.Core.Graph;
using AntColonyOptimizationTSPSolver.Core.Utils;

namespace AntColonyOptimizationTSPSolver.Core.ACO
{
    internal class Ant
    {
        public Ant(int id, int generation, TspGraph graph)
        {
            Id = id;
            Generation = generation;
            Graph = graph;
            Thread = new Thread(WalkAround);
            Thread.Start();
        }

        public int Id { get; }

        public int Generation { get; }

        public int StartNode { get; private set; }

        public TspGraph Graph { get; }
        
        public Thread Thread { get; }

        public List<TspEdge> Path { get; } = new List<TspEdge>();

        public double PathDistance => Path.CalculateDistance();

        private void WalkAround()
        {
            Console.WriteLine($"Ant #{Id} start to walk...");
            StartNode = new Random().Next(1, Graph.VertexCount);
            var currentNode = StartNode;
            while (true)
            {
                if (!Graph.TryGetOutEdges(currentNode, out IEnumerable<TspEdge> possibleEdges))
                    break;

                // can assume that's possibleEdges is not null/empty because if currentNode don't have out edges will break loop
                var selectedEdge = ChooseNextStep(possibleEdges);
                Path.Add(selectedEdge);
                currentNode = selectedEdge.Target;

                // hit first node again
                if (selectedEdge.Target == StartNode && Path.Count > 1)
                {
                    UpdatePheromone();
                    break;
                }

            }
        }

        private TspEdge ChooseNextStep(IEnumerable<TspEdge> possibleEdges)
        {
            var roulette = new RouletteWheelSelection<TspEdge>();
            // calculate the product tauK^ALPHA * etaK ^ BETA of the ant k for each edge
            possibleEdges.ToList().ForEach(edge =>
            {
                // assume that x = edge.Start and y = edge.Target
                var tauK_xy = SafeDiv(1, edge.Weight); // inverso da distancia entre edge.Start e edge.Target
                var etaK_xy = SafeDiv(edge.Pheromone, edge.Weight);// concentração de feromonio entre edge.Start e edge.Target
                var factor = Math.Pow(tauK_xy, AntColonyOptimizationAlgorithm.ALPHA) * Math.Pow(etaK_xy, AntColonyOptimizationAlgorithm.BETA);
                roulette.AddItem(edge, factor);
            });

            // calculate the probability pK of the ant k choosing each edge
            // then we draw one based on probability
            return roulette.SelectItem();
            
            static double SafeDiv(double a, double b)
            {
                if (b == 0) return 0;
                return a / b;
            }
        }

        private void UpdatePheromone()
        {

        }
    }
}
