using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SlackLogger
{
    internal class SlackLoggerProvider : ILoggerProvider
    {

        private readonly SlackLoggerOptions _options;
        private readonly IHostingEnvironment _environment;
        private readonly SlackLoggerSettings _settings;

        public SlackLoggerProvider(SlackLoggerOptions options, IConfiguration slackConfiguration, IConfiguration loggingConfiguration, IHostingEnvironment environment)
        {
            _options = BuildOptions(options, slackConfiguration);
            _environment = environment;
            if (loggingConfiguration != null)
            {
                _settings = new SlackLoggerSettings(loggingConfiguration);
            }
        }

        public ILogger CreateLogger(string name)
        {
            var filter = _settings != null ? GetFilter(name, _settings) : null;
            return new global::SlackLogger.SlackLogger(name, _options, _environment, filter);
        }


        private Func<string, LogLevel, bool> GetFilter(string name, SlackLoggerSettings settings)
        {
            if (settings != null)
            {
                foreach (var prefix in GetKeyPrefixes(name))
                {
                    LogLevel level;
                    if (settings.TryGetSwitch(prefix, out level))
                    {
                        return (n, l) => l >= level;
                    }
                }
            }
            return (n, l) => false;
        }

        public IEnumerable<string> GetKeyPrefixes(string name)
        {
            while (!string.IsNullOrEmpty(name))
            {
                yield return name;
                var lastIndexOfDot = name.LastIndexOf('.');
                if (lastIndexOfDot == -1)
                {
                    yield return "Default";
                    break;
                }
                name = name.Substring(0, lastIndexOfDot);
            }
        }

        private static SlackLoggerOptions BuildOptions(SlackLoggerOptions options, IConfiguration configuration)
        {
            var result = options;
            if (configuration != null)
            {
                if (configuration["Channel"] != null)
                {
                    result.Channel = configuration["Channel"];
                }
                if (configuration["WebhookUrl"] != null)
                {
                    result.WebhookUrl = configuration["WebhookUrl"];
                }
                if (configuration["LogLevel"] != null)
                {
                    result.LogLevel = ParseLogLevel(configuration, "LogLevel");
                }
                if (configuration["NotificationLevel"] != null)
                {
                    result.NotificationLevel = ParseLogLevel(configuration, "NotificationLevel");
                }
            }

            ValidateWebookUrl(result);

            return result;
        }

        private static void ValidateWebookUrl(SlackLoggerOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.WebhookUrl))
            {
                throw new ArgumentException("WebhookUrl must be set");
            }
            Uri uriResult;
            if (!(Uri.TryCreate(options.WebhookUrl, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == "http" || uriResult.Scheme == "https")))
            {
                throw new ArgumentException($"Invalid WebhookUrl: {options.WebhookUrl}");
            }

        }

        private static LogLevel ParseLogLevel(IConfiguration config, string key)
        {
            try
            {
                return (LogLevel) Enum.Parse(typeof(LogLevel), config[key]);
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Can't map the config value {key} to any LogLevel. Allowed values [{string.Join(", ", Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>())}]");
            }
        }


        public void Dispose()
        {
        }
    }
}