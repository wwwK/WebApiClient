﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示http请求方法描述特性
    /// </summary>
    [DebuggerDisplay("Method = {Method}")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HttpMethodAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 获取请求方法
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        /// 获取请求相对路径
        /// </summary>
        public HttpPath Path { get; }

        /// <summary>
        /// http请求方法描述特性
        /// </summary>
        /// <param name="method">请求方法</param>
        public HttpMethodAttribute(string method)
            : this(method, path: null)
        {
        }

        /// <summary>
        /// http请求方法描述特性
        /// </summary>
        /// <param name="method">请求方法</param>
        /// <param name="path">请求绝对或相对路径</param>
        public HttpMethodAttribute(string method, string? path)
            : this(new HttpMethod(method), path)
        {
        }

        /// <summary>
        /// http请求方法描述特性
        /// </summary>
        /// <param name="method">请求方法</param>
        /// <param name="path">请求绝对或相对路径</param>
        protected HttpMethodAttribute(HttpMethod method, string? path)
        {
            this.Method = method;
            this.Path = HttpPath.Create(path);
            this.OrderIndex = int.MinValue + 1;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            var baseUri = context.HttpContext.RequestMessage.RequestUri;
            var requestUri = this.Path.MakeUri(baseUri);

            context.HttpContext.RequestMessage.Method = this.Method;
            context.HttpContext.RequestMessage.RequestUri = requestUri;
            return Task.CompletedTask;
        }
    }
}
