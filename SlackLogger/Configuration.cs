using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SlackLogger
{
    public static class SlackConfiguration
    {
        public static ILoggingBuilder AddSlack(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, LoggerProvider>();

            return builder;
        }


        public static ILoggingBuilder AddSlack(this ILoggingBuilder builder, Action<SlackLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddSlack();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}