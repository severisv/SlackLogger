
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
                    logging.AddConsole(options => options.IncludeScopes = true);
                    logging.AddSlack(options =>
                    {
                        options.WebhookUrl = "";
                        options.LogLevel = LogLevel.Information;
                        options.ApplicationName = "Slacklogger";
                    });
                   
                    logging.AddDebug();
                })
                .UseStartup<Startup>()
                .Build();
    }
}
