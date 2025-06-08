using Telegram.Bot;
using Telegram.Bot.Types;

namespace SensorServer.Services.TgCommands
{
    public class HeatpointWaterLast12Hours : ITgBotCommand
    {
        private readonly ISensorsService _sensorsService;
        private readonly IWebHostEnvironment _env;

        public HeatpointWaterLast12Hours(ISensorsService sensorsService, IWebHostEnvironment env)
        {
            this._sensorsService=sensorsService;
            this._env=env;
        }
        public string Command => "heathpoint_temperature_last_12hours";

        public string Description => "Temperature from sensors at the heatpoint of hot water during the last 12 hours";

        public async Task<Message> Execute(ITelegramBotClient botClient, Message message)
        {
            using var sr = System.IO.File.OpenRead(System.IO.Path.Combine(_env.WebRootPath, "images/temperature-last-12h.png"));
            return await botClient.SendPhoto(message.Chat.Id, InputFile.FromStream(sr, "graph"));
        }
    }
}
