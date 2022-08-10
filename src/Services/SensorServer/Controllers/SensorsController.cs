using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SensorServer.Models;
using SensorServer.Services;
using SensorsServer.Models;

namespace SensorServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ISensorsService _sensorsService;
        private readonly ILogger<SensorsController> _logger;

        public SensorsController(ISensorsService sensorsService, ILogger<SensorsController> logger)
        {
            this._sensorsService=sensorsService;
            this._logger=logger;
        }
        [HttpGet]
        public ActionResult Index()
        {
            _logger.LogInformation("sensors->index");
            return Ok("worked");
        }

        [HttpGet]
        [Route("temperatures/{interval}")]
        public async Task<ActionResult> Temperatures(int interval)
        {
            _logger.LogInformation("sensors->temperature-last-hour");
            var values = await _sensorsService.GetTemperatures(interval);
            var rows = new List<object>();

            var row = new List<object>();
            row.Add("time");
            foreach (var k in values.Keys)
                row.Add(k.description);
            rows.Add(row);

            var firstKey = values.Keys.FirstOrDefault();
            if (firstKey!=null)
            {
                var firstKeyValues = values[firstKey];
                if (firstKeyValues!=null)
                    foreach (var value in firstKeyValues)
                    {
                        var ts = value.ts;
                        row = new List<object>();
                        row.Add(DateTimeOffset.FromUnixTimeSeconds(ts).ToLocalTime().ToString("HH:mm"));
                        foreach (var c in values.Keys)
                        {
                            if (c.id!=firstKey.id)
                            {
                                //find nearest timestamp
                                var v = values[c]?.Where(v => v.ts>=ts-30 && v.ts<=ts+30).FirstOrDefault();
                                row.Add(v?.v);
                            }
                            else row.Add(value.v);
                        }
                        rows.Add(row);
                    }
            }
            return new JsonResult(rows);
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
                        var data = System.Text.Json.JsonSerializer.Deserialize<SensorValue[]>(body);
                        if (data!=null)
                        {
                            await _sensorsService.StoreSensorData(sensorId, data);
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

        [HttpPost]
        [Route("humiture")]
        public async Task<IActionResult> Humiture()
        {
            _logger.LogInformation("start storing data for Humiture sensor");
            try
            {
                using (var stream = Request.Body)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string body = await reader.ReadToEndAsync();
                        _logger.LogInformation("Humidity body={body}", body);
                        var data = System.Text.Json.JsonSerializer.Deserialize<HumidityModel>(body);
                        if (data!=null)
                        {
                            await _sensorsService.StoreHumidityData(data);
                        }
                        else
                        {
                            _logger.LogInformation("Humiture NO DATA");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Humiture {error_message}", ex.Message);
            }
            return Ok();
        }

        [HttpPost]
        [Route("barometer")]
        public async Task<IActionResult> Barometer()
        {
            _logger.LogInformation("start storing data for Barometer sensor");
            try
            {
                using (var stream = Request.Body)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string body = await reader.ReadToEndAsync();
                        _logger.LogInformation("Barometer body={body}", body);
                        var data = System.Text.Json.JsonSerializer.Deserialize<BarometerModel>(body);
                        if (data!=null)
                        {
                            await _sensorsService.StoreBarometerData(data);
                        }
                        else
                        {
                            _logger.LogInformation("Barometer NO DATA");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Barometer {error_message}", ex.Message);
            }
            return Ok();
        }

        [HttpPost("graph/{filename}")]

        public async Task<IActionResult> StoreGraph(string filename)
        {
            _logger.LogInformation("start storing graph {filename}", filename);
            try
            {
                if (Request.Form.Files!=null)
                    foreach (var file in Request.Form.Files)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        _logger.LogInformation("start storing graph-> get filename {filename}", fileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/images", fileName);
                        _logger.LogInformation("start storing graph-> get file path {filepath}", filePath);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StoreGraph {filename} failed", filename);
            }
            return Ok();
        }
    }
}
