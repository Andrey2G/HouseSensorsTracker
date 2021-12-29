using SensorServer;
using Serilog;

Host.CreateDefaultBuilder(args)
           .ConfigureWebHostDefaults(webBuilder =>
           {
               webBuilder.UseStartup<Startup>();
           })
           .ConfigureAppConfiguration((context, config) =>
           {
               config.AddJsonFile("appsettings.json");
           })
           .Build().Run();

Log.CloseAndFlush();