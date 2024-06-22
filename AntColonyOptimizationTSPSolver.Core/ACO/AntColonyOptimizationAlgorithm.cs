using AntColonyOptimizationTSPSolver.Core.Extensions;
using AntColonyOptimizationTSPSolver.Core.Graph;
using AntColonyOptimizationTSPSolver.Core.Interfaces;
using AntColonyOptimizationTSPSolver.Core.Utils;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AntColonyOptimizationTSPSolver.Core.ACO
{
    public class AntColonyOptimizationAlgorithm : IAntColonyOptimizationAlgorithm
    {
        private ILogger? _logger;
        public AntColonyOptimizationAlgorithm(TspGraph graph,
                                              double alpha = 0.9,
                                              double beta = 1.2,
                                              double rho = 0.01,
                                              double q = 5000,
                                              double initialPheromoneAmount = 0.001,
                                              int ants = 300,
                                              int iterations = 100)
        {
            Graph = graph;
            Alpha = alpha;
            Beta = beta;
            Rho = rho;
            Q = q;
            InitialPheromoneAmount = initialPheromoneAmount;
            AntCount = ants;
            Iterations = iterations;
        }

        /// <summary>
        /// Weight of distance factor constant
        /// </summary>
        public double Alpha { get; }

        /// <summary>
        /// Weight of pheromone factor constant
        /// </summary>
        public double Beta { get; }

        /// <summary>
        /// Pheromone evaporation rate constant
        /// </summary>
        public double Rho { get; }

        /// <summary>
        /// Pheromone Update constant
        /// </summary>
        public double Q { get; }
        
        /// <summary>
        /// Initial pheromone amount over graph edges
        /// </summary>
        public double InitialPheromoneAmount { get; }

        /// <summary>
        /// Amount of ants
        /// </summary>
        public int AntCount { get; }

        /// <summary>
        /// Number of iterations
        /// </summary>
        public int Iterations { get; }

        public TspGraph Graph { get; }

        public AntColonyOptimizationAlgorithm WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public IList<TspEdge> Solve()
        {
            Log($"Starting ACO algorithm with following parameters:");
            Log($"Alpha = {Alpha}; Beta = {Beta}; Rho = {Rho}; Q = {Q}.");
            
            Stopwatch sw = new();
            Colony colony = new();
            sw.Start();
            Graph.SetInitialPheromoneAmount(InitialPheromoneAmount);
            for (int i = 0; i < Iterations; i++)
            {
                Log($"\nGenerating {AntCount} artificial ants from #{i + 1}th wave...");
                Log($"Graph total amount of pheromone on #{i + 1}th wave: {Graph.CalculateTotalPheromoneAmount()}");
                Log($"Graph average amount of pheromone on #{i + 1}th wave: {Graph.CalculateAvgPheromoneAmount()}");
                Ant[] ants = GenerateAntsWave(generation: i+1);
                Log($"#{i + 1}th wave ants start to walk...");
                WaitForAntsToStop(ants);
                Log($"#{i + 1}th wave ants has stopped!");
                colony.UpdateBestPath(ants);
            }
            sw.Stop();

            Log($"Every ant has stoped after {sw.Elapsed}...");
            Log($"\nFinishing execution...");

            if(colony.EmployeeOfTheMonth is not null)
                Log($"Better solution found by ant {colony.EmployeeOfTheMonth.Id} on #{colony.EmployeeOfTheMonth.Generation}th wave!");
            return colony.BestPath;
        }

        private void Log(string message) => _logger?.Log(message);
        private static void WaitForAntsToStop(Ant[] ants) => Task.WaitAll(ants.Select(a => a.Task).ToArray());

        private Ant[] GenerateAntsWave(int generation)
        {
            Ant[] ants = new Ant[AntCount];
            for (int i = 0; i < AntCount; i++)
                ants[i] = new Ant(id: i + 1, generation, context: this);
            return ants;
        }
    }
}
