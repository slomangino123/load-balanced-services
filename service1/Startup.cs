using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using service1.Configuration;
using Shared;
using Shared.Middleware;

namespace service1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Service2Options>(Configuration.GetSection("ConnectionStrings:Service2"));
            services.AddScoped<IEndpointDetailsService, EndpointDetailsService>();
            services.AddMvc();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;

                options.KnownProxies.Clear();
                options.KnownNetworks.Clear();
                //options.KnownProxies.Add(IPAddress.Parse("127.0.10.1"));

                foreach (var proxy in Configuration.GetSection("KnownProxies").AsEnumerable().Where(c => c.Value != null))
                {
                    options.KnownProxies.Add(IPAddress.Parse(proxy.Value));
                }
            });

            services.Configure<HstsOptions>(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromMinutes(1);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<Startup>();

            app.UseMiddleware<LogRequestIsHttpsMiddleware>();
            app.UseMiddleware<PreForwardedHeadersLogHeadersMiddleware>();
            app.UseForwardedHeaders();
            app.UseMiddleware<PostForwardedHeadersLogHeadersMiddleware>();
            app.UseMiddleware<LogRequestIsHttpsMiddleware>();

            app.UseMiddleware<LogResponseHeadersMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                logger.LogDebug($"Configuring HSTS");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapGet("api/test", EndpointExtensions.TestEndpoint("service1"));
            });
        }
    }
}
