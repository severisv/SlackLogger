using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SlackLogger.Console
{
 
   public class Program
   {

       public static IConfiguration Configuration { get; } =

           new ConfigurationBuilder()
               .AddJsonFile($"{Directory.GetCurrentDirectory()}/appsettings.json", false, true)
               .Build();
            
        public static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(Configuration)
                .AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddSlack(options =>
                {
                    options.WebhookUrl =
                        "";
                    options.LogLevel = LogLevel.Information;
                });
                
                
            }).BuildServiceProvider();


            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger.LogInformation("Example log message");

            Thread.Sleep(1000);

        }
   
    }

}

