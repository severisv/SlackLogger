using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace SlackLogger
{
    internal class ScopeSettings
    {
        private readonly LoggerFilterOptions _configuration;

        public ScopeSettings(LoggerFilterOptions configuration)
        {
            var config = new LoggerFilterOptions { MinLevel = configuration.MinLevel };
            foreach (var rule in configuration.Rules.Where(r => r.ProviderName == LoggerProvider.ProviderAlias
                                    || r.ProviderName == null)
            )
            {
                config.Rules.Add(rule);
            }
            _configuration = config;
        }



        public Func<string, LogLevel, bool?> GetFilter(string name)
        {
            foreach (var prefix in GetKeyPrefixes(name))
            {
                LogLevel level;
                if (TryGetSwitch(prefix, out level))
                {
                    return (n, l) => l >= level;
                }
            }
            return (n, l) => null;
        }


        private IEnumerable<string> GetKeyPrefixes(string name)
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

        private bool TryGetSwitch(string nameSpace, out LogLevel level)
        {
            var switches = _configuration.Rules;
            if (!switches.Any())
            {
                level = LogLevel.None;
                return false;
            }

            var value = switches.FirstOrDefault(s => s.CategoryName == nameSpace && s.ProviderName == LoggerProvider.ProviderAlias)?.LogLevel;

            if (value == null)
            {
                level = LogLevel.None;
                return false;
            }
            level = value.Value;
            return true;
        }
    }
}