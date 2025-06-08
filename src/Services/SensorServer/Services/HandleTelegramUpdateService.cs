using SensorServer.Services.TgCommands;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SensorServer.Services
{
    public class HandleTelegramUpdateService
    {
        private readonly ITelegramBotClient _tgBotClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<HandleTelegramUpdateService> _logger;

        public HandleTelegramUpdateService(ITelegramBotClient botClient, IServiceProvider serviceProvider, ILogger<HandleTelegramUpdateService> logger)
        {
            _tgBotClient = botClient;
            _serviceProvider=serviceProvider;
            _logger = logger;
        }

        string _botUsername = "";
        public async Task<string> BotUserName() { if (string.IsNullOrEmpty(_botUsername)) _botUsername= $"@{(await _tgBotClient.GetMe()).Username}"; return _botUsername; }

        public async Task EchoAsync(Update update)
        {
            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                // UpdateType.Poll:
                UpdateType.Message => OnMessageReceived(update?.Message ?? new Message()),
                UpdateType.EditedMessage => OnMessageReceived(update?.EditedMessage ?? new Message()),
                //UpdateType.CallbackQuery => ,
                //UpdateType.InlineQuery => 
                //UpdateType.ChosenInlineResult => , 
                _ => UnknownUpdateHandlerAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(exception);
            }
        }

        private async Task OnMessageReceived(Message message)
        {
            _logger.LogInformation("Receive message: {message_id} {message_type}", message.MessageId, message.Type);
            
            //only text messages at the moment
            if (message.Type != MessageType.Text)
                return;

            //no empty messages allowed to be processed
            if (string.IsNullOrEmpty(message.Text))
                return;
            
            var botCommand = message.Entities?.FirstOrDefault(c => c.Type==MessageEntityType.BotCommand);
            //no command - just exit at the moment
            //TODO: make AI request
            if (botCommand == null) return;

            var command = message.Text.Substring(botCommand.Offset, botCommand.Length);
            //replace bot usernamne
            command = command.Replace(await BotUserName(), string.Empty).Replace("/","");
            var service = _serviceProvider.GetServices<ITgBotCommand>().SingleOrDefault(s => s.Command==command);
            if (service!=null)
            {
                var sentMessage = await service.Execute(_tgBotClient, message);
                _logger.LogInformation("The message was sent with id: {sentMessageId}", sentMessage.MessageId);
            }
            else
            {
                //unknown command
                await _tgBotClient.SendMessage(message.Chat.Id, "Unknown command. Try /help to get the list of available commands.");
                _logger.LogInformation("Unknown Command {command}",command);
            }
        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            _logger.LogInformation("Unknown update type: {updateType}", update.Type);
            return Task.CompletedTask;
        }

        public Task HandleErrorAsync(Exception exception)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
