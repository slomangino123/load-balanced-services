using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.HttpsPolicy;

namespace Shared
{
    public static class HstsExtensions
    {
        public static bool IsExcluded(HstsOptions options, string host)
        {
            if (options.ExcludedHosts == null)
            {
                return false;
            }

            for (var i = 0; i < options.ExcludedHosts.Count; i++)
            {
                if (string.Equals(host, options.ExcludedHosts[i], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
