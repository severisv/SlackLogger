using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SlackLogger
{
    public class SlackLoggerOptions
    {
        public string Channel { get; set; }
        public string ChannelCritical { get; set; }
        public string ChannelError { get; set; }
        public string ChannelWarning { get; set; }
        public string ChannelInformation { get; set; }
        public string ChannelDebug { get; set; }
        public string ChannelTrace { get; set; }
        public string ApplicationName { get; set; }
        public string UserName { get; set; } = "SlackLogger";
        public string WebhookUrl { get; set; }
        public string EnvironmentName { get; set; }
        public Func<string, string> SanitizeOutputFunction { get; set; }
        private LogLevel? _logLevel;
        public LogLevel LogLevel 
        {
            get => _logLevel ?? Microsoft.Extensions.Logging.LogLevel.Warning;
            set => _logLevel = value;
        }
        private LogLevel? _notificationLevel;
        public LogLevel NotificationLevel
        {
            get => _notificationLevel ?? Microsoft.Extensions.Logging.LogLevel.Error;
            set => _notificationLevel = value;
        }

        /// <summary>
        /// If <see langword="true"/>, <see cref="LogLevel"/> will be forced to <see cref="LogLevel.None"/> given invalid options,
        /// e.g. <see cref="WebhookUrl"/> is not set.
        /// </summary>
        public bool Optional { get; set; }

        public void Merge(IConfiguration configuration)
        {
            if (configuration != null)
            {
                if (configuration["ApplicationName"] != null)
                {
                    ApplicationName = configuration["ApplicationName"];
                }
                if (configuration["EnvironmentName"] != null)
                {
                    EnvironmentName = configuration["EnvironmentName"];
                }
                if (configuration["UserName"] != null)
                {
                    UserName = configuration["UserName"];
                }
                if (configuration["Channel"] != null)
                {
                    Channel = configuration["Channel"];
                }
                if (configuration["ChannelCritical"] != null)
                {
                    ChannelCritical = configuration["ChannelCritical"];
                }
                if (configuration["ChannelError"] != null)
                {
                    ChannelError = configuration["ChannelError"];
                }
                if (configuration["ChannelWarning"] != null)
                {
                    ChannelWarning = configuration["ChannelWarning"];
                }
                if (configuration["ChannelInformation"] != null)
                {
                    ChannelInformation = configuration["ChannelInformation"];
                }
                if (configuration["ChannelDebug"] != null)
                {
                    ChannelDebug = configuration["ChannelDebug"];
                }
                if (configuration["ChannelTrace"] != null)
                {
                    ChannelTrace = configuration["ChannelTrace"];
                }
                if (configuration["WebhookUrl"] != null)
                {
                    WebhookUrl = configuration["WebhookUrl"];
                }
                if (configuration["Environment"] != null)
                {
                    EnvironmentName = configuration["Environment"];
                }
                if (configuration["LogLevel"] != null)
                {
                    LogLevel = ParseLogLevel(configuration, "LogLevel");
                }
                if (configuration["NotificationLevel"] != null)
                {
                    NotificationLevel = ParseLogLevel(configuration, "NotificationLevel");
                }
            }
        }

        private static LogLevel ParseLogLevel(IConfiguration config, string key)
        {
            try
            {
                return (LogLevel)Enum.Parse(typeof(LogLevel), config[key]);
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Can't map the config value {key} to a LogLevel. Allowed values [{string.Join(", ", Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>())}]", e);
            }
        }

        public void ValidateWebookUrl()
        {
            if (string.IsNullOrWhiteSpace(WebhookUrl))
            {
                if (Optional)
                {
                    LogLevel = LogLevel.None;
                    return;
                }

                throw new ArgumentException("WebhookUrl must be set");
            }
            Uri uriResult;
            if (!(Uri.TryCreate(WebhookUrl, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == "http" || uriResult.Scheme == "https")))
            {
                throw new ArgumentException($"Invalid WebhookUrl: {WebhookUrl}");
            }
        }
    }
}
