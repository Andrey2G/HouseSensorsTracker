namespace SensorsServer.Models
{
    public class SensorModel
    {
        public int id { get; set;}
        public string name { get; set; } = "";
        public string description { get; set; } = "";
    }

    public class SensorViewModel
    {
        public int id { get; set; }
        public string name { get; set; } = "";
        public string type { get; set; } = "";
        public string prefix { get; set; } = "";
        public string description { get; set; } = "";
    }
}
