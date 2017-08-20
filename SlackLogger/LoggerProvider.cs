using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SlackLogger
{
    internal class LoggerProvider : ILoggerProvider
    {

        private readonly SlackLoggerOptions _options;
        private readonly ScopeSettings _scopeSettings;

        public LoggerProvider(IOptions<LoggerFilterOptions> filterOptions, IOptions<SlackLoggerOptions> options, IConfiguration configuration, IHostingEnvironment env)
        {
            _options = options.Value;
            _options.Merge(configuration.GetSection("Logging:Slack"));
            
            _options.EnvironmentName = env?.EnvironmentName;
            _options.ApplicationName = !string.IsNullOrEmpty(_options.ApplicationName)
                ? _options.ApplicationName
                : env?.ApplicationName;

            _options.ValidateWebookUrl();

            var filterSettings = filterOptions.Value;
            if (!filterSettings.Rules.Any())
            {
                filterSettings.Rules.Add(new LoggerFilterRule(null, null, LogLevel.Information, null));
            }
            _scopeSettings = new ScopeSettings(filterSettings);
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