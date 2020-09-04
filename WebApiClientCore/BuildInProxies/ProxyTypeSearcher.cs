using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiClientCore.BuildInProxies
{
    static class ProxyTypeSearcher
    {
        public static Type? Find(Type interfaceType)
        {
            if (interfaceType.IsGenericType == false)
            {
                return interfaceType.Assembly.GetType($"____{interfaceType.Name}");
            }

            return null;
        }
    }
}
