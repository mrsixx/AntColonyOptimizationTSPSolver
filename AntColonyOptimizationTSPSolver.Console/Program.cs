// See https://aka.ms/new-console-template for more information
using AntColonyOptimizationTSPSolver.Console;
using AntColonyOptimizationTSPSolver.Core;

var startup = new Startup();
var logger = new Logger();
var solver = new Solver(startup.Configuration, logger);
solver.Run();
Console.ReadKey();