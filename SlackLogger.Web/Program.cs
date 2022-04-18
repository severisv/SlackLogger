
using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace SlackLogger.Web._2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddSlack(options =>
                    {
                        options.LogLevel = LogLevel.Information;
                        options.Channel = "#slacklogger";
                        options.ApplicationName = "Hei";
                    });


                    logging.AddDebug();
                })
                .UseStartup<Startup>()
                .Build();
    }
}
