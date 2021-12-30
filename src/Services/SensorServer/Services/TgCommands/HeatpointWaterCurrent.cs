using StackExchange.Redis;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SensorServer.Services.TgCommands
{
    public class HeatpointWaterCurrent : ITgBotCommand
    {
        private readonly ISensorsService _sensorsService;

        public HeatpointWaterCurrent(ISensorsService sensorsService)
        {
            this._sensorsService=sensorsService;
        }
        public string Command => "heathpoint_water_current";

        public string Description => "Current temperature from sensors in heatpoint of hot water";

        public async Task<Message> Execute(ITelegramBotClient botClient, Message message)
        {
            string response = await _sensorsService.GetCurrentHeatpointTemperature();
            return await botClient.SendTextMessageAsync(message.Chat.Id, response);
        }
    }
}
