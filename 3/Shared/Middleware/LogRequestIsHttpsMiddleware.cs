using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared.Middleware
{
    public class LogRequestIsHttpsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogRequestIsHttpsMiddleware> _logger;

        public LogRequestIsHttpsMiddleware(RequestDelegate next, ILogger<LogRequestIsHttpsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request.IsHttps;

            var serializedRequest = JsonSerializer.Serialize(request);
            _logger.LogDebug($"Request IsHttps: {serializedRequest}");

            await _next(context);
        }
    }
}
