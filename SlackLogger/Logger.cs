using System;
using Microsoft.Extensions.Logging;

namespace SlackLogger
{
    public class Logger : ILogger<Type>
    {
        private readonly LogLevel DefaultLogLevel = LogLevel.Warning;

        private readonly SlackLoggerOptions _options;
        private readonly string _environmentName;
        private readonly string _name;
        private readonly Func<string, LogLevel, bool?> _scopeFilter;


        public Logger(string name, SlackLoggerOptions options, Func<string, LogLevel, bool?> scopeFilter)
        {
            _name = name;
            _environmentName = options.EnvironmentName ?? "";
            _options = options;
            _scopeFilter = scopeFilter;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            try
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                var message = FormatLogMessage(state, formatter);

                var slackService = new SlackService(_options);
                slackService.Log(logLevel, _name, message, _environmentName, exception);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in SlackLogger: {e.Message} \n {e.StackTrace}");
            }
        }

        private static string FormatLogMessage<TState>(TState state,
            Func<TState, Exception, string> formatter)
        {
            string message;
            if (formatter != null)
            {
                message = formatter(state, null);
            }
            else
            {
                message = state?.ToString();
            }
            return message;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var passesFilter = _scopeFilter(_name, logLevel);
            return passesFilter != null ? 
                    passesFilter.Value : 
                    logLevel >= (_options.LogLevel ?? DefaultLogLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        private class NoopDisposable : IDisposable
        {
            public static readonly NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }
}