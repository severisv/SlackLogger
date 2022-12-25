using SlackLogger;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSlack(options =>
                    {
                        options.LogLevel = LogLevel.Information;
                        options.WebhookUrl = "";     });

var app = builder.Build();



app.MapGet("/", () => "Hello World!");

app.Run();
