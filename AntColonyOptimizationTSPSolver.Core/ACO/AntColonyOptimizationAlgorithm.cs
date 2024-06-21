using AntColonyOptimizationTSPSolver.Core.Extensions;
using AntColonyOptimizationTSPSolver.Core.Graph;
using AntColonyOptimizationTSPSolver.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AntColonyOptimizationTSPSolver.Core.ACO
{
    public class AntColonyOptimizationAlgorithm : IAntColonyOptimizationAlgorithm
    {
        public const double ALPHA = 0.9; // weight of distance factor
        public const double BETA = 1.2; // weight of pheromone factor
        public const double RHO = 0.01; // pheromone evaporation rate
        public const double INITIAL_PHEROMONE_AMOUNT = 0.001; //initial pheromone amount
        public const double PHEROMONE_UPDATE_CONSTANT = 5000.0;

        private readonly int _antCount;
        private readonly int _iterations;
        private readonly ILogger _logger;
        public AntColonyOptimizationAlgorithm(ILogger logger, int antCount = 300, int iterations = 100)
        {
            _logger = logger;
            _antCount = antCount;
            _iterations = iterations;
        }

        public IList<TspEdge> Solve(TspGraph graph)
        {
            Stopwatch sw = new();
            Colony colony = new();

            _logger.Log($"Starting ACO algorithm with following parameters:");
            _logger.Log($"Alpha = {ALPHA}; Beta = {BETA}; Rho = {RHO}; Pheromone constant = {PHEROMONE_UPDATE_CONSTANT}.");
            sw.Start();
            for (int i = 0; i < _iterations; i++)
            {
                _logger.Log($"\nGenerating {_antCount} artificial ants from #{i + 1}th wave...");
                Ant[] ants = GenerateAntsWave(graph, i);
                _logger.Log($"#{i + 1}th wave ants start to walk...");
                WaitForAntsToStop(ants);
                _logger.Log($"#{i + 1}th wave ants has stopped!");
                colony.UpdateBestPath(ants);
            }
            sw.Stop();

            _logger.Log($"Every ant has stoped after {sw.Elapsed}...");
            _logger.Log($"\nFinishing execution...");

            if(colony.EmployeeOfTheMonth is not null)
                _logger.Log($"Better solution found by ant {colony.EmployeeOfTheMonth.Id} on #{colony.EmployeeOfTheMonth.Generation}th wave!");
            return colony.BestPath;
        }

        private static void WaitForAntsToStop(Ant[] ants) => Task.WaitAll(ants.Select(a => a.Task).ToArray());

        private Ant[] GenerateAntsWave(TspGraph graph, int i)
        {
            Ant[] ants = new Ant[_antCount];
            for (int j = 0; j < _antCount; j++)
                ants[j] = new Ant(j + 1, i + 1, graph);
            return ants;
        }
    }
}
