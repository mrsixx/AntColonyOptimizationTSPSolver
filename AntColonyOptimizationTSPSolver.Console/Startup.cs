using Microsoft.Extensions.Configuration;

namespace AntColonyOptimizationTSPSolver.Console
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", true, true);

            IConfiguration config = builder.Build();

            Configuration = config.GetSection("TspLib").Get<Configuration>();
        }

        public Configuration Configuration { get; private set; }
    }
}
