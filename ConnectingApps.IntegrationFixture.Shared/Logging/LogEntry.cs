using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ConnectingApps.IntegrationFixture.Logging
{
    public class LogEntry
    {
        private readonly dynamic _state;
        private readonly dynamic _formatter;

        public LogLevel LogLevel { get; }
        public EventId EventId { get; }
        public Exception Exception { get; }
        public IReadOnlyList<KeyValuePair<string, object>> LoggedObjects { get; }
        public string LogLine => _formatter(_state, Exception);

        internal LogEntry(LogLevel logLevel, EventId eventId, dynamic state, Exception exception, dynamic formatter)
        {
            LogLevel = logLevel;
            EventId = eventId;
            Exception = exception;
            _state = state;
            _formatter = formatter;
            LoggedObjects = state is IReadOnlyList<KeyValuePair<string, object>>
                ? state 
                : new List<KeyValuePair<string, object>>();
        }
    }
}
