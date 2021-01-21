﻿using System;
using System.Net.Http;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示HttpClient上下文
    /// </summary>
    public class HttpClientContext
    {
        /// <summary>
        /// 获取HttpClient实例
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 获取选项
        /// </summary>
        public HttpApiOptions HttpApiOptions { get; }

        /// <summary>
        /// 获取选项名称 
        /// </summary>
        public string OptionsName { get; }

        /// <summary>
        /// HttpClient上下文
        /// </summary>
        /// <param name="httpClient">httpClient实例</param>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="httpApiOptions">选项</param>
        /// <param name="optionsName">选项名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpClientContext(HttpClient httpClient, IServiceProvider serviceProvider, HttpApiOptions httpApiOptions, string optionsName)
        {
            this.HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.HttpApiOptions = httpApiOptions ?? throw new ArgumentNullException(nameof(httpApiOptions));
            this.OptionsName = optionsName ?? string.Empty;
        }
    }
}
