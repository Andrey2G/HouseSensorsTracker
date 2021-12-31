using Telegram.Bot;
using Telegram.Bot.Types;

namespace SensorServer.Services.TgCommands
{
    public class HeatpointWaterLastHour : ITgBotCommand
    {
        private readonly ISensorsService _sensorsService;

        public HeatpointWaterLastHour(ISensorsService sensorsService)
        {
            this._sensorsService=sensorsService;
        }
        public string Command => "heathpoint_water_last_hour";

        public string Description => "Temperature from sensors in heatpoint of hot water during the last hour";

        public async Task<Message> Execute(ITelegramBotClient botClient, Message message)
        {
            string response = await _sensorsService.GetCurrentHeatpointTemperature();
            return await botClient.SendTextMessageAsync(message.Chat.Id, response);
        }
    }
}
