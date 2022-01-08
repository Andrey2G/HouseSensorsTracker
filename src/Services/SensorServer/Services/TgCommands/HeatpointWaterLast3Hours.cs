using Telegram.Bot;
using Telegram.Bot.Types;

namespace SensorServer.Services.TgCommands
{
    public class HeatpointWaterLast3Hours : ITgBotCommand
    {
        private readonly ISensorsService _sensorsService;
        private readonly IWebHostEnvironment _env;

        public HeatpointWaterLast3Hours(ISensorsService sensorsService, IWebHostEnvironment env)
        {
            this._sensorsService=sensorsService;
            this._env=env;
        }
        public string Command => "heathpoint_temperature_last_3h";

        public string Description => "Temperature from sensors at the heatpoint of hot water during the last 3 hours";

        public async Task<Message> Execute(ITelegramBotClient botClient, Message message)
        {
            using var sr = System.IO.File.OpenRead(System.IO.Path.Combine(_env.WebRootPath, "images/temperature-last-3h.png"));
            return await botClient.SendPhotoAsync(message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(sr, "graph"));
        }
    }
}
