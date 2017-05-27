using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SlackLogger
{
    internal class LoggerProvider : ILoggerProvider
    {

        private readonly SlackLoggerOptions _options;
        private readonly ScopeSettings _scopeSettings;

        public LoggerProvider(SlackLoggerOptions options, IConfiguration slackConfiguration, IConfiguration loggingConfiguration)
        {
            _options = options;
            _options.Merge(slackConfiguration);
            if (loggingConfiguration != null)
            {
                _scopeSettings = new ScopeSettings(loggingConfiguration);
            }
        }

        public ILogger CreateLogger(string name)
        {
            var scopeFilter = _scopeSettings?.GetFilter(name);
            return new Logger(name, _options, scopeFilter);
        }
          

        public void Dispose()
        {
        }
    }
}