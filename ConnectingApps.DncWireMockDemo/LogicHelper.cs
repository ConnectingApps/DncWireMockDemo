using Microsoft.Extensions.Logging;

namespace ConnectingApps.DncWireMockDemo
{
    public class LogicHelper : ILogicHelper
    {
        public LogicHelper(ILogger<LogicHelper> logger)
        {
            logger.LogCritical("Log Debug LogicHelper");
        }
    }
}
