using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace service2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IEndpointDetailsService deetsService;
        public ValuesController(IEndpointDetailsService deetsService)
        {
            this.deetsService = deetsService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<EndpointDetailsViewModel> Get()
        {
            var deets = deetsService.HackEndpointDetails();
            return deets;
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<ApiResultModel>> Post([FromQuery] string endpoint)
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
                    var service1LoadBalancer = "http://host.docker.internal:8101";
                    var result = await httpClient.GetAsync($"{service1LoadBalancer}/api/{endpoint}");
                    model.Result = await result.Content.ReadAsStringAsync();
                    return model;
                }
                catch (Exception e)
                {
                    model.Exception = $"{e.Message}, {e.InnerException?.Message}";
                }
            }

            return model;
        }

        // POST api/values
        [HttpPost]
        [Route("posthttps")]
        public async Task<ActionResult<ApiResultModel>> PostHttps([FromQuery] string endpoint)
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
                    return CertificateValidationCallBack(sender, cert, chain, error, model);
                }
            };
            using (var httpClient = new HttpClient(handler))
            {
                try
                {
                    var service1LoadBalancerHttps = "https://host.docker.internal:8102";
                    // var service1LoadBalancerHttps = "https://service1:8102";
                    var service1DirectHttps = "https://service1:8102";
                    var result = await httpClient.GetAsync($"{service1LoadBalancerHttps}/api/{endpoint}");
                    model.Result = await result.Content.ReadAsStringAsync();
                    return model;
                }
                catch (Exception e)
                {
                    model.Exception = $"{e.Message}, {e.InnerException?.Message}";
                }
            }

            return model;
        }
        private static bool CertificateValidationCallBack(
        object sender,
        System.Security.Cryptography.X509Certificates.X509Certificate certificate,
        System.Security.Cryptography.X509Certificates.X509Chain chain,
        System.Net.Security.SslPolicyErrors sslPolicyErrors,
        ApiResultModel model)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid.
                            model.Reason = "The cert is self signed and the root is untrusted.";
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                model.Reason = $"There are other errors in the certificate chain. {status.Status}, {status.StatusInformation}";
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                return true;
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }
    }
}
