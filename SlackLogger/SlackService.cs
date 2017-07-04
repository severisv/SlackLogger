using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackLogger.Extensions;

namespace SlackLogger
{
    internal class SlackService
    {
        private readonly SlackLoggerOptions _options;

        public SlackService(SlackLoggerOptions options)
        {
            _options = options;
        }


        public void Log(LogLevel logLevel, string typeName, string message,string environmentName, Exception exception = null)
        {
            Post(typeName, message, exception, environmentName, logLevel);
        }


        private void Post(string typeName, string message, Exception exception, string environment, LogLevel logLevel)
        {
            var icon = GetIcon(logLevel);
            var color = GetColor(logLevel);
            var environmentName = string.IsNullOrEmpty(environment) ? "" : $"({environment})";

            var exceptionMessage = exception?.Message;
            var stackTrace = exception?.StackTrace;

            if (_options.SanitizeOutputFunction != null && exception != null)
            {
                exceptionMessage = _options.SanitizeOutputFunction(exceptionMessage);
                stackTrace = _options.SanitizeOutputFunction(stackTrace);
            }

            var formattedStacktrace =
                exception != null
                    ? $"```{exceptionMessage} \n{stackTrace.Truncate(1900)}```"
                    : string.Empty;

            
            var notification = ShouldNotify(logLevel) ? "<!channel>: \n" : "";

            using (var client = new HttpClient())
            {
                var payload = new
                {
                    channel = _options.Channel,
                    username = _options.Application,
                    icon_emoji = icon,
                    text = $"{notification}*{_options.Application}* {environmentName}",
                    attachments = new[]
                    {
                        new
                        {
                            fallback = $"Error in {_options.Application}",
                            color = color,
                            mrkdwn_in = new[] {"fields"},
                            fields = new[]
                            {
                                new
                                {
                                    title = "",
                                    value = $"`{typeName}`",
                                },
                                new
                                {
                                    title = $"{icon} [{logLevel}]",
                                    value = $"{message.Sanitize(_options)}",
                                },
                                new
                                {
                                    title = "",
                                    value = formattedStacktrace,
                                }
                            }
                        }
                    }
                };

                var url = _options.WebhookUrl;
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8,
                    "application/json");
                try
                {
                    client.PostAsync(url, content).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    throw new Exception("Error posting til Slack", e);
                }
            }
        }

        private string GetIcon(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return ":fire:";
                case LogLevel.Error:
                    return ":exclamation:";
                case LogLevel.Warning:
                    return ":warning:";
                case LogLevel.Information:
                    return ":information_source:";
                case LogLevel.Debug:
                    return ":bug:";
                case LogLevel.Trace:
                    return ":mag:";
                default: return "";
            }
        }

        private string GetColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Error:
                case LogLevel.Critical:
                    return "danger";
                case LogLevel.Warning:
                    return "warning";
                case LogLevel.Information:
                    return "#007AB8";
                default: return "";
            }
        }

        private bool ShouldNotify(LogLevel logLevel) => GetNotificationLogLevels().Contains(logLevel);

        private IEnumerable<LogLevel> GetNotificationLogLevels()
        {
            var result = new List<LogLevel>();
            for (int i = (int) _options.NotificationLevel; i < (int) LogLevel.None; i++)
            {
                result.Add((LogLevel) i);
            }
            return result;
        }


       
      
    }

    internal static class StringExtensions
    {
        public static string Sanitize(this string input, SlackLoggerOptions options)
            =>
                options.SanitizeOutputFunction != null ? 
                    options.SanitizeOutputFunction(input) : 
                    input;
        
    }

}
