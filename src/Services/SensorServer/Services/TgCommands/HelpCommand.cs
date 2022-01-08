using Telegram.Bot;
using Telegram.Bot.Types;

namespace SensorServer.Services.TgCommands
{
    public class HelpCommand : ITgBotCommand
    {
        public string Command => "help";

        public string Description => "Get information about Bot commands";

        public async Task<Message> Execute(ITelegramBotClient botClient, Message message)
        {
            return await botClient.SendTextMessageAsync(message.Chat.Id,
                 "Commands:\n" +
                                 "/ping - check the Bot liveness\n" +
                                 "/heathpoint_temperature_current - Current temperature from sensors at the heatpoint of hot water\n" +
                                 "/heathpoint_humidity_current - Current humidity value at the heatpoint of hot water\n" +
                                 "/heathpoint_pressure_current - Current pressure value at the heatpoint of hot water\n" +
                                 "/heathpoint_temperature_last_hour - Chart with Temperature of sensors during the last hour\n" +
                                 "/heathpoint_temperature_last_3h - Chart with Temperature of sensors during the last 3 hours\n" +
                                 "/heathpoint_temperature_last_6h - Chart with Temperature of sensors during the last 6 hours\n" +
                                 "/heathpoint_temperature_last_12h - Chart with Temperature of sensors during the last 12 hours\n" +
                                 "/heathpoint_temperature_last_24h - Chart with Temperature of sensors during the last day\n" +
                                 //"/heathpoint_water_avg_hour - Avarage temperature from sensors in heatpoint of hot water during the last hour\n" +
                                 //"/heathpoint_water_avg_day - Avarage temperature from sensors in heatpoint of hot water during the last day\n" +
                                 //"/heathpoint_water_avg_week - Avarage temperature from sensors in heatpoint of hot water during the last week\n" +
                                 //"/heathpoint_water_avg_month - Avarage temperature from sensors in heatpoint of hot water during the last month\n" +
                                 "/help - get information about the Bot commands"
                );
        }
    }
}
