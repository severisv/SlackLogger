using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SlackLogger.Web.Controllers
{
    [Route("log")]
    public class LoggController : Controller
    {
        private readonly ILogger<LoggController> _logger;

        public LoggController(ILogger<LoggController> logger)
        {
            _logger = logger;
        }

        [Route("debug/{message?}")]
        public void Debug(string message = "debug") => _logger.LogDebug(message);

        [Route("info/{message?}")]
        public void Info(string message = "info") => _logger.LogInformation(message);

        [Route("Trace/{message?}")]
        public void Trace(string message = "trace") => _logger.LogTrace(message);

        [Route("Critical/{message?}")]
        public void Critical(string message = "critical") => _logger.LogCritical(message);

        [Route("Warning/{message?}")]
        public void Warning(string message = "warning") => _logger.LogWarning(message);

        [Route("Error/{message?}")]
        public void Error(string message = "error") => throw new Exception(message);

    }
}