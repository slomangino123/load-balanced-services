using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using service1mvc.Models;
using Shared;

namespace service1mvc.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Test()
        {
            return View();
        }

        public IActionResult ServiceTest()
        {
            var context = Request.HttpContext;
            var serviceName = "service1";
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

            return View(nameof(Test), model);
        }

        [HttpGet]
        public IActionResult Api()
        {
            return View();
        }

        public async Task<IActionResult> ApiCall()
         {
            var model = new ApiResultModel();
            var handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true,
                ServerCertificateCustomValidationCallback = (sender, cert, chain, error) =>
                {
                    model.Thumbprint = cert.Thumbprint;
                    model.Subject = cert.Subject;
                    model.Error = error;
                    return CertificateExtensions.CertificateValidationCallBack(sender, cert, chain, error, model);
                }
            };

            using (var httpClient = new HttpClient(handler))
            //using (var httpClient = new HttpClient())
            {
                try
                {
                    var service2LoadBalancerHttps = "https://host.docker.internal:8202";
                    var result = await httpClient.GetAsync($"{service2LoadBalancerHttps}/api/test");
                    model.Result = JsonSerializer.Deserialize<TestApiResultModel>(await result.Content.ReadAsStringAsync());
                    return View(model);
                }
                catch (Exception e)
                {
                    model.Exception = $"{e.Message}, {e.InnerException?.Message}";
                }
            }

            return View(nameof(Api), model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
