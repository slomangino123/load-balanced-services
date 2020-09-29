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
            foreach (var header in headers)
            {
                _logger.LogDebug($"Post: {header.Key} : {header.Value}");
            }

            return _next(context);
        }
    }
}
