using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace SlackLogger
{
    internal class ScopeSettings
    {
        private readonly LoggerFilterOptions _configuration;
        private readonly bool _includeScopes;

        public ScopeSettings(LoggerFilterOptions configuration, bool includeScopes)
        {
            _configuration = configuration ?? new LoggerFilterOptions();
            _includeScopes = includeScopes;
        }


        public Func<string, LogLevel, bool> GetFilter(string name)
        {
            foreach (var prefix in GetKeyPrefixes(name))
            {
                LogLevel level;
                if (TryGetSwitch(prefix, out level))
                {
                    return (n, l) => l >= level;
                }
            }            
            return (n, l) => false;
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
  

        private bool TryGetSwitch(string name, out LogLevel level)
        {
            
            var switches = _configuration.Rules.Where(r => r.CategoryName == name).ToList();
            if (!switches.Any())
            {
                level = LogLevel.None;
                return false;
            }

            var value = switches.First().LogLevel;
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