using AntColonyOptimizationTSPSolver.Core.Extensions;
using AntColonyOptimizationTSPSolver.Core.Graph;
using AntColonyOptimizationTSPSolver.Core.Utils;

namespace AntColonyOptimizationTSPSolver.Core.ACO
{
    internal class Ant
    {
        public Ant(int id, int generation, AntColonyOptimizationAlgorithm context)
        {
            Id = id;
            Generation = generation;
            Context = context;
            UnvisitedNodes = Context.Graph.Vertices.ToList();
            Task = new Task(() => WalkAround());
            Task.Start();
        }

        public int Id { get; }

        public int Generation { get; }

        public int StartNode { get; private set; }

        public AntColonyOptimizationAlgorithm Context { get; }


        public List<int> UnvisitedNodes { get; }
        
        public Task Task { get; }

        public List<TspEdge> Path { get; } = new List<TspEdge>();

        public TspGraph Graph => Context.Graph;

        public double PathDistance => Path.CalculateDistance();

        private void WalkAround()
        {
            StartNode = Utils.Math.Random.Next(1, Graph.VertexCount);
            UnvisitedNodes.Remove(StartNode);
            var currentNode = StartNode;
            while (true)
            {
                if (!Graph.TryGetOutEdges(currentNode, out IEnumerable<TspEdge> currentNodeEdges))
                    break;

                List<TspEdge> possibleEdges = ExtractPossibleEdges(currentNode, currentNodeEdges);

                if (possibleEdges.IsEmpty())
                    break;

                var selectedEdge = ChooseNextStep(possibleEdges);
                Path.Add(selectedEdge);
                currentNode = selectedEdge.Target;
                UnvisitedNodes.Remove(currentNode);
                // hit first node again
                if (selectedEdge.Target == StartNode && UnvisitedNodes.IsEmpty())
                {
                    UpdatePheromone();
                    break;
                }

            }
        }

        private List<TspEdge> ExtractPossibleEdges(int currentNode, IEnumerable<TspEdge> currentNodeEdges)
        {
            // possible edges to ant visit are unvisited edges and an edge from start node if all are visited
            var possibleEdges = currentNodeEdges.Where(edge => UnvisitedNodes.Contains(edge.Target)).ToList();
            if (UnvisitedNodes.IsEmpty() && Graph.TryGetEdge(currentNode, StartNode, out TspEdge cycleEdge))
                possibleEdges.Add(cycleEdge);
            return possibleEdges;
        }

        private TspEdge ChooseNextStep(IEnumerable<TspEdge> possibleEdges)
        {
            var roulette = new RouletteWheelSelection<TspEdge>();
            // calculate the product tauK^ALPHA * etaK ^ BETA of the ant k for each edge
            possibleEdges.ToList().ForEach(edge =>
            {
                // assume that x = edge.Start and y = edge.Target
                var tauK_xy = edge.Weight.Inverse(); // inverso da distancia entre edge.Start e edge.Target
                var etaK_xy = edge.Pheromone.DividedBy(edge.Weight);// concentração de feromonio entre edge.Start e edge.Target
                var factor = System.Math.Pow(tauK_xy, Context.Alpha) * System.Math.Pow(etaK_xy, Context.Beta);
                roulette.AddItem(edge, factor);
            });

            // calculate the probability pK of the ant k choosing each edge
            // then we draw one based on probability
            return roulette.SelectItem();
        }

        private void UpdatePheromone()
        {
            foreach(var step in Path)
            {
                step.EvaporatePheromone(rate: Context.Rho);
                step.DepositPheromone(amount: Context.Q.DividedBy(PathDistance));
            }
        }
    }
}
