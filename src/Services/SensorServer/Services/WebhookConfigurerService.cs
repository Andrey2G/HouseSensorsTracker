using SensorServer.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace SensorServer.Services
{
    public class WebhookConfigurerService:IHostedService
    {
        private readonly ILogger<WebhookConfigurerService> _logger;
        private readonly IServiceProvider _services;
        private readonly TelegramConfiguration _telegramConfiguration;

        public WebhookConfigurerService(ILogger<WebhookConfigurerService> logger,  IConfiguration configuration, IServiceProvider services)
        {
            this._logger=logger;
            this._services=services;
            _telegramConfiguration=configuration.GetSection("TelegramConfiguration").Get<TelegramConfiguration>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Configure custom endpoint
            // see https://core.telegram.org/bots/api#setwebhook
            var webhookUrl = @$"{_telegramConfiguration.webhook_url}/bot/{_telegramConfiguration.token}";
            try
            {
                _logger.LogInformation("StartAsync: remove webhook");
                await botClient.DeleteWebhook(cancellationToken: cancellationToken);
            }
            catch (Exception ex) { _logger.LogError(ex, "StartAsync: can't remove webhook"); }
            try
            {
                _logger.LogInformation("setup webhook: {webhookAddress}", webhookUrl);
                await botClient.SetWebhook(url: webhookUrl, allowedUpdates: Array.Empty<UpdateType>(), dropPendingUpdates: true, cancellationToken: cancellationToken);
            }
            catch (Exception ex) { _logger.LogError(ex, "StartAsync: can't setup webhook"); throw; }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Remove webhook when service shutdown
            try
            {
                _logger.LogInformation("StopAsync: remove webhook");
                await botClient.DeleteWebhook(cancellationToken: cancellationToken);
            }
            catch (Exception ex) { _logger.LogError(ex, "StopAsync: can't remove webhook"); }
            //botClient.SendTextMessageAsync()
            await Task.CompletedTask;
        }
    }
}
