﻿namespace WebApiClientCore
{
    /// <summary>
    /// 定义方法接口拦截器的行为
    /// </summary>
    public interface IApiActionInterceptor
    {
        /// <summary>
        /// 拦截方法的调用
        /// </summary>
        /// <param name="actionInvoker">action执行器</param> 
        /// <param name="arguments">action的参数集合</param>
        /// <returns></returns>
        object Intercept(ApiActionInvoker actionInvoker, object?[] arguments);
    }
}
