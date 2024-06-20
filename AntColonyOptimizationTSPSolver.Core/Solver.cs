using AntColonyOptimizationTSPSolver.Core.Graph;
using AntColonyOptimizationTSPSolver.Core.Interfaces;
using TspLibNet;

namespace AntColonyOptimizationTSPSolver.Core
{
    public class Class1
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public Class1(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void Run()
        {
            try
            {
                var tsp = LoadTsp(_configuration.ProblemName, ProblemType.TSP);
                //var dantzig42 = LoadTSPDantzig42();
                //var brazil58 = LoadTSPBrazil58();
                LoadTspGraph(tsp.Problem);

            }
            catch (Exception) { }
        }

        private TspLib95Item LoadTSPDantzig42() => LoadTsp("dantzig42", ProblemType.TSP);
        private TspLib95Item LoadTSPBrazil58() => LoadTsp("brazil58", ProblemType.TSP);

        private TspLib95Item LoadTsp(string problemName, ProblemType type)
        {
            var tspLib = new TspLib95(_configuration.TspLibPath);
            tspLib.LoadTSP(problemName);
            var item = tspLib.GetItemByName(problemName, ProblemType.TSP);

            _logger.Log($"Problem {item.Problem.Name}: {item.Problem.Comment}");
            _logger.Log($"Known optimal TSP solution: {item.OptimalTourDistance}");
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
                        graph.AddEdge(new TspEdge(source.Id, source.Id, weight));
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