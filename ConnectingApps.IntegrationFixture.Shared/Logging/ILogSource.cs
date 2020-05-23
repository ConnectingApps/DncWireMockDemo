using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ConnectingApps.IntegrationFixture.Logging
{
    public interface ILogSource
    {
        ConcurrentBag<LogEntry> LogEntries { get; }
        IEnumerable<LogEntry> GetWarnings();
        IEnumerable<LogEntry> GetErrors();
        IEnumerable<LogEntry> GetCriticals();
        IEnumerable<LogEntry> GetDebugs();
        IEnumerable<LogEntry> GetInformations();
        IEnumerable<LogEntry> GetNones();
        IEnumerable<LogEntry> GetTraces();
        IReadOnlyList<KeyValuePair<string, object>> GetLoggedObjects();
        IEnumerable<KeyValuePair<string, T>> GetLoggedObjects<T>() where T : class;
        IEnumerable<string> GetLogLines();
        IEnumerable<Exception> GetExceptions();
    }
}