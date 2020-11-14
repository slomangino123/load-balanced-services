using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared.Middleware
{
    public class PreForwardedHeadersLogHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PreForwardedHeadersLogHeadersMiddleware> _logger;

        public PreForwardedHeadersLogHeadersMiddleware(RequestDelegate next, ILogger<PreForwardedHeadersLogHeadersMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext context)
        {
            var headers = context.Request.Headers;
            var log = "Pre: \n";
            foreach (var header in headers)
            {
                log += $"{header.Key} : {header.Value}\n";
            }

            _logger.LogDebug(log);

            return _next(context);
        }
    }
}
