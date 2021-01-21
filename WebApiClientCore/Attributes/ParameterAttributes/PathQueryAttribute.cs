﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Internals;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用KeyValueSerializer序列化参数值得到的键值对作为url路径参数或query参数的特性
    /// 没有任何特性修饰的参数，将默认被PathQueryAttribute修饰
    /// </summary>
    public class PathQueryAttribute : ApiParameterAttribute, ICollectionFormatable
    {
        /// <summary>
        /// 获取或设置集合格式化方式
        /// </summary>
        public CollectionFormat CollectionFormat { get; set; } = CollectionFormat.Multi;

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiParameterContext context)
        {
            var uri = context.HttpContext.RequestMessage.RequestUri;
            if (uri == null)
            {
                throw new ApiInvalidConfigException(Resx.required_HttpHost);
            }

            var keyValues = this.SerializeToKeyValues(context).CollectAs(this.CollectionFormat);
            context.HttpContext.RequestMessage.RequestUri = this.CreateUri(uri, keyValues);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 序列化参数为keyValue
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected virtual IEnumerable<KeyValue> SerializeToKeyValues(ApiParameterContext context)
        {
            return context.SerializeToKeyValues();
        }

        /// <summary>
        /// 创建新的uri
        /// </summary>
        /// <param name="uri">原始uri</param>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        protected virtual Uri CreateUri(Uri uri, IEnumerable<KeyValue> keyValues)
        {
            var uriValue = new UriValue(uri);
            foreach (var keyValue in keyValues)
            {
                uriValue = uriValue.Replace(keyValue.Key, keyValue.Value, out var replaced);
                if (replaced == false)
                {
                    uriValue = uriValue.AddQuery(keyValue.Key, keyValue.Value);
                }
            }
            return uriValue.ToUri();
        }
    }
}
