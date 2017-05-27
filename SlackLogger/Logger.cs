using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace SlackLogger
{
    public class Logger : ILogger<Type>
    {
        private readonly SlackLoggerOptions _options;
        private readonly string _environmentName;
        private readonly IEnumerable<LogLevel> _enabledLogLevels;
        private readonly string _name;
        private readonly Func<string, LogLevel, bool> _scopeFilter;

        public Logger(string name, SlackLoggerOptions options, Func<string, LogLevel, bool> scopeFilter)
        {
            _name = name;
            _environmentName = options.Environment ?? "";
            _options = options;
            _enabledLogLevels = GetEnabledLogLevels(_options.LogLevel);
            _scopeFilter = scopeFilter ?? ((category, logLevel) => true);
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

                var message = FormatLogMessage(state, exception, formatter);

                var slackService = new SlackService(_options);
                slackService.Log(logLevel, _name, message, _environmentName, exception);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in SlackLogger: {e.Message} \n {e.StackTrace}");
            }
        }

        private static string FormatLogMessage<TState>(TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            string message;
            if (exception == null)
            {
                if (formatter != null)
                {
                    message = formatter(state, null);
                }
                else
                {
                    message = state.ToString();
                }
            }
            else
            {
                message = state + " Message: " + exception.Message;
            }
            return message;
        }


        public bool IsEnabled(LogLevel logLevel)
        {
            var passesFilter = _scopeFilter(_name, logLevel);
            return passesFilter && _enabledLogLevels.Contains(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        private IEnumerable<LogLevel> GetEnabledLogLevels(LogLevel logLevel)
        {
            var result = new List<LogLevel>();
            for (int i = (int) logLevel; i < (int) LogLevel.None; i++)
            {
                result.Add((LogLevel) i);
            }
            return result;
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