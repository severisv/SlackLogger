using System;
using Microsoft.Extensions.Logging;

namespace SlackLogger
{
    public class SlackLoggerOptions
    {
        public string Channel { get; set; }
        public string Application { get; }
        public string WebhookUrl { get; set; }
        public LogLevel? LogLevel { get; set; }
        public string Environment { get; set; }
        public Func<string, string> SanitizeOutput { get; set; }

        private LogLevel? _notificationLevel;
        public LogLevel NotificationLevel
        {
            get => _notificationLevel ?? Microsoft.Extensions.Logging.LogLevel.Error;
            set => _notificationLevel = value;
        }

        public SlackLoggerOptions(string applicationName)
        {
            Application = applicationName;
        }
    }
}
