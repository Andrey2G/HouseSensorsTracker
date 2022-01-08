using Telegram.Bot;
using Telegram.Bot.Types;

namespace SensorServer.Services.TgCommands
{
    public class HeatpointWaterLast6Hours : ITgBotCommand
    {
        private readonly ISensorsService _sensorsService;
        private readonly IWebHostEnvironment _env;

        public HeatpointWaterLast6Hours(ISensorsService sensorsService, IWebHostEnvironment env)
        {
            this._sensorsService=sensorsService;
            this._env=env;
        }
        public string Command => "heathpoint_temperature_last_6h";

        public string Description => "Temperature from sensors at the heatpoint of hot water during the last 6 hours";

        public async Task<Message> Execute(ITelegramBotClient botClient, Message message)
        {
            using var sr = System.IO.File.OpenRead(System.IO.Path.Combine(_env.WebRootPath, "images/temperature-last-6h.png"));
            return await botClient.SendPhotoAsync(message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(sr, "graph"));
        }
    }
}
