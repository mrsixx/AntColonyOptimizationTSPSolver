using AntColonyOptimizationTSPSolver.Core.ACO;
using AntColonyOptimizationTSPSolver.Core.Extensions;
using AntColonyOptimizationTSPSolver.Core.Graph;
using AntColonyOptimizationTSPSolver.Core.Interfaces;
using TspLibNet;

namespace AntColonyOptimizationTSPSolver.Core
{
    public class Solver
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public Solver(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void Run()
        {
            try
            {
                //var tsp = LoadTsp("dantzig42", ProblemType.TSP);
                var tsp = LoadTsp("brazil58", ProblemType.TSP);
                double quadraticError = 0;
                double numberOfTests = 10;
                for(int i = 0; i < numberOfTests; i++)
                {
                    _logger.Log($"Test #{i + 1} starting");
                    var graph = LoadTspGraph(tsp.Problem);
                    var aco = new AntColonyOptimizationAlgorithm(graph,
                        alpha: 1.3,
                        beta: 0.9,
                        rho: 0.08,
                        q: 5000,
                        ants: 100,
                        initialPheromoneAmount: 0.001,
                        iterations: 10).Verbose(_logger);
                    var bestPath = aco.Solve();

                    var bestDistance = bestPath.CalculateDistance();
                    var relativeError = bestDistance.CalculateRelativeError(exactValue: tsp.OptimalTourDistance);
                    quadraticError += Math.Pow(tsp.OptimalTourDistance - bestDistance, 2);
                    _logger.Log($"Known optimal TSP solution: {tsp.OptimalTourDistance}");
                    _logger.Log($"Best distance: {bestDistance}");
                    _logger.Log($"Test #{i+1} relative Error: {relativeError}%");
                    _logger.LogPath(bestPath);
                    _logger.Log("\n\n\n");
                }

                _logger.Log($"REMQ: {Math.Sqrt(quadraticError / numberOfTests)}");
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private TspLib95Item LoadTsp(string problemName, ProblemType type)
        {
            var tspLib = new TspLib95(_configuration.TspLibPath);
            tspLib.LoadTSP(problemName);
            var item = tspLib.GetItemByName(problemName, type);

            _logger.Log($"Problem {item.Problem.Name}: {item.Problem.Comment}");
            return item;
        }
        
        private TspGraph LoadTspGraph(IProblem problem)
        {
            var graph = new TspGraph();
            problem.NodeProvider.GetNodes().ForEach(node => graph.AddVertex(node.Id));
            problem.NodeProvider.GetNodes().ForEach(source =>
            {
                problem.NodeProvider.GetNodes().ForEach(target =>
                {
                    if(problem.EdgeProvider.HasEdge(source, target))
                    {
                        var weight = problem.EdgeWeightsProvider.GetWeight(source, target);
                        graph.AddEdge(new TspEdge(source.Id, target.Id, weight));
                    }
                });
            });
            _logger.Log($"Graph reading finished...");
            _logger.Log($"Nodes: {graph.VertexCount}");
            _logger.Log($"Edges: {graph.EdgeCount}");
            return graph;
        }
    }
}