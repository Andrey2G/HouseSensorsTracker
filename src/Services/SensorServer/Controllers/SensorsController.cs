using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SensorServer.Models;
using SensorServer.Services;

namespace SensorServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ISensorsService sensorsService;
        private readonly ILogger<SensorsController> logger;

        public SensorsController(ISensorsService sensorsService, ILogger<SensorsController> logger)
        {
            this.sensorsService=sensorsService;
            this.logger=logger;
        }
        [HttpGet]
        public ActionResult Index()
        {
            return Ok("worked");
        }

        //[HttpPost]
        //[Route("{**catchAll}")]
        //public async Task <IActionResult> ProcessPostMethod(string catchAll)
        //{
        //    logger.LogInformation(catchAll);
        //    await System.IO.File.AppendAllTextAsync(@"c:\temp\sensors.log", $"{DateTime.UtcNow} {catchAll}\r\n");
        //    return Ok("Catched!");
        //}

        [HttpPost("{sensorId}")]

        public async Task<IActionResult> StoreData(string sensorId)
        {
            try
            {
                using (var stream = Request.Body)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string body = await reader.ReadToEndAsync();
                        await System.IO.File.AppendAllTextAsync(@"c:\temp\sensors.log", $"{DateTime.UtcNow} {sensorId} body={body}\r\n");
                        var data = System.Text.Json.JsonSerializer.Deserialize<SensorTemperature[]>(body);
                        if (data!=null)
                        {
                            await System.IO.File.AppendAllTextAsync(@"c:\temp\sensors.log", $"{DateTime.UtcNow} {sensorId} {System.Text.Json.JsonSerializer.Serialize(data)}\r\n");
                            await sensorsService.StoreSensorData(sensorId, data);
                        }
                        else
                        {
                            await System.IO.File.AppendAllTextAsync(@"c:\temp\sensors.log", $"{DateTime.UtcNow} {sensorId} NO DATA\r\n");
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                await System.IO.File.AppendAllTextAsync(@"c:\temp\sensors.log", $"{DateTime.UtcNow} {sensorId} {ex.Message}\r\n{ex.StackTrace}\r\n");
            }
            return Ok();
        }
    }
}
