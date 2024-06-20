using AntColonyOptimizationTSPSolver.Core.Interfaces;

namespace AntColonyOptimizationTSPSolver.Console
{
    public class Configuration : IConfiguration
    {
        public string ProblemName { get; set; }
        public string TspLibPath { get; set; }
    }
}
