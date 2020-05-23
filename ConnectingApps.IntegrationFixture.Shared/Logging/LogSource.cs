using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectingApps.IntegrationFixture.Logging;
using Microsoft.Extensions.Logging;

namespace ConnectingApps.IntegrationFixture.Shared.Logging
{
    public class LogSource : ILogSource
    {
        public ConcurrentBag<LogEntry> LogEntries { get; } = new ConcurrentBag<LogEntry>();

        public IEnumerable<LogEntry> GetWarnings() => LogEntries.Where(l => l.LogLevel == LogLevel.Warning);

        public IEnumerable<LogEntry> GetErrors() => LogEntries.Where(l => l.LogLevel == LogLevel.Error);

        public IEnumerable<LogEntry> GetCriticals() => LogEntries.Where(l => l.LogLevel == LogLevel.Critical);

        public IEnumerable<LogEntry> GetDebugs() => LogEntries.Where(l => l.LogLevel == LogLevel.Debug);

        public IEnumerable<LogEntry> GetInformations() => LogEntries.Where(l => l.LogLevel == LogLevel.Information);

        public IEnumerable<LogEntry> GetNones() => LogEntries.Where(l => l.LogLevel == LogLevel.None);

        public IEnumerable<LogEntry> GetTraces() => LogEntries.Where(l => l.LogLevel == LogLevel.Trace);

        public IReadOnlyList<KeyValuePair<string, object>> GetLoggedObjects() => LogEntries.SelectMany(a => a.LoggedObjects).ToList();

        public IEnumerable<string> GetLogLines() => LogEntries.Select(l => l.LogLine);

        public IEnumerable<KeyValuePair<string, T>> GetLoggedObjects<T>() where T : class
        {
            return GetLoggedObjects()
                .Where(l => l.Value is T)
                .Select(l => new KeyValuePair<string, T>(l.Key, l.Value as T));
        }
    }
}
