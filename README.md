# SlackLogger

A simple and configurable logger for .NET projects that logs to Slack.

SlackLogger will post formatted log messages at the desired level to an incoming webhook in a custom Slack App.

[![Build status](https://ci.appveyor.com/api/projects/status/xirkw5ma3prrs70t?svg=true)](https://ci.appveyor.com/project/SeverinSverdvik/slacklogger)

![Example log message](/documentation/logexample.png)
![Example log message](/documentation/exceptionexample.png)

## Usage

For .NET >= 6 reference package `SlackLogger` >= `4.0`.

For .NET 2 to 5 Reference version `3` ([docs here](https://github.com/severisv/SlackLogger/tree/613ce47128f03c5cd0be129c65414475a8d85822))


## Getting started
Create a Webhook:
1. Go to https://api.slack.com/apps
2. Create Slack App (choose "from scratch")
3. Select a name for your app (this will be the name the logger posts as) and select the Slack workspace you want to post to
4. Activate "Incoming Webhooks"
5. Add new webhook to workspace (select which channel you would like to log to)
6. Copy the wehbook url - you will need it for the configuration of SlackLogger

If you wish to post to different channels for different environments, you have to create one webhook for each environment and configure accordingly.

To log from a web application:

`Program.cs`

```cs
using SlackLogger;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSlack(options =>
    {
        options.WebhookUrl = "https://hooks.slack.com/services/MYWBHKK/ADRS/552989R2D21337";
    });

var app = builder.Build();

```

or in a console application:
`Program.cs`

```cs
using SlackLogger;

var configuration =
        new ConfigurationBuilder()
            .AddJsonFile($"{Directory.GetCurrentDirectory()}/appsettings.json", false, true)
            .Build();

var serviceProvider = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
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
     options.SanitizeOutputFunction = output => Regex.Replace(output, "@[^\\.@-]", "");
});

```

`WebhookUrl`
Url of the Slack webhook. Required unless `IsOptional` is set to `true`.

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

`IsOptional`
Makes the logger configuration optional, meaning that it will fail silently and just not log anything when a `WebhookUrl` is not present.

### Deprecated options
The following properties are deprecated, as setting the username and channel in code is no longer supported by the Slack integration.

`UserName` (DEPRECATED)
Sets the username sent to Slack. Defaults to `SlackLogger`. Set it to null, if you want to use the default webhook username.

`Channel` (DEPRECATED)
Overrides the channel or person that is configured in the Slack webhook.

`ChannelCritical`, `ChannelError`, `ChannelWarning`, `ChannelInformation`, `ChannelDebug`, `ChannelTrace` (DEPRECATED)
Overrides the `Channel` setting for the specified log level. If any of these are unset, it will fall back to the `Channel` value.


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

...

.ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddSlack();
                })
```

`appsettings.json`:

```json
"Logging": {
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
