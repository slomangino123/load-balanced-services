using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;

namespace Shared
{
    public class ApiResultModel
    {
        public string Thumbprint { get; set; }

        public string Subject { get; set; }

        public SslPolicyErrors Error { get; set; }

        public string Result { get; set; }

        public string Exception { get; set; }

        public string Reason { get; set; }
    }
}
