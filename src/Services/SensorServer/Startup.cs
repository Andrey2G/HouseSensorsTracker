using SensorServer.Models;
using SensorServer.Services;
using SensorsServer.Services;
using Serilog;
using StackExchange.Redis;
using Telegram.Bot;

namespace SensorServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _botConfig = Configuration.GetSection("TelegramConfiguration").Get<TelegramConfiguration>();
        }

        public IConfiguration Configuration { get; }
        private TelegramConfiguration _botConfig { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var loggerConfiguration = new LoggerConfiguration();
            //if (Debugger.IsAttached)
            {
                //in case debugging just output everything into the console
                //for .Net Framework: set Project->Properties->Output Type->Console Application
                loggerConfiguration.WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.SystemConsoleTheme.Colored);
            }
            loggerConfiguration.WriteTo.File(System.IO.Path.Combine(Configuration["log_files"], "sensor-service-.log"), rollingInterval: RollingInterval.Hour);
            Log.Logger = loggerConfiguration.CreateLogger();
            services.AddLogging(config =>
            {
                config.AddSerilog(Log.Logger);
            });

            // Add services to the container.
            services.AddSingleton(ConnectionMultiplexer.Connect(Configuration.GetConnectionString("redis")));
            services.AddTransient<IDataContext, DataContext>();
            services.AddTransient<ISensorsService, SensorsService>();
            services.AddBotCommands();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();



            // There are several strategies for completing asynchronous tasks during startup.
            // Some of them could be found in this article https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
            // We are going to use IHostedService to add and later remove Webhook
            services.AddHostedService<WebhookConfigurerService>();

            // Register named HttpClient to get benefits of IHttpClientFactory
            // and consume it with ITelegramBotClient typed client.
            // More read:
            //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
            //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient("tg_webhook")
                    .AddTypedClient<ITelegramBotClient>(httpClient
                        => new TelegramBotClient(_botConfig.token, httpClient));

            // Dummy business-logic service
            services.AddScoped<HandleTelegramUpdateService>();

            // The Telegram.Bot library heavily depends on Newtonsoft.Json library to deserialize
            // incoming webhook updates and send serialized responses back.
            // Read more about adding Newtonsoft.Json to ASP.NET Core pipeline:
            //   https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-5.0#add-newtonsoftjson-based-json-format-support
            services.AddControllers()
                    .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Configure the HTTP request pipeline.
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                // Configure custom endpoint per Telegram API recommendations:
                // https://core.telegram.org/bots/api#setwebhook
                // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
                // using a secret path in the URL, e.g. https://www.example.com/<token>.
                // Since nobody else knows your bot's token, you can be pretty sure it's us.
                var token = _botConfig.token;
                endpoints.MapControllerRoute(name: "tg_webhook",
                                             pattern: $"bot/{token}",
                                             new { controller = "Bot", action = "Post" });
                endpoints.MapControllers();
            });
        }
    }
}
