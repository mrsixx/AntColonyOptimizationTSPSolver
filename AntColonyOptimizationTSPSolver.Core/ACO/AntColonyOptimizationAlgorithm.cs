using AntColonyOptimizationTSPSolver.Core.Extensions;
using AntColonyOptimizationTSPSolver.Core.Graph;
using AntColonyOptimizationTSPSolver.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AntColonyOptimizationTSPSolver.Core.ACO
{
    public class AntColonyOptimizationAlgorithm : IAntColonyOptimizationAlgorithm
    {
        public const double ALPHA = 0.5;
        public const double BETA = 1.2;
        public const double PHEROMONE_EVAPORATION_RATE = 0.40;
        public const double PHEROMONE_CONSTANT = 1000.0;

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
            _logger.Log($"Alpha = {ALPHA}; Beta = {BETA}; Pheromone evaporation rate = {PHEROMONE_EVAPORATION_RATE}; Pheromone constant = {PHEROMONE_CONSTANT}.");
            sw.Start();
            for (int i = 0; i < _iterations; i++)
            {
                _logger.Log($"\nGenerating {_antCount} artificial ants from #{i + 1}th wave...");
                Ant[] ants = GenerateAntsWave(graph, i);
                WaitForAntsToStop(ants);
                colony.UpdateBestPath(ants);
            }
            sw.Stop();

            _logger.Log($"Every ant has stoped after {sw.Elapsed}...");
            _logger.Log($"\nFinishing execution...");

            if(colony.EmployeeOfTheMonth is not null)
                _logger.Log($"Better solution found by ant {colony.EmployeeOfTheMonth.Id} on #{colony.EmployeeOfTheMonth.Generation}th wave!");
            return colony.BestPath;
        }

        private static void WaitForAntsToStop(Ant[] ants)
        {
            foreach (var ant in ants)
                ant.Thread.Join();
        }

        private Ant[] GenerateAntsWave(TspGraph graph, int i)
        {
            Ant[] ants = new Ant[_antCount];
            for (int j = 0; j < _antCount; j++)
                ants[j] = new Ant(j + 1, i + 1, graph);
            return ants;
        }
    }
}
