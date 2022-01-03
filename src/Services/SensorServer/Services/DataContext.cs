using Dapper;
using Dapper.Contrib.Extensions;
using SensorServer.Models;
using SensorsServer.Models;

namespace SensorsServer.Services
{
    public class DataContext : IDataContext
    {
        private readonly ILogger<DataContext> _logger;

        string connectionString { get; set; }
        public DataContext(ILogger<DataContext> logger, IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("db");
            this._logger=logger;
        }

        /// <summary>
        /// add sensor type with name=prefix
        /// name can be updated outside -we dont care about it at the moment
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async Task<int> AddSensorType(string prefix)
        {
            _logger.LogInformation("DataContext->AddSensorType->prefix={prefix}", prefix);
            using var cn = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            cn.Open();
            return await cn.ExecuteAsync("INSERT INTO sensor_types(name, prefix) VALUES(@name,@prefix)", new { name = prefix, prefix });
        }

        /// <summary>
        /// updating sensor type name by prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<int> UpdateSensorType(string prefix, string name)
        {
            _logger.LogInformation("DataContext->UpdateSensorType->prefix={prefix} woth name={name}", prefix, name);
            using var cn = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            cn.Open();
            return await cn.ExecuteAsync("UPDATE sensor_types SET name=@name WHERE prefix=@prefix)", new { name, prefix });
        }

        /// <summary>
        /// get sensor type by prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async Task<int> GetSensorType(string prefix)
        {
            _logger.LogInformation("DataContext->GetSensorType for prefix={prefix}", prefix);
            using var cn = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            cn.Open();
            var sensorTypeId = await cn.ExecuteScalarAsync("SELECT id FROM sensor_types WHERE prefix=@prefix", new { prefix });
            if (sensorTypeId==null)
            {
                sensorTypeId=await AddSensorType(prefix);
            }
            return (int)sensorTypeId;
        }

        /// <summary>
        /// add new sensor
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<int> AddSensor(string prefix, string name, string description)
        {
            _logger.LogInformation("DataContext->AddSensor->prefix={prefix} name={name} description={description} ", prefix, name, description);
            using var cn = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            cn.Open();
            var sensorType = await GetSensorType(prefix);
            return await cn.ExecuteAsync("INSERT INTO sensors(type, name, description) VALUES(@type,@name,@description)", new { sensorType, name, description });
        }

        /// <summary>
        /// get sensor by name (but prefix will be used in case this is a new sensor type, we will add it and use it)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async Task<int> GetSensor(string name, string prefix)
        {
            _logger.LogInformation("DataContext->GetSensor for name={name} prefix={prefix}", name, prefix);
            using var cn = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            cn.Open();
            var sensorId = await cn.ExecuteScalarAsync("SELECT id FROM sensors WHERE name=@name", new { name });
            if (sensorId==null)
            {
                sensorId=await AddSensor(prefix, name, prefix);
            }
            return (int)sensorId;
        }

        /// <summary>
        /// get all awailable sensors with their types details
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SensorViewModel>> GetSensors()
        {
            _logger.LogInformation("DataContext->GetSensors");
            using var cn = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            cn.Open();
            return await cn.QueryAsync<SensorViewModel>("SELECT s.id, s.name, s.description, st.name as type, st.prefix FROM sensors s INNER JOIN sensor_types st ON st.id=s.type");
        }

        /// <summary>
        /// add sensor values
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<int> AddSensorValues(int sensorId, SensorValue[] values)
        {
            _logger.LogInformation("DataContext->AddSensorValues for {sensor_id}", sensorId);
            using var cn = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            cn.Open();

            // we don't care about bulk insert, because the data can be duplicated and don't need to be inserted
            var qty = 0;
            foreach (var value in values)
            {
                try
                {
                    await cn.ExecuteAsync("INSERT INTO sensors_values(sensor_id, value, scanned_at, added_at) VALUES(@id,@value,@scanned_at,@added_at)", new { id = sensorId, value = value.v, scanned_at = value.ts, added_at = DateTimeOffset.Now.ToUnixTimeSeconds() });
                    qty++;
                }
                catch { }
            }
            return qty;
        }
    }
}
