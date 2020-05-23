using System.Collections.Concurrent;
using ConnectingApps.IntegrationFixture.Logging;
using Microsoft.Extensions.Logging;

namespace ConnectingApps.IntegrationFixture.Shared.Logging
{
    internal class TestLoggerProvider : ILoggerProvider
    {
        private readonly ILogSource _logSource;
        private readonly ConcurrentDictionary<string, TestLogger> _loggers = new ConcurrentDictionary<string, TestLogger>();
       
        public TestLoggerProvider(ILogSource logSource)
        {
            _logSource = logSource;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new TestLogger(_logSource));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
