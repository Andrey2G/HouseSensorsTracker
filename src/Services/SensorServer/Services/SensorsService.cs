using SensorServer.Models;
using StackExchange.Redis;

namespace SensorServer.Services
{
    public interface ISensorsService
    {
        Task StoreSensorData(string sensorId, SensorTemperature[] data);
    }

    public class SensorsService : ISensorsService
    {
        private readonly ConnectionMultiplexer redis;
        private readonly ILogger<SensorsService> logger;

        public SensorsService(ConnectionMultiplexer redis, ILogger<SensorsService> logger)
        {
            this.redis=redis;
            this.logger=logger;
        }

        public async Task StoreSensorData(string sensorId, SensorTemperature[] data)
        {
            try
            {
                await redis.GetDatabase(1).HashSetAsync(
                    $"{sensorId}",
                    data.Where(d=>d.t!=null).Select(d => new HashEntry(d.ts, d.t)).ToArray());
            }
            catch(Exception ex)
            {
                await System.IO.File.AppendAllTextAsync(@"c:\temp\sensors.log", $"{DateTime.UtcNow} {sensorId} {ex.Message}\r\n{ex.StackTrace}\r\n");
            }

        }
    }
}
