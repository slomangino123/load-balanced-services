using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace service1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Hello from service 1!" };
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<string>> Post([FromQuery] string endpoint)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var service2Direct = "http://host.docker.internal:32080";
                    var service2LoadBalancer = "http://host.docker.internal:8002";
                    var result = await httpClient.GetAsync($"{service2LoadBalancer}/api/{endpoint}");
                    return await result.Content.ReadAsStringAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return "didnt work";
        }
    }
}
