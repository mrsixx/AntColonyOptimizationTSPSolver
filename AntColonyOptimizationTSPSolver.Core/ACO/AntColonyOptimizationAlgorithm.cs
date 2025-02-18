﻿using AntColonyOptimizationTSPSolver.Core.Extensions;
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
        /// <summary>
        /// TSP solver using the Ant Colony Optimization method
        /// </summary>
        /// <param name="graph">Graph on which the TSP was modeled</param>
        /// <param name="alpha">Weight of distance factor constant</param>
        /// <param name="beta">Weight of pheromone factor constant</param>
        /// <param name="rho">Pheromone evaporation rate constant</param>
        /// <param name="q">Pheromone Update constant</param>
        /// <param name="initialPheromoneAmount">Initial pheromone amount over graph edges</param>
        /// <param name="ants">Amount of ants</param>
        /// <param name="iterations">Number of iterations</param>
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

        public AntColonyOptimizationAlgorithm Verbose(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public IList<TspEdge> Solve()
        {
            Log($"Starting ACO algorithm with following parameters:");
            Log($"Alpha = {Alpha}; Beta = {Beta}; Rho = {Rho}; Q = {Q}; Initial pheromone = {InitialPheromoneAmount}.");
            
            Stopwatch sw = new();
            Stopwatch iSw = new();
            Colony colony = new();
            sw.Start();
            Graph.SetInitialPheromoneAmount(InitialPheromoneAmount);
            for (int i = 0; i < Iterations; i++)
            {
                Log($"\nGenerating {AntCount} artificial ants from #{i + 1}th wave...");
                Log($"Graph pheromone on #{i + 1}th wave: total = {Graph.CalculateTotalPheromoneAmount()}; average = {Graph.CalculateAvgPheromoneAmount()}");
                iSw.Restart();
                Ant[] ants = GenerateAntsWave(generation: i+1);
                Log($"#{i + 1}th wave ants start to walk...");
                WaitForAntsToStop(ants);
                iSw.Stop();
                Log($"#{i + 1}th wave ants has stopped after {iSw.Elapsed}!");
                colony.UpdateBestPath(ants);
            }
            sw.Stop();

            Log($"Every ant has stopped after {sw.Elapsed}.");
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
