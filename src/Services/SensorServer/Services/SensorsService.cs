using SensorServer.Models;
using StackExchange.Redis;
using System.Text;

namespace SensorServer.Services
{
    public interface ISensorsService
    {
        Task StoreSensorData(string sensorId, SensorTemperature[] data);
        Task<string> GetCurrentHeatpointTemperature();
    }

    public class SensorsService : ISensorsService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly ILogger<SensorsService> _logger;

        public SensorsService(ConnectionMultiplexer redis, ILogger<SensorsService> logger)
        {
            this._redis=redis;
            this._logger=logger;
        }

        public async Task StoreSensorData(string sensorId, SensorTemperature[] data)
        {
            _logger.LogInformation("SensorService->StoreSensorData=>{sensor_id} {@data}", sensorId, data);
            try
            {
                await _redis.GetDatabase().SetAddAsync("sensors", sensorId);
                await _redis.GetDatabase().HashSetAsync(
                    $"{sensorId}",
                    data.Where(d=>d.t!=null).Select(d => new HashEntry(d.ts, d.t)).ToArray());

                await _redis.GetDatabase().StringSetAsync($"{sensorId}:last_value", data.Where(d => d.t!=null).Select(d => d.ts).Max());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "SensorService->StoreSensorData=>{sensor_id} FAILED", sensorId);
            }
        }

        /// <summary>
        /// get the list of all available temperature sensors
        /// </summary>
        /// <returns></returns>
        async Task<string[]?> GetTemperatureSensors()
        {
            _logger.LogInformation("SensorService->GetTemperatureSensors");
            try
            {
                return (await _redis.GetDatabase().SetMembersAsync("sensors")).Where(v=>v.StartsWith("28-")).Select(v => (string)v).ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->GetTemperatureSensors FAILED");
            }
            return null;
        }

        async Task<Dictionary<string,string>> GetTemperatureSensorsWithNames()
        {
            _logger.LogInformation("SensorService->GetTemperatureSensorsWithNames");
            var result=new Dictionary<string,string>();
            try
            {
                var sensors = await GetTemperatureSensors();
                if (sensors==null)
                {
                    _logger.LogInformation("SensorService->GetTemperatureSensorsWithNames->No Temperarure Sensors available");
                    return result;
                }
                _logger.LogInformation("SensorService->GetTemperatureSensorsWithNames->{@sensors}",sensors);
                var sensorsNames = await _redis.GetDatabase().HashGetAsync($"sensors:names", sensors.Select(s=>(RedisValue)s).ToArray());
                for (int i=0;i<sensors.Length;i++)
                {
                    result[sensors[i]]=sensorsNames[i];
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->GetTemperatureSensors FAILED");
            }
            _logger.LogInformation("SensorService->GetTemperatureSensorsWithNames->{@sensors_names}", result);
            return result;
        }

        public async Task<string> GetCurrentHeatpointTemperature()
        {
            _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature");
            StringBuilder result = new StringBuilder();
            try
            {
                var sensors = await GetTemperatureSensorsWithNames();
                foreach (var sensor in sensors.Keys)
                {
                    _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature->next sensor {sensor}",sensor);
                    var lastKey = await _redis.GetDatabase().StringGetAsync($"{sensor}:last_value");
                    _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature->{sensor} last key at {sensor_timestamp}", sensor, lastKey);
                    if (string.IsNullOrEmpty(lastKey)) continue;
                    var lastValue = await _redis.GetDatabase().HashGetAsync($"{sensor}", lastKey);
                    _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature->{sensor} last key at {sensor_timestamp} with value={value}", sensor, lastKey, lastValue);

                    result.AppendLine($"{sensors[sensor]}: {double.Parse(lastValue).ToString("F1")}°C at {DateTimeOffset.FromUnixTimeSeconds(long.Parse(lastKey)).ToString("dd.MM.yyyy HH:mm")}");
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
