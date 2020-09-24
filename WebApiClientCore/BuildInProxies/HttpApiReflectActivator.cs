using System;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示THttpApi的实例创建器
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    class HttpApiReflectActivator<THttpApi> : HttpApiActivator<THttpApi>
    {
        /// <summary>
        /// 代理类
        /// </summary>
        private static readonly Type? proxyType = FindProxyType(typeof(THttpApi));

        /// <summary>
        /// 获取是否支持
        /// </summary>
        public static bool IsSupported => proxyType != null;

        /// <summary>
        /// 创建实例工厂
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        protected override Func<IActionInterceptor, THttpApi> CreateFactory()
        {
            if (proxyType == null)
            {
                throw new NotSupportedException();
            }

            var actionInvokers = this.GetActionInvokers();
            var proxyTypeCtor = Lambda.CreateCtorFunc<IActionInterceptor, IActionInvoker[], THttpApi>(proxyType);
            return (interceptor) => proxyTypeCtor(interceptor, actionInvokers);
        }

        /// <summary>
        /// 从接口所在程序集查找代理类
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        private static Type? FindProxyType(Type interfaceType)
        {
            const string proxyTypePref = "____";
            foreach (var type in interfaceType.Assembly.GetTypes())
            {
                if (type.IsClass && type.IsAbstract == false && type.Name == $"{proxyTypePref}{interfaceType.Name}")
                {
                    var proxyType = type;
                    if (interfaceType.IsGenericType && proxyType.IsGenericType)
                    {
                        proxyType = proxyType.MakeGenericType(interfaceType.GetGenericArguments());
                    }

                    if (interfaceType.IsAssignableFrom(proxyType) == true)
                    {
                        return proxyType;
                    }
                }
            }
            return null;
        }
    }
}
