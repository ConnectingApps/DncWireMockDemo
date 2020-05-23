﻿using System;
using Microsoft.Extensions.Logging;

namespace ConnectingApps.IntegrationFixture.Logging
{
    public class TestLogger : ILogger
    {
        private readonly ILogSource _logSource;

        public TestLogger(ILogSource logSource)
        {
            _logSource = logSource;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var logEntry = new LogEntry(logLevel,eventId, state, exception , formatter);
            _logSource.LogEntries.Add(logEntry);
        }
    }
}
