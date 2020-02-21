# SlackLogger
A simple and configurable logger for ASP.NET Core projects that logs to Slack.

SlackLogger will post formatted log messages at the desired level to a specified channel or person.

[![Build status](https://ci.appveyor.com/api/projects/status/xirkw5ma3prrs70t?svg=true)](https://ci.appveyor.com/project/SeverinSverdvik/slacklogger)

![Example log message](/documentation/logexample.png)
![Example log message](/documentation/exceptionexample.png)

## Usage
Works with ASP.NET Core >= `2.0`. Reference package `SlackLogger` >= `2.0`. 

To use with ASP.NET Core 1.0 reference SlackLogger version < `2.0` ([docs here](https://github.com/severisv/SlackLogger/tree/f00cabfddaec673e35201f9ebeff6b5dd927972a))

`Program.cs`
```cs
  public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddSlack(options =>
                    {
                        options.WebhookUrl = "https://hooks.slack.com/services/ABC123FGH321QWERTYUICAZzDJBG3sehHH7scclYdDxj";
                    });

                })
                .UseStartup<Startup>()
                .Build();
```

or in a console application:
`Program.cs`
```cs
      IConfiguration configuration =
                new ConfigurationBuilder()
                    .AddJsonFile($"{Directory.GetCurrentDirectory()}/appsettings.json", false, true)
                    .Build();
            
      var serviceProvider = new ServiceCollection()
            .AddSingleton(configuration)
            .AddLogging(builder =>
        {
            builder.AddConfiguration(Configuration.GetSection("Logging"));
            builder.AddSlack(options =>
            {
                options.WebhookUrl =
                    "https://hooks.slack.com/services/ABC123FGH321QWERTYUICAZzDJBG3sehHH7scclYdDxj";
            });
        }).BuildServiceProvider();
    
    
        var logger = serviceProvider.GetService<ILogger<Program>>();
```

## Configuration

The logger has a number of optional settings:

```cs


logging.AddSlack(options =>
{
     options.WebhookUrl = "https://hooks.slack.com/services/ABC123FGH321QWERTYUICAZzDJBG3sehHH7scclYdDxj";
     options.LogLevel = LogLevel.Information;
     options.NotificationLevel = LogLevel.None;
     options.Environment = env.EnvironmentName;
     options.ApplicationName = "My application";
     options.Channel = "#mychannel";
     options.ChannelCritical = "#mychannel-critical";
     options.SanitizeOutputFunction = output => Regex.Replace(output, "@[^\\.@-]", "");
     options.UserName = "SlackLogger";
});
            
```

`Channel`
Overrides the channel or person that is configured in the Slack webhook.

`ChannelCritical`, `ChannelError`, `ChannelWarning`, `ChannelInformation`, `ChannelDebug`, `ChannelTrace`
Overrides the `Channel` setting for the specified log level. If any of these are unset, it will fall back to the `Channel` value.

`LogLevel`
Sets the minimum log level used. Defaults to `Warning`.

`NotificationLevel`
Sets the minimum log level that causes notifications on Slack (using @channel). Defaults to `Error`.

`ApplicationName`
Prints the name of the current hosting environment in each log statement, if set. Defaults to the entry assembly name.

`EnvironmentName`
Prints the name of the current hosting environment in each log statement, if set. Defaults to env variable `ASPNETCORE_ENVIRONMENT`, if set, otherwise it is empty.

`SanitizeOutputFunction`
Can be used to modify the messages (including stack traces) before they are logged. Used if one is concerned that certain details are too sensitive to be posted to Slack.

`UserName`
Sets the username sent to Slack. Defaults to `SlackLogger`. Set it to null, if you want to use the default webhook username.


## Configure using ConfigurationProvider
SlackLogger can be completely or partially configured using a configuration provider:

```cs

.ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddSlack();
                })

```

`appsettings.json`:
```json
"Logging": {
    "Slack": {
    "WebhookUrl": "https://hooks.slack.com/services/ABC123FGH321QWERTYUICAZzDJBG3sehHH7scclYdDxj",
    "LogLevel": "Information",
    "NotificationLevel": "Error"
    }
}

```

If the same property is set both in code and in the configuration provider, the value from the configuration provider is used.

## Filtering namespaces
The logger filters log statements from different part of the code using namespaces in the same way as the default loggers, if that section of the configuration is provided:

```cs
public IConfigurationRoot Configuration { get; set; }


.ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddSlack();
                })



```

`appsettings.json`:
```json
"Logging": {
    "IncludeScopes": false,
    "LogLevel": {
        "Default": "Debug",
        "System": "Information",
        "Microsoft": "Warning"
    },
    "Slack": {
        "WebhookUrl": "https://hooks.slack.com/services/ABC123FGH321QWERTYUICAZzDJBG3sehHH7scclYdDxj"
    }
},

```
