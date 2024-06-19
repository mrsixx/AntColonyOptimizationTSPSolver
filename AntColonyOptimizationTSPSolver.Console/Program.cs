// See https://aka.ms/new-console-template for more information
using AntColonyOptimizationTSPSolver.Console;
using AntColonyOptimizationTSPSolver.Core;

var startup = new Startup();
var @class = new Class1(startup.Configuration.ProblemName, startup.Configuration.TspLibPath);
@class.Run();
Console.ReadKey();