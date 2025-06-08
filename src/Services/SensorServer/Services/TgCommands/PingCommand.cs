using Telegram.Bot;
using Telegram.Bot.Types;

namespace SensorServer.Services.TgCommands
{
    public class PingCommand : ITgBotCommand
    {
        public string Command => "ping";

        public string Description => "Check bot liveness";

        public async Task<Message> Execute(ITelegramBotClient botClient, Message message)
        {
            return await botClient.SendMessage(message.Chat.Id, "pong");
        }
    }
}
