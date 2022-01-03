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
        Task StoreBarometerData(BarometerModel data);
        Task<string> GetCurrentHeatpointHumidity();
        Task<string> GetCurrentHeatpointPressure();
        Task<Dictionary<SensorViewModel, SensorValue[]?>> GetLastHourTemperature();
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
                if (data==null)
                {
                    _logger.LogInformation("SensorService->StoreSensorData=>{sensor_id} NO DATA", sensorId);
                    return;
                }
                if (data.Where(d => d.v!=null).FirstOrDefault()==null)
                {
                    _logger.LogWarning("SensorService->StoreSensorData=>{sensor_id} DATA EXISTS BUT ALL EMPTY", sensorId);
                    return;
                }
                await _redis.GetDatabase().SetAddAsync("sensors", sensorId);
                //TODO: prefix should be a constant
                var id = await _dataContext.GetSensor(sensorId, sensorId.Substring(0, 3));
                await _dataContext.AddSensorValues(id, data);

                var lastNonemptyTimestamp = data.Where(d => d.v!=null).Select(d => d.ts).Max();
                await _redis.GetDatabase().StringSetAsync($"{sensorId}:last_value_ts", lastNonemptyTimestamp);
                var lastNonEmptyValue = data.Where(d => d.ts==lastNonemptyTimestamp).Select(d => d.v).FirstOrDefault();
                if (lastNonEmptyValue!=null)
                    await _redis.GetDatabase().StringSetAsync($"{sensorId}:last_value", lastNonEmptyValue);
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
                await _redis.GetDatabase().SetAddAsync("sensors", "dht11-h");
                //TODO: prefix should be a constant
                var id = await _dataContext.GetSensor("dht11-h", "dht11-");
                if (data.humidity!=null)
                {
                    if (data.humidity.Where(d => d.v!=null).FirstOrDefault()==null)
                    {
                        _logger.LogWarning("SensorService->StoreHumidityData=>Humidity DATA EXISTS BUT ALL EMPTY");
                    }
                    else
                    {
                        await _dataContext.AddSensorValues(id, data.humidity);
                        var lastNonemptyTimestamp = data.humidity.Where(d => d.v!=null).Select(d => d.ts).Max();
                        await _redis.GetDatabase().StringSetAsync($"dht11-h:last_value_ts", lastNonemptyTimestamp);
                        var lastNonEmptyValue = data.humidity.Where(d => d.ts==lastNonemptyTimestamp).Select(d => d.v).FirstOrDefault();
                        await _redis.GetDatabase().StringSetAsync($"dht11-h:last_value", lastNonEmptyValue);
                    }
                }

                await _redis.GetDatabase().SetAddAsync("sensors", "dht11-t");
                //TODO: prefix should be a constant
                id = await _dataContext.GetSensor("dht11-t", "dht11-");
                if (data.temperature!=null)
                {
                    if (data.temperature.Where(d => d.v!=null).FirstOrDefault()==null)
                    {
                        _logger.LogWarning("SensorService->StoreHumidityData=>Temperature DATA EXISTS BUT ALL EMPTY");
                    }
                    else
                    {
                        await _dataContext.AddSensorValues(id, data.temperature);
                        var lastNonemptyTimestamp = data.temperature.Where(d => d.v!=null).Select(d => d.ts).Max();
                        await _redis.GetDatabase().StringSetAsync($"dht11-t:last_value_ts", lastNonemptyTimestamp);
                        var lastNonEmptyValue = data.temperature.Where(d => d.ts==lastNonemptyTimestamp).Select(d => d.v).FirstOrDefault();
                        await _redis.GetDatabase().StringSetAsync($"dht11-t:last_value", lastNonEmptyValue);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->StoreHumidityData FAILED");
            }
        }

        public async Task StoreBarometerData(BarometerModel data)
        {
            _logger.LogInformation("SensorService->StoreBarometerData=>{@barometer_data}", data);
            try
            {
                await _redis.GetDatabase().SetAddAsync("sensors", "bmp180-p");
                //TODO: prefix should be a constant
                var id = await _dataContext.GetSensor("bmp180-p", "bmp180-");
                if (data.pressure!=null)
                {
                    if (data.pressure.Where(d => d.v!=null).FirstOrDefault()==null)
                    {
                        _logger.LogWarning("SensorService->StoreBarometerData=>Pressure DATA EXISTS BUT ALL EMPTY");
                    }
                    else
                    {
                        await _dataContext.AddSensorValues(id, data.pressure);
                        var lastNonemptyTimestamp = data.pressure.Where(d => d.v!=null).Select(d => d.ts).Max();
                        await _redis.GetDatabase().StringSetAsync($"bmp180-p:last_value_ts", lastNonemptyTimestamp);
                        var lastNonEmptyValue = data.pressure.Where(d => d.ts==lastNonemptyTimestamp).Select(d => d.v).FirstOrDefault();
                        await _redis.GetDatabase().StringSetAsync($"bmp180-p:last_value", lastNonEmptyValue);
                    }
                }

                await _redis.GetDatabase().SetAddAsync("sensors", "bmp180-t");
                //TODO: prefix should be a constant
                id = await _dataContext.GetSensor("bmp180-t", "bmp180-");
                if (data.temperature!=null)
                {
                    if (data.temperature.Where(d => d.v!=null).FirstOrDefault()==null)
                    {
                        _logger.LogWarning("SensorService->StoreBarometerData=>Temperature DATA EXISTS BUT ALL EMPTY");
                    }
                    else
                    {
                        await _dataContext.AddSensorValues(id, data.temperature);
                        var lastNonemptyTimestamp = data.temperature.Where(d => d.v!=null).Select(d => d.ts).Max();
                        await _redis.GetDatabase().StringSetAsync($"bmp180-t:last_value_ts", lastNonemptyTimestamp);
                        var lastNonEmptyValue = data.temperature.Where(d => d.ts==lastNonemptyTimestamp).Select(d => d.v).FirstOrDefault();
                        await _redis.GetDatabase().StringSetAsync($"bmp180-t:last_value", lastNonEmptyValue);
                    }
                }

                await _redis.GetDatabase().SetAddAsync("sensors", "bmp180-a");
                //TODO: prefix should be a constant
                id = await _dataContext.GetSensor("bmp180-a", "bmp180-");
                if (data.altitude!=null)
                {
                    if (data.altitude.Where(d => d.v!=null).FirstOrDefault()==null)
                    {
                        _logger.LogWarning("SensorService->StoreBarometerData=>Altitude DATA EXISTS BUT ALL EMPTY");
                    }
                    else
                    {
                        await _dataContext.AddSensorValues(id, data.altitude);
                        var lastNonemptyTimestamp = data.altitude.Where(d => d.v!=null).Select(d => d.ts).Max();
                        await _redis.GetDatabase().StringSetAsync($"bmp180-a:last_value_ts", lastNonemptyTimestamp);
                        var lastNonEmptyValue = data.altitude.Where(d => d.ts==lastNonemptyTimestamp).Select(d => d.v).FirstOrDefault();
                        await _redis.GetDatabase().StringSetAsync($"bmp180-a:last_value", lastNonEmptyValue);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->StoreBarometerData FAILED");
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
                    var timestamp = await _redis.GetDatabase().StringGetAsync($"{sensor}:last_value_ts");
                    _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature->{sensor} last key at {sensor_timestamp}", sensor, timestamp);
                    if (string.IsNullOrEmpty(timestamp)) continue;
                    var lastValue = await _redis.GetDatabase().StringGetAsync($"{sensor}:last_value");
                    _logger.LogInformation("SensorService->GetCurrentHeatpointTemperature->{sensor} last key at {sensor_timestamp} with value={value}", sensor, timestamp, lastValue);

                    result.AppendLine($"{s.description}: {double.Parse(lastValue).ToString("F1")}°C at {DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestamp)).ToLocalTime().ToString("dd.MM.yyyy HH:mm")}");
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
                        
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->GetCurrentHeatpointTemperature FAILED");
            }
            return "NO DATA";
        }

        public async Task<string> GetCurrentHeatpointHumidity()
        {
            _logger.LogInformation("SensorService->GetCurrentHeatpointHumidity");
            StringBuilder result = new StringBuilder();
            try
            {
                var timestamp =  await _redis.GetDatabase().StringGetAsync($"dht11-h:last_value_ts");
                _logger.LogInformation("SensorService->GetCurrentHeatpointHumidity->last key at {sensor_timestamp}", timestamp);

                var lastValue = await _redis.GetDatabase().StringGetAsync($"dht11-h:last_value");
                _logger.LogInformation("SensorService->GetCurrentHeatpointHumidity->last value {value} at {sensor_timestamp}", lastValue, timestamp);

                result.AppendLine($"Humidity: {double.Parse(lastValue).ToString("F1")}% at {DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestamp)).ToLocalTime().ToString("dd.MM.yyyy HH:mm")}");
                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->GetCurrentHeatpointHumidity FAILED");
            }
            return "NO DATA";
        }

        public async Task<string> GetCurrentHeatpointPressure()
        {
            _logger.LogInformation("SensorService->GetCurrentHeatpointPressure");
            StringBuilder result = new StringBuilder();
            try
            {
                var timestamp = await _redis.GetDatabase().StringGetAsync($"bmp180-p:last_value_ts");
                _logger.LogInformation("SensorService->GetCurrentHeatpointPressure->last key at {sensor_timestamp}", timestamp);

                var lastValue = await _redis.GetDatabase().StringGetAsync($"bmp180-p:last_value");
                _logger.LogInformation("SensorService->GetCurrentHeatpointPressure->last value {value} at {sensor_timestamp}", lastValue, timestamp);

                result.AppendLine($"Pressure: {double.Parse(lastValue).ToString("F2")}hPa at {DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestamp)).ToLocalTime().ToString("dd.MM.yyyy HH:mm")}");
                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->GetCurrentHeatpointPressure FAILED");
            }
            return "NO DATA";
        }

        public async Task<Dictionary<SensorViewModel,SensorValue[]?>> GetLastHourTemperature()
        {
            _logger.LogInformation("SensorService->GetLastHourTemperature");
            var result = new Dictionary<SensorViewModel, SensorValue[]?>();
            try
            {
                var temperatureSensors = await GetTemperatureSensors();
                if (temperatureSensors!=null)
                {
                    var values = await _dataContext.GetSensorsValues(temperatureSensors.Select(s => s.id).ToArray(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()-60*60);
                    foreach(var key in values.Keys)
                    {
                        var k = temperatureSensors.Where(s => s.id==key).FirstOrDefault();
                        if (k!=null)
                        {
                            result[k]=values[key];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SensorService->GetLastHourTemperature FAILED");
            }
            return result;
        }
    }
}
