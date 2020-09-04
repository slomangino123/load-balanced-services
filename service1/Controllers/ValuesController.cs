using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace service1.Controllers
{
    [Route("api")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IEndpointDetailsService deetsService;
        public ValuesController(IEndpointDetailsService deetsService)
        {
            this.deetsService = deetsService;
        }

        [HttpGet]
        public ActionResult<EndpointDetailsViewModel> Get()
        {
            var deets = deetsService.HackEndpointDetails();
            return deets;
        }

        [HttpPost]
        [Route("apicall")]
        public async Task<ActionResult<ApiResultModel>> ApiCall()
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
                    return error == SslPolicyErrors.None;
                }
            };

            using (var httpClient = new HttpClient(handler))
            {
                try
                {
                    var service2LoadBalancer = "http://host.docker.internal:8201";
                    var result = await httpClient.GetAsync($"{service2LoadBalancer}/api/test");
                    model.Result = JsonSerializer.Deserialize<TestApiResultModel>(await result.Content.ReadAsStringAsync());
                    return model;
                }
                catch (Exception e)
                {
                    model.Exception = $"{e.Message}, {e.InnerException?.Message}";
                }

                return model;
            }
        }

        [HttpPost]
        [Route("httpsapicall")]
        public async Task<ActionResult<ApiResultModel>> HttpsApiCall()
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
                    return model;
                }
                catch (Exception e)
                {
                    model.Exception = $"{e.Message}, {e.InnerException?.Message}";
                }
            }

            return model;
        }
    }
}
