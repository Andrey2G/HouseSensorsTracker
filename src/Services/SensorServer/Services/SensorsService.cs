using SensorServer.Models;
using SensorsServer.Models;
using SensorsServer.Services;
using StackExchange.Redis;
using System.Text;

namespace SensorServer.Services
{
    public interface ISensorsService
    {
        Task StoreSensorData(string sensorId, SensorValue[] data);
        Task<string> GetCurrentHeatpointTemperature();
        Task<string> GetLastHourHeatpointTemperature();
        Task StoreHumidityData(HumidityModel data);
    }

    public class SensorsService : ISensorsService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDataContext _dataContext;
        private readonly ILogger<SensorsService> _logger;

        public SensorsService(ConnectionMultiplexer redis, IDataContext dataContext, ILogger<SensorsService> logger)
        {
            this._redis=redis;
            this._dataContext=dataContext;
            this._logger=logger;
        }

        public async Task StoreSensorData(string sensorId, SensorValue[] data)
        {
            _logger.LogInformation("SensorService->StoreSensorData=>{sensor_id} {@data}", sensorId, data);
            try
            {
                await _redis.GetDatabase().SetAddAsync("sensors", sensorId);
                //TODO: prefix should be a constant
                var id = await _dataContext.GetSensor(sensorId, sensorId.Substring(0, 3));
                await _dataContext.AddSensorValues(id, data);

                await _redis.GetDatabase().StringSetAsync($"{sensorId}:last_value", data.Where(d => d.v!=null).Select(d => d.ts).Max());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->StoreSensorData=>{sensor_id} FAILED", sensorId);
            }
        }

        public async Task StoreHumidityData(HumidityModel data)
        {
            _logger.LogInformation("SensorService->StoreSensorData=>{@humidity_data}", data);
            try
            {
                await _redis.GetDatabase().SetAddAsync("sensors", "dht11-t");
                //TODO: prefix should be a constant
                var id = await _dataContext.GetSensor("dht11-t", "dht11-");
                if (data.humidity!=null)
                {
                    await _dataContext.AddSensorValues(id, data.humidity);
                    await _redis.GetDatabase().StringSetAsync($"dht11-t:last_value", data.humidity.Where(d => d.v!=null).Select(d => d.ts).Max());
                }

                await _redis.GetDatabase().SetAddAsync("sensors", "dht11-h");
                //TODO: prefix should be a constant
                id = await _dataContext.GetSensor("dht11-h", "dht11-");
                if (data.temperature!=null)
                {
                    await _dataContext.AddSensorValues(id, data.temperature);
                    await _redis.GetDatabase().StringSetAsync($"dht11-t:last_value", data.temperature.Where(d => d.v!=null).Select(d => d.ts).Max());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->StoreHumidityData FAILED");
            }
        }

        /// <summary>
        /// get the list of all available temperature sensors
        /// </summary>
        /// <returns></returns>
        async Task<SensorViewModel[]?> GetTemperatureSensors()
        {
            _logger.LogInformation("SensorService->GetTemperatureSensors");
            try
            {
                return (await _dataContext.GetSensors()).Where(d=>d.prefix=="28-").ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->GetTemperatureSensors FAILED");
            }
            return null;
        }

        public async Task<string> GetCurrentHeatpointTemperature()
        {
            _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature");
            StringBuilder result = new StringBuilder();
            try
            {
                var sensors = await GetTemperatureSensors();
                if (sensors!=null)
                foreach (var s in sensors)
                {
                    var sensor = s.name;
                    _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature->next sensor {sensor}",sensor);
                    var lastKey = await _redis.GetDatabase().StringGetAsync($"{sensor}:last_value");
                    _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature->{sensor} last key at {sensor_timestamp}", sensor, lastKey);
                    if (string.IsNullOrEmpty(lastKey)) continue;
                    var lastValue = await _redis.GetDatabase().HashGetAsync($"{sensor}", lastKey);
                    _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature->{sensor} last key at {sensor_timestamp} with value={value}", sensor, lastKey, lastValue);

                    result.AppendLine($"{s.description}: {double.Parse(lastValue).ToString("F1")}°C at {DateTimeOffset.FromUnixTimeSeconds(long.Parse(lastKey)).ToString("dd.MM.yyyy HH:mm")}");
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->GetCurrentHeatpointTemperature FAILED");
            }
            return "NO DATA";
        }

        public async Task<string> GetLastHourHeatpointTemperature()
        {
            _logger.LogInformation("SensorService->GetLastHourHeatpointTemperature");
            StringBuilder result = new StringBuilder();
            try
            {
                var sensors = await GetTemperatureSensors();
                if (sensors!=null)
                    foreach (var s in sensors)
                {
                        var sensor = s.name;
                        _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature->next sensor {sensor}", sensor);
                    var lastKey = await _redis.GetDatabase().StringGetAsync($"{sensor}:last_value");
                    _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature->{sensor} last key at {sensor_timestamp}", sensor, lastKey);
                    if (string.IsNullOrEmpty(lastKey)) continue;
                    var lastValue = await _redis.GetDatabase().HashGetAsync($"{sensor}", lastKey);
                    _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature->{sensor} last key at {sensor_timestamp} with value={value}", sensor, lastKey, lastValue);

                    result.AppendLine($"{s.description}: {double.Parse(lastValue).ToString("F1")}°C at {DateTimeOffset.FromUnixTimeSeconds(long.Parse(lastKey)).ToString("dd.MM.yyyy HH:mm")}");
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->GetCurrentHeatpointTemperature FAILED");
            }
            return "NO DATA";
        }
    }
}
