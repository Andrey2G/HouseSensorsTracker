using SensorServer;
using Serilog;
using System.Reflection;

Host.CreateDefaultBuilder(args)
           .ConfigureWebHostDefaults(webBuilder =>
           {
               webBuilder.UseStartup<Startup>();
           })
           .ConfigureAppConfiguration((context, config) =>
           {
               config.AddJsonFile("appsettings.json");
               config.AddUserSecrets(Assembly.GetExecutingAssembly());
           })
           .Build().Run();

Log.CloseAndFlush();