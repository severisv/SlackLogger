# SlackLogger
A simple and configurable logger for ASP.NET Core projects that logs to Slack.

SlackLogger will post formatted log messages at the desired level to a specified channel or person on Slack.

![Example log message](/documentation/logexample.png)
![Example log message](/documentation/exceptionexample.png)

## Usage

```cs
public void Configure(IApplicationBuilder app, IHostingEnvironment env, LoggerFactory loggerFactory)
{
    loggerFactory.AddSlack(new SlackLoggerOptions("ApplicationName") {
            WebhookUrl = "https://hooks.slack.com/services/ABC123FGH321QWERTYUICAZzDJBG3sehHH7scclYdDxj"                 
    });
    ...
}
```

## Configuration

The logger has a number of optional settings:

```cs
loggerFactory.AddSlack(new SlackLoggerOptions("ApplicationName") {
        WebhookUrl = "https://hooks.slack.com/services/ABC123FGH321QWERTYUICAZzDJBG3sehHH7scclYdDxj",
        LogLevel = LogLevel.Information,
        NotificationLevel = LogLevel.None,
        Environment = env.EnvironmentName,
        Channel = "#mychannel",
        SanitizeOutputFunction = output => Regex.Replace(output, "@[^\\.@-]", "")

});
```

`Channel`  
Overrides the channel or person that is configured in the Slack webhook.

`LogLevel`  
Sets the minimum log level used. Defaults to `Warning`.

`NotificationLevel`  
Sets the minimum log level that causes notifications on Slack (using @channel). Defaults to `Error`.

`EnvironmentName`  
Prints the name of the current hosting environment in each log statement, if set.

`SanitizeOutputFunction`  
Can be used to modify the messages (including stack traces) before they are logged. Used if one is concerned that certain details are too sensitive to be posted to Slack.  
  

## Configure using ConfigurationProvider
SlackLogger can be completely or partially configured using a configuration provider:

```cs
public IConfigurationRoot Configuration { get; set; }

public void Configure(IApplicationBuilder app, IHostingEnvironment env, LoggerFactory loggerFactory)
{
    loggerFactory.AddSlack(
        new SlackLoggerOptions("ApplicationName"),
        Configuration.GetSection("SlackLogger")
    );
    ...
}
```

`appsettings.json`:
```json
"SlackLogger": {
    "WebhookUrl": "https://hooks.slack.com/services/ABC123FGH321QWERTYUICAZzDJBG3sehHH7scclYdDxj"
    "LogLevel": "Information",
    "NotificationLevel": "Error"      
}
```

If the same property is set both in code and in the configuration provider, the value from the configuration provider is used.

## Filtering namespaces
The logger filters log statements from different part of the code using namespaces in the same way as the default loggers, if that section of the configuration is provided:

```cs
public IConfigurationRoot Configuration { get; set; }

public void Configure(IApplicationBuilder app, IHostingEnvironment env, LoggerFactory loggerFactory)
{
    loggerFactory.AddSlack(
        new SlackLoggerOptions("ApplicationName"),
        Configuration.GetSection("SlackLogger"),
        Configuration.GetSection("Logging"),
    );
    ...
}
```

`appsettings.json`:
```json
"Logging": {
    "IncludeScopes": false,
    "LogLevel": {
        "Default": "Debug",
        "System": "Information",
        "Microsoft": "Warning"
    }
},
"SlackLogger": {
    "WebhookUrl": "https://hooks.slack.com/services/ABC123FGH321QWERTYUICAZzDJBG3sehHH7scclYdDxj"
}
```
