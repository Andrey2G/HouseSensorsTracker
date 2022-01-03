using SensorServer.Models;

namespace SensorsServer.Models
{
    public class BarometerModel
    {
        public SensorValue[]? temperature { get; set; }
        public SensorValue[]? pressure { get; set; }
        public SensorValue[]? altitude { get; set; }
    }
}
