using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SlackLogger
{
    public class LoggerProvider : ILoggerProvider
    {

        private readonly SlackLoggerOptions _options;
        private readonly ScopeSettings _scopeSettings;

        public LoggerProvider(IOptions<LoggerFilterOptions> filterOptions, IOptions<SlackLoggerOptions> options, IConfiguration configuration)
        {
            _options = options.Value;
            _options.Merge(configuration.GetSection("Logging:Slack"));
            
            _options.ApplicationName = string.IsNullOrEmpty(_options.ApplicationName)
                ? System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Name
                : _options.ApplicationName;


            if (string.IsNullOrEmpty(_options.EnvironmentName))
            {
                try
                {
                    _options.EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                }
                catch (Exception)
                {
                    // If something wrong happens trying to get the default environment, we're better off continuing with it unset
                }
            }

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