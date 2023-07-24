using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace GenericHost
{
    internal class MyLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new MyLogger(categoryName);
        }

        public void Dispose()
        {

        }

    }
    internal class MyLogger : ILogger
    {
        private IExternalScopeProvider externalScopeProvider;
        private string _name;
        public MyLogger(string name)
        {
            externalScopeProvider = null;
            _name = name;
        }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var t_stringWriter = new StringWriter();
            LogEntry<TState> logEntry = new LogEntry<TState>(logLevel, nameof(state), eventId, state, exception, formatter);
            new MyConsoleFormat(_name).Write(logEntry, this.externalScopeProvider, t_stringWriter);
            var sb = t_stringWriter.GetStringBuilder();
            if (sb.Length == 0)
            {
                return;
            }
            string computedAnsiString = sb.ToString();
            sb.Clear();
            Console.WriteLine(computedAnsiString);
        }
    }
    internal class MyConsoleFormat : ConsoleFormatter
    {
        public MyConsoleFormat(string name) : base(name)
        {
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
        {
            textWriter.Write($"message:{logEntry.State}");
        }
    }
}
