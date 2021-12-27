using SensorServer.Models;
using SensorServer.Services;
using StackExchange.Redis;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

var tgBotConfig= builder.Configuration.GetSection("TelegramConfiguration").Get<TelegramConfiguration>();

// Add services to the container.
builder.Services.AddSingleton(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("redis")));
builder.Services.AddHostedService<WebhookConfigurerService>();
builder.Services.AddHttpClient("tg_webhook").AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(tgBotConfig.token, httpClient));

builder.Services.AddBotCommands();
builder.Services.AddTransient<ISensorsService, SensorsService>();


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

app.UseAuthorization();

app.MapControllers();

app.Run();
