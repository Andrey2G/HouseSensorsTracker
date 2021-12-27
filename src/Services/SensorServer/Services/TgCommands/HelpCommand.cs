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
                                 "/heathpoint_water_current - Current temperature from sensors in heatpoint of hot water\n" +
                                 "/heathpoint_water_avg_hour - Avarage temperature from sensors in heatpoint of hot water during the last hour\n" +
                                 "/heathpoint_water_avg_day - Avarage temperature from sensors in heatpoint of hot water during the last day\n" +
                                 "/heathpoint_water_avg_week - Avarage temperature from sensors in heatpoint of hot water during the last week\n" +
                                 "/heathpoint_water_avg_month - Avarage temperature from sensors in heatpoint of hot water during the last month\n" +
                                 "/help - get information about the Bot commands"
                );
        }
    }
}
