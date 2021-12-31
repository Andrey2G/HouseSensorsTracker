using SensorServer.Models;

namespace SensorsServer.Services
{
    public interface IDataContext
    {
        Task<int> AddSensor(string prefix, string name, string description);
        Task<int> AddSensorType(string prefix);
        Task<int> AddSensorValues(int sensorId, SensorValue[] values);
        Task<int> GetSensor(string name, string prefix);
        Task<int> GetSensorType(string prefix);
        Task<int> UpdateSensorType(string prefix, string name);
    }
}