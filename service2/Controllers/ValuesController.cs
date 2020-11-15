using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using service2.Configuration;
using Shared;

namespace service2.Controllers
{
    [Route("api")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IEndpointDetailsService deetsService;
        private readonly Service1Options service1Options;
        public ValuesController(
            IEndpointDetailsService deetsService,
            IOptions<Service1Options> service2Options)
        {
            this.deetsService = deetsService;
            this.service1Options = service2Options.Value;
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
                    var result = await httpClient.GetAsync($"{service1Options.Http}/api/test");
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
                    var result = await httpClient.GetAsync($"{service1Options.Https}/api/test");
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
