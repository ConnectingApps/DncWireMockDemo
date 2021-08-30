using Microsoft.Extensions.Logging;

namespace ConnectingApps.Dnc50Demo
{
    public class LogicHelper : ILogicHelper
    {
        public LogicHelper(ILogger<LogicHelper> logger)
        {
            logger.LogCritical("Log Debug LogicHelper");
        }
    }
}
