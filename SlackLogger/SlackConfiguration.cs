using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SlackLogger
{
    public static class SlackConfiguration
    {
       
        public static void AddSlack(this ILoggerFactory loggerFactory,
                SlackLoggerOptions options,
            IConfiguration slackConfiguration = null,
            IConfiguration loggingConfiguration = null,
            IHostingEnvironment environment = null)
        {
            loggerFactory.AddProvider(new SlackLoggerProvider(options, slackConfiguration, loggingConfiguration, environment));
        }
    }
}