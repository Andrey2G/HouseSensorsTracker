namespace SensorsServer.Services
{
    public class ChartService
    {
        private readonly ILogger<ChartService> _logger;

        public ChartService(ILogger<ChartService> logger)
        {
            this._logger=logger;
        }

    }
}
