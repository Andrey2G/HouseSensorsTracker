using Telegram.Bot;
using Telegram.Bot.Types;

namespace SensorServer.Services.TgCommands
{
    public class PressureCurrent : ITgBotCommand
    {
        private readonly ISensorsService _sensorsService;

        public PressureCurrent(ISensorsService sensorsService)
        {
            this._sensorsService=sensorsService;
        }
        public string Command => "heathpoint_pressure_current";

        public string Description => "Current pressure value in heatpoint of hot water";

        public async Task<Message> Execute(ITelegramBotClient botClient, Message message)
        {
            string response = await _sensorsService.GetCurrentHeatpointPressure();
            return await botClient.SendTextMessageAsync(message.Chat.Id, response);
        }
    }
}
