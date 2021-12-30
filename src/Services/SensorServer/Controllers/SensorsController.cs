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
        private readonly ILogger<SensorsController> _logger;

        public SensorsController(ISensorsService sensorsService, ILogger<SensorsController> logger)
        {
            this.sensorsService=sensorsService;
            this._logger=logger;
        }
        [HttpGet]
        public ActionResult Index()
        {
            _logger.LogInformation("sensors->index");
            return Ok("worked");
        }

        [HttpPost("{sensorId}")]

        public async Task<IActionResult> StoreData(string sensorId)
        {
            _logger.LogInformation("start storing data for sensor->{sensor_id}", sensorId);
            try
            {
                using (var stream = Request.Body)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string body = await reader.ReadToEndAsync();
                        _logger.LogInformation("{sensor_id} body={body}",sensorId, body);
                        var data = System.Text.Json.JsonSerializer.Deserialize<SensorTemperature[]>(body);
                        if (data!=null)
                        {
                            await sensorsService.StoreSensorData(sensorId, data);
                        }
                        else
                        {
                            _logger.LogInformation("{sensor_id} NO DATA", sensorId);
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{sensor_id} {error_message}", sensorId, ex.Message);
            }
            return Ok();
        }
    }
}
