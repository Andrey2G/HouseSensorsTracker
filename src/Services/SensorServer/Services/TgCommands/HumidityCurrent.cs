using Telegram.Bot;
using Telegram.Bot.Types;

namespace SensorServer.Services.TgCommands
{
    public class HumidityCurrent : ITgBotCommand
    {
        private readonly ISensorsService _sensorsService;

        public HumidityCurrent(ISensorsService sensorsService)
        {
            this._sensorsService=sensorsService;
        }
        public string Command => "heathpoint_humidity_current";

        public string Description => "Current humidity value in heatpoint of hot water";

        public async Task<Message> Execute(ITelegramBotClient botClient, Message message)
        {
            string response = await _sensorsService.GetCurrentHeatpointHumidity();
            return await botClient.SendMessage(message.Chat.Id, response);
        }
    }
}
