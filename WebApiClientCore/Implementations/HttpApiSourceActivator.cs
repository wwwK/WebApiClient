using System;
using System.Reflection;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Internals.Utilities;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示THttpApi的实例创建器
    /// 通过查找类型代理类型创建实例
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class HttpApiSourceActivator<THttpApi> : HttpApiActivator<THttpApi>
    {
        /// <summary>
        /// 通过查找类型代理类型创建实例
        /// </summary>
        /// <param name="apiActionDescriptorProvider"></param>
        /// <param name="actionInvokerProvider"></param>
        public HttpApiSourceActivator(IApiActionDescriptorProvider apiActionDescriptorProvider, IApiActionInvokerProvider actionInvokerProvider)
            : base(apiActionDescriptorProvider, actionInvokerProvider)
        {
        }

        /// <summary>
        /// 创建实例工厂
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        protected override Func<IApiActionInterceptor, ApiActionInvoker[], THttpApi> CreateFactory()
        {
            var proxyType = FindProxyType(typeof(THttpApi));
            if (proxyType == null)
            {
                throw new NotSupportedException();
            }
            return Lambda.CreateCtorFunc<IApiActionInterceptor, ApiActionInvoker[], THttpApi>(proxyType);
        }

        /// <summary>
        /// 从接口所在程序集查找代理类
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        private static Type? FindProxyType(Type interfaceType)
        {
            var httpApiType = interfaceType.IsGenericType
                ? interfaceType.GetGenericTypeDefinition()
                : interfaceType;

            foreach (var proxyType in interfaceType.Assembly.GetTypes())
            {
                var proxyClass = proxyType.GetCustomAttribute<HttpApiProxyClassAttribute>();
                if (proxyClass != null && proxyClass.HttpApiType == httpApiType)
                {
                    return proxyType.IsGenericType
                        ? proxyType.MakeGenericType(interfaceType.GetGenericArguments())
                        : proxyType;
                }
            }
            return null;
        }
    }
}
