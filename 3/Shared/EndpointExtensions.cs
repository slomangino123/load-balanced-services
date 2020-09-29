using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Shared
{
    public static class EndpointExtensions
    {
        public static RequestDelegate TestEndpoint(string serviceName)
        {
            return async context =>
            {
                var name = Dns.GetHostName(); // get container id
                var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

                var model = new TestApiResultModel()
                {
                    ServiceMessage = $"Hello from {serviceName}!",
                    HostName = $"{ Environment.MachineName} \t {name}\t {ip}",
                    RequestMethod = $"{context.Request.Method}",
                    RequestScheme = $"{context.Request.Scheme}",
                    RequestUrl = $"{context.Request.GetDisplayUrl()}",
                    RequestPath = $"{context.Request.Path}",
                    RequestHeaders = context.Request.Headers.Select(x => $"{x.Key}: {x.Value}").ToArray(),
                    RemoteIp = $"{context.Connection.RemoteIpAddress}",
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(model));

                /*
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($"Hello from service2!{Environment.NewLine}");

                // Host info
                var name = Dns.GetHostName(); // get container id
                var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                Console.WriteLine($"Host Name: { Environment.MachineName} \t {name}\t {ip}");
                await context.Response.WriteAsync($"Host Name: {Environment.MachineName}{Environment.NewLine}");
                await context.Response.WriteAsync(Environment.NewLine);

                // Request method, scheme, and path
                await context.Response.WriteAsync($"Request Method: {context.Request.Method}{Environment.NewLine}");
                await context.Response.WriteAsync($"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
                await context.Response.WriteAsync($"Request URL: {context.Request.GetDisplayUrl()}{Environment.NewLine}");
                await context.Response.WriteAsync($"Request Path: {context.Request.Path}{Environment.NewLine}");

                // Headers
                await context.Response.WriteAsync($"Request Headers:{Environment.NewLine}");
                foreach (var (key, value) in context.Request.Headers)
                {
                    await context.Response.WriteAsync($"\t {key}: {value}{Environment.NewLine}");
                }
                await context.Response.WriteAsync(Environment.NewLine);

                // Connection: RemoteIp
                await context.Response.WriteAsync($"Request Remote IP: {context.Connection.RemoteIpAddress}");
                */
            };
        }
    }
}
