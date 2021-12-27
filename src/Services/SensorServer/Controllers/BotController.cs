using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SensorServer.Services;
using Telegram.Bot.Types;

namespace SensorServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        [HttpPost]
        [Route("tg")]
        public async Task<IActionResult> Post([FromServices] HandleTelegramUpdateService handleTelegram,
                                          [FromBody] Update update)
        {
            await handleTelegram.EchoAsync(update);
            return Ok();
        }
    }
}
