using Telegram.Bot;
using Telegram.Bot.Types;

namespace SensorServer.Services.TgCommands
{
    public interface ITgBotCommand
    {
        string Command { get; }
        string Description { get; }

        Task<Message> Execute(ITelegramBotClient botClient, Message message);
    }
}
