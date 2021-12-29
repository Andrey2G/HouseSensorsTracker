using SensorServer.Models;
using SensorServer.Services;
using Serilog;
using StackExchange.Redis;
using System.Diagnostics;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

var tgBotConfig= builder.Configuration.GetSection("TelegramConfiguration").Get<TelegramConfiguration>();

var loggerConfiguration = new LoggerConfiguration();
//if (Debugger.IsAttached)
{
    //in case debugging just output everything into the console
    //for .Net Framework: set Project->Properties->Output Type->Console Application
    loggerConfiguration.WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.SystemConsoleTheme.Colored);
}
var baseDir = builder.Configuration.GetValue<string>("log_files");
loggerConfiguration.WriteTo.File(System.IO.Path.Combine(baseDir, "sensor-service-.log"), rollingInterval: RollingInterval.Hour);
Log.Logger = loggerConfiguration.CreateLogger();
builder.Services.AddLogging(config =>
{
    config.AddSerilog(Log.Logger);
});

// Add services to the container.
builder.Services.AddSingleton(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("redis")));
builder.Services.AddHostedService<WebhookConfigurerService>();
builder.Services.AddHttpClient("tg_webhook").AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(tgBotConfig.token, httpClient));

builder.Services.AddBotCommands();
builder.Services.AddTransient<ISensorsService, SensorsService>();


// Dummy business-logic service
builder.Services.AddScoped<HandleTelegramUpdateService>();

// The Telegram.Bot library heavily depends on Newtonsoft.Json library to deserialize
// incoming webhook updates and send serialized responses back.
// Read more about adding Newtonsoft.Json to ASP.NET Core pipeline:
//   https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-5.0#add-newtonsoftjson-based-json-format-support
builder.Services.AddControllers()
        .AddNewtonsoftJson();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePathBase("/sensors");
//app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    // Configure custom endpoint per Telegram API recommendations:
    // https://core.telegram.org/bots/api#setwebhook
    // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
    // using a secret path in the URL, e.g. https://www.example.com/<token>.
    // Since nobody else knows your bot's token, you can be pretty sure it's us.
    var token = tgBotConfig.token;
    endpoints.MapControllerRoute(name: "tg_webhook",
                                 pattern: $"bot/{token}",
                                 new { controller = "Bot", action = "Post" });
    endpoints.MapControllers();
});

app.Run();

Log.CloseAndFlush();