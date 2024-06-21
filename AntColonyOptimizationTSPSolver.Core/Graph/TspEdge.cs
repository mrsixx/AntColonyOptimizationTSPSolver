using AntColonyOptimizationTSPSolver.Core.ACO;
using QuikGraph;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AntColonyOptimizationTSPSolver.Core.Graph
{
    public class TspEdge : IEdge<int>
    {
        private readonly object _lock = new();
        private double _pheromoneAmount = AntColonyOptimizationAlgorithm.INITIAL_PHEROMONE_AMOUNT;
        public TspEdge(int source, int target, double weight)
        {
            Source = source;
            Target = target; 
            Weight = weight;
        }

        public int Source { get; }

        public int Target { get; }

        public double Weight { get; }

        public double Pheromone
        {
            get 
            {
                lock (_lock)
                {
                    return _pheromoneAmount;
                }
            }
        }

        public void EvaporatePheromone(double rate)
        {
            lock (_lock)
            {
                var old = _pheromoneAmount;
                _pheromoneAmount = (1 - rate) * _pheromoneAmount;
                //Console.WriteLine($"{Source}->{Target}: evaporate {old} to {_pheromoneAmount}");
            }
        }

        public void DepositPheromone(double amount)
        {
            lock (_lock)
            {

                _pheromoneAmount += amount;
                //Console.WriteLine($"{Source}->{Target}: deposited {amount} to {_pheromoneAmount}");
            }
        }
    }
}
