using Microsoft.AspNetCore.Mvc;
using SensorServer.Services;
using Telegram.Bot.Types;

namespace SensorServer.Controllers
{
    public class BotController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices] HandleTelegramUpdateService handleTelegram,
                                          [FromBody] Update update)
        {
            await handleTelegram.EchoAsync(update);
            return Ok();
        }
    }
}
