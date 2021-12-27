using StackExchange.Redis;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SensorServer.Services.TgCommands
{
    public class HeatpointWaterCurrent : ITgBotCommand
    {
        private readonly ConnectionMultiplexer _redis;

        public HeatpointWaterCurrent(ConnectionMultiplexer redis)
        {
            this._redis=redis;
        }
        public string Command => "heathpoint_water_current";

        public string Description => "Current temperature from sensors in heatpoint of hot water";

        public Task<Message> Execute(ITelegramBotClient botClient, Message message)
        {
            throw new NotImplementedException();
        }
    }
}
