
using FigureGearBE.API;

namespace FigureGear.API
{
    public class Program

    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
             webBuilder.UseStartup<Startup>().UseUrls("http://0.0.0.0:5000"));
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}
