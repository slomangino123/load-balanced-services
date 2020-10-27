using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared.Middleware
{
    public class LogResponseHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogResponseHeadersMiddleware> _logger;

        public LogResponseHeadersMiddleware(RequestDelegate next, ILogger<LogResponseHeadersMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            var log = "Response Headers:\n";

            foreach (var header in context.Response.Headers)
            {
                log += $"{header.Key} : {header.Value}\n";
            }

            _logger.LogDebug(log);
        }
    }
}
