using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ConnectingApps.IntegrationFixture.Logging
{
    public class LogEntry
    {
        private readonly dynamic _state;
        private readonly Lazy<string> _logLineCreator;

        public LogLevel LogLevel { get; }
        public EventId EventId { get; }
        public Exception Exception { get; }
        public IReadOnlyList<KeyValuePair<string, object>> LoggedObjects { get; }
        public string LogLine => _logLineCreator.Value;

        internal LogEntry(LogLevel logLevel, EventId eventId, dynamic state, Exception exception, Lazy<string> logLineCreator)
        {
            LogLevel = logLevel;
            EventId = eventId;
            Exception = exception;
            _state = state;
            _logLineCreator = logLineCreator;
            LoggedObjects = state is IReadOnlyList<KeyValuePair<string, object>>
                ? state 
                : new List<KeyValuePair<string, object>>();
        }
    }
}
