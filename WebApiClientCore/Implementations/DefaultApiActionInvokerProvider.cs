﻿using System.Threading.Tasks;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示ApiActionInvoker提供者
    /// </summary>
    public class DefaultApiActionInvokerProvider : IApiActionInvokerProvider
    {
        /// <summary>
        /// 创建Action执行器
        /// </summary>
        /// <param name="actionDescriptor">Action描述</param>
        /// <returns></returns>
        public ApiActionInvoker CreateActionInvoker(ApiActionDescriptor actionDescriptor)
        {
            var actionInvoker = this.CreateDefaultActionInvoker(actionDescriptor);
            if (actionDescriptor.Return.ReturnType.IsInheritFrom<Task>() == false)
            {
                if (actionInvoker is IITaskReturnConvertable convertable)
                {
                    actionInvoker = convertable.ToITaskReturnActionInvoker();
                }
            }
            return actionInvoker;
        }

        /// <summary>
        /// 创建DefaultApiActionInvoker类型或其子类型的实例
        /// </summary>
        /// <param name="actionDescriptor">Action描述</param>
        /// <returns></returns>
        protected virtual ApiActionInvoker CreateDefaultActionInvoker(ApiActionDescriptor actionDescriptor)
        {
            var resultType = actionDescriptor.Return.DataType.Type;
            var invokerType = typeof(DefaultApiActionInvoker<>).MakeGenericType(resultType);
            return invokerType.CreateInstance<ApiActionInvoker>(actionDescriptor);
        }
    }
}
