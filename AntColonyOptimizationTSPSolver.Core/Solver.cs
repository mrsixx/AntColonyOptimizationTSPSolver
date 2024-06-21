using AntColonyOptimizationTSPSolver.Core.ACO;
using AntColonyOptimizationTSPSolver.Core.Extensions;
using AntColonyOptimizationTSPSolver.Core.Graph;
using AntColonyOptimizationTSPSolver.Core.Interfaces;
using TspLibNet;

namespace AntColonyOptimizationTSPSolver.Core
{
    public class Solver
    {
        private readonly IAntColonyOptimizationAlgorithm _acoAlgorithm;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public Solver(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
            _acoAlgorithm = new AntColonyOptimizationAlgorithm(logger, antCount: 300, iterations: 100);
        }

        public void Run()
        {
            try
            {
                var tsp = LoadTsp(_configuration.ProblemName, ProblemType.TSP);
                //var dantzig42 = LoadTSPDantzig42();
                //var brazil58 = LoadTSPBrazil58();
                var graph = LoadTspGraph(tsp.Problem);
                var path = _acoAlgorithm.Solve(graph);

                _logger.Log($"Known optimal TSP solution: {tsp.OptimalTourDistance}");
                _logger.Log($"Best distance: {path.CalculateDistance()}");
                _logger.LogPath(path);
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private TspLib95Item LoadTSPDantzig42() => LoadTsp("dantzig42", ProblemType.TSP);
        private TspLib95Item LoadTSPBrazil58() => LoadTsp("brazil58", ProblemType.TSP);

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
            //var graph = new TspGraph(problem.NodeProvider.CountNodes());
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