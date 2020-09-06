using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class TestApiResultModel
    {
        public string ServiceMessage { get; set; }

        public string HostName { get; set; }

        public string RequestMethod { get; set; }

        public string RequestScheme { get; set; }

        public string RequestUrl { get; set; }

        public string RequestPath { get; set; }

        public string[] RequestHeaders { get; set; }

        public string RemoteIp { get; set; }

        public bool RequestIsWhitelisted { get; set; }
    }
}
