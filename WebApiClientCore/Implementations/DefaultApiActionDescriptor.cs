﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示Action描述
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public class DefaultApiActionDescriptor : ApiActionDescriptor
    {
        /// <summary>
        /// 获取所在接口类型
        /// 这个值不一定是声明方法的接口类型
        /// </summary>
        public override Type InterfaceType { get; protected set; }

        /// <summary>
        /// 获取Api名称
        /// </summary>
        public override string Name { get; protected set; }

        /// <summary>
        /// 获取关联的方法信息
        /// </summary>
        public override MethodInfo Member { get; protected set; }

        /// <summary>
        /// 获取Api关联的缓存特性
        /// </summary>
        public override IApiCacheAttribute? CacheAttribute { get; protected set; }

        /// <summary>
        /// 获取Api关联的特性
        /// </summary>
        public override IReadOnlyList<IApiActionAttribute> Attributes { get; protected set; }

        /// <summary>
        /// 获取Api关联的过滤器特性
        /// </summary>
        public override IReadOnlyList<IApiFilterAttribute> FilterAttributes { get; protected set; }


        /// <summary>
        /// 获取Api的参数描述
        /// </summary>
        public override IReadOnlyList<ApiParameterDescriptor> Parameters { get; protected set; }

        /// <summary>
        /// 获取Api的返回描述
        /// </summary>
        public override ApiReturnDescriptor Return { get; protected set; }

        /// <summary>
        /// 获取自定义数据存储的字典
        /// </summary>
        public override ConcurrentDictionary<object, object> Properties { get; protected set; }

        /// <summary>
        /// 请求Api描述
        /// for test only
        /// </summary>
        /// <param name="method">接口的方法</param>
        internal DefaultApiActionDescriptor(MethodInfo method)
            : this(method, method.DeclaringType)
        {
        }

        /// <summary>
        /// 请求Api描述
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <param name="interfaceType">接口类型</param> 
        public DefaultApiActionDescriptor(MethodInfo method, Type interfaceType)
        {
            var methodAttributes = method.GetCustomAttributes().ToArray();
            var interfaceAttributes = interfaceType.GetInterfaceCustomAttributes();

            // 接口特性优先于方法所在类型的特性
            var actionAttributes = methodAttributes
                .OfType<IApiActionAttribute>()
                .Concat(interfaceAttributes.OfType<IApiActionAttribute>())
                .Distinct(MultiplableComparer<IApiActionAttribute>.Instance)
                .OrderBy(item => item.OrderIndex)
                .ToReadOnlyList();

            var filterAttributes = methodAttributes
                .OfType<IApiFilterAttribute>()
                .Concat(interfaceAttributes.OfType<IApiFilterAttribute>())
                .Distinct(MultiplableComparer<IApiFilterAttribute>.Instance)
                .OrderBy(item => item.OrderIndex)
                .Where(item => item.Enable)
                .ToReadOnlyList();

            this.InterfaceType = interfaceType;

            this.Member = method;
            this.Name = method.Name;
            this.Attributes = actionAttributes;
            this.CacheAttribute = methodAttributes.OfType<IApiCacheAttribute>().FirstOrDefault();
            this.FilterAttributes = filterAttributes;
            this.Properties = new ConcurrentDictionary<object, object>();

            this.Return = new DefaultApiReturnDescriptor(method.ReturnType, methodAttributes, interfaceAttributes);
            this.Parameters = method.GetParameters().Select(p => new DefaultApiParameterDescriptor(p)).ToReadOnlyList();
        }
    }
}
