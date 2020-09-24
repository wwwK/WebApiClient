using System;
using System.Reflection;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示THttpApi的实例创建器
    /// 通过查找类型代理类型创建实例
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    class HttpApiSourceActivator<THttpApi> : HttpApiActivator<THttpApi>
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
            var httpApiType = interfaceType.IsGenericType
                ? interfaceType.GetGenericTypeDefinition()
                : interfaceType;

            foreach (var proxyType in interfaceType.Assembly.GetTypes())
            {
                var attribute = proxyType.GetCustomAttribute<ProxyTypeAttribute>();
                if (attribute != null && attribute.HttpApiType == httpApiType)
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
