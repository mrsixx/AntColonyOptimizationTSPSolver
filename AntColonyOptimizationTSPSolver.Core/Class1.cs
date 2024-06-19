using AntColonyOptimizationTSPSolver.Core.Graph;
using TspLibNet;

namespace AntColonyOptimizationTSPSolver.Core
{
    public class Class1
    {
        private readonly string _problemName;
        private readonly string _tspLibPath;

        public Class1(string problemName, string tspLibPath)
        {
            _problemName = problemName;
            _tspLibPath = tspLibPath;
        }

        public void Run()
        {
            try
            {
                var tsp = LoadTsp(_problemName, ProblemType.TSP);
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
            var tspLib = new TspLib95(_tspLibPath);
            tspLib.LoadTSP(problemName);
            var item = tspLib.GetItemByName(problemName, ProblemType.TSP);

            Console.WriteLine($"Problem {item.Problem.Name}: {item.Problem.Comment}");
            Console.WriteLine($"Known optimal TSP solution: {item.OptimalTourDistance}");
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
            Console.WriteLine($"Graph reading finished...");
            Console.WriteLine($"Nodes: {graph.VertexCount}");
            Console.WriteLine($"Edges: {graph.EdgeCount}");
            return graph;
        }
    }
}