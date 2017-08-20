using System;
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

            bool includeScopes;
            bool.TryParse(configuration.GetSection("Logging")["IncludeScopes"], out includeScopes);
       
            _scopeSettings = new ScopeSettings(filterOptions.Value, includeScopes: includeScopes);
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