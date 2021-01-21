﻿using System;
using System.Reflection;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// ApiActionDescriptor提供者的接口
    /// </summary>
    public class DefaultApiActionDescriptorProvider : IApiActionDescriptorProvider
    {
        /// <summary>
        /// 创建Action描述
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <param name="interfaceType">接口类型</param> 
        public virtual ApiActionDescriptor CreateActionDescriptor(MethodInfo method, Type interfaceType)
        {
            return new DefaultApiActionDescriptor(method, interfaceType);
        }
    }
}
