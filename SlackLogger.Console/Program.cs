using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlackLogger;

var Configuration = new ConfigurationBuilder()
               .AddJsonFile($"{Directory.GetCurrentDirectory()}/appsettings.json", false, true)
               .Build();


var serviceProvider = new ServiceCollection()
    .AddSingleton<IConfiguration>(Configuration)
    .AddLogging(builder =>
{
    builder.AddConfiguration(Configuration.GetSection("Logging"));
    builder.AddSlack(options =>
    {
        options.ApplicationName = "Application name";
        options.WebhookUrl = "";
        options.LogLevel = LogLevel.Information;
    });


}).BuildServiceProvider();


var logger = serviceProvider.GetService<ILogger<Program>>();
logger.LogInformation("Example log message");

Thread.Sleep(1000);


