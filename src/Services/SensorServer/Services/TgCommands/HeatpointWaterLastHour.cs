using Telegram.Bot;
using Telegram.Bot.Types;

namespace SensorServer.Services.TgCommands
{
    public class HeatpointWaterLastHour : ITgBotCommand
    {
        private readonly ISensorsService _sensorsService;
        string _webhookUrl = "";
        public HeatpointWaterLastHour(ISensorsService sensorsService, IConfiguration configuration)
        {
            this._sensorsService=sensorsService;
            _webhookUrl=configuration.GetValue<string>("TelegramConfiguration:webhook_url");
        }
        public string Command => "heathpoint_temperature_last_hour";

        public string Description => "Temperature from sensors at the heatpoint of hot water during the last hour";

        public async Task<Message> Execute(ITelegramBotClient botClient, Message message)
        {
            var response = $"{_webhookUrl}/TemperatureCharts";
            return await botClient.SendTextMessageAsync(message.Chat.Id, response);
        }
    }
}
