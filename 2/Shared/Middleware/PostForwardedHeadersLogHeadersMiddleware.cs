using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared.Middleware
{
    public class PostForwardedHeadersLogHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PostForwardedHeadersLogHeadersMiddleware> _logger;

        public PostForwardedHeadersLogHeadersMiddleware(RequestDelegate next, ILogger<PostForwardedHeadersLogHeadersMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext context)
        {
            var headers = context.Request.Headers;
            var log = "Post: \n";
            foreach (var header in headers)
            {
                log += $"{header.Key} : {header.Value}\n";
            }

            _logger.LogDebug(log);

            return _next(context);
        }
    }
}
