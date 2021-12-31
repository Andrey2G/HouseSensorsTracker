using Microsoft.AspNetCore.Mvc;
using SensorServer.Models;
using SensorsServer.Services;

namespace SensorServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly IDataContext dataContext;
        private readonly ILogger<SensorsController> _logger;

        public SensorsController(IDataContext dataContext, ILogger<SensorsController> logger)
        {
            this.dataContext=dataContext;
            this._logger=logger;
        }
        [HttpGet]
        public ActionResult Index()
        {
            _logger.LogInformation("sensors->index");
            return Ok("worked");
        }

        [HttpPost("{sensorId}")]

        public async Task<IActionResult> StoreTemperatureData(string sensorId)
        {
            _logger.LogInformation("start storing data for temperature sensor->{sensor_id}", sensorId);
            try
            {
                using (var stream = Request.Body)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string body = await reader.ReadToEndAsync();
                        _logger.LogInformation("{sensor_id} body={body}",sensorId, body);
                        var data = System.Text.Json.JsonSerializer.Deserialize<SensorValue[]>(body);
                        if (data!=null)
                        {
                            //TODO: prefix should be a constant
                            var id = await dataContext.GetSensor(sensorId, sensorId.Substring(0, 3));
                            await dataContext.AddSensorValues(id, data);
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
