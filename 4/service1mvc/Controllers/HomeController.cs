using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
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

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Api()
        {
            return View();
        }

        public async Task<IActionResult> ApiCall(ApiResultModel model)
        {
            model = new ApiResultModel();
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
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
