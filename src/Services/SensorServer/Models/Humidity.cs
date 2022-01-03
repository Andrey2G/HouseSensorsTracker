using SensorServer.Models;

namespace SensorsServer.Models
{
    public class HumidityModel
    {
        public SensorValue[]? temperature { get; set; }
        public SensorValue[]? humidity { get; set; }
    }
}
