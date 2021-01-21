﻿using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Internals.Utilities;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示THttpApi的实例Emit创建器
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class HttpApiEmitActivator<THttpApi> : HttpApiActivator<THttpApi>
    {
        /// <summary>
        /// IApiActionInterceptor的Intercept方法
        /// </summary>
        private static readonly MethodInfo interceptMethod = typeof(IApiActionInterceptor).GetMethod(nameof(IApiActionInterceptor.Intercept)) ?? throw new MissingMethodException(nameof(IApiActionInterceptor.Intercept));

        /// <summary>
        /// 代理类型的构造器的参数类型
        /// (IApiActionInterceptor interceptor,ApiActionInvoker[] actionInvokers)
        /// </summary>
        private static readonly Type[] proxyTypeCtorArgTypes = new Type[] { typeof(IApiActionInterceptor), typeof(ApiActionInvoker[]) };


        /// <summary>
        /// THttpApi的实例Emit创建器
        /// </summary>
        /// <param name="apiActionDescriptorProvider"></param>
        /// <param name="actionInvokerProvider"></param>
        public HttpApiEmitActivator(IApiActionDescriptorProvider apiActionDescriptorProvider, IApiActionInvokerProvider actionInvokerProvider)
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
            var proxyType = BuildProxyType(typeof(THttpApi), this.ApiMethods);
            return Lambda.CreateCtorFunc<IApiActionInterceptor, ApiActionInvoker[], THttpApi>(proxyType);
        }

        /// <summary>
        /// 创建IHttpApi代理类的类型
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="apiMethods">接口的方法</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        private static Type BuildProxyType(Type interfaceType, MethodInfo[] apiMethods)
        {
            // 接口的实现在动态程序集里，所以接口必须为public修饰才可以创建代理类并实现此接口            
            if (interfaceType.IsVisible == false)
            {
                var message = Resx.required_PublicInterface.Format(interfaceType);
                throw new NotSupportedException(message);
            }

            var moduleName = Guid.NewGuid().ToString();
            var assemblyName = new AssemblyName(Guid.NewGuid().ToString());

            var module = AssemblyBuilder
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName);

            var typeName = interfaceType.FullName ?? Guid.NewGuid().ToString();
            var builder = module.DefineType(typeName, System.Reflection.TypeAttributes.Class);
            builder.AddInterfaceImplementation(interfaceType);

            var fieldActionInterceptor = BuildField(builder, "<>actionInterceptor", typeof(IApiActionInterceptor));
            var fieldActionInvokers = BuildField(builder, "<>actionInvokers", typeof(ApiActionInvoker[]));

            BuildCtor(builder, fieldActionInterceptor, fieldActionInvokers);
            BuildMethods(builder, apiMethods, fieldActionInterceptor, fieldActionInvokers);

            var proxyType = builder.CreateType();
            return proxyType ?? throw new ProxyTypeCreateException(interfaceType);
        }

        /// <summary>
        /// 生成代理类型的字段
        /// </summary>
        /// <param name="builder">类型生成器</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldType">字段类型</param>
        /// <returns></returns>
        private static FieldBuilder BuildField(TypeBuilder builder, string fieldName, Type fieldType)
        {
            const FieldAttributes filedAttribute = FieldAttributes.Private | FieldAttributes.InitOnly;
            return builder.DefineField(fieldName, fieldType, filedAttribute);
        }

        /// <summary>
        /// 生成代理类型的构造器
        /// </summary>
        /// <param name="builder">类型生成器</param>
        /// <param name="fieldActionInterceptor">拦截器字段</param>
        /// <param name="fieldActionInvokers">action执行器字段</param> 
        /// <returns></returns>
        private static void BuildCtor(TypeBuilder builder, FieldBuilder fieldActionInterceptor, FieldBuilder fieldActionInvokers)
        {
            // .ctor(IApiActionInterceptor actionInterceptor, ApiActionInvoker[] actionInvokers)
            var ctor = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, proxyTypeCtorArgTypes);

            var il = ctor.GetILGenerator();

            // this.actionInterceptor = 第一个参数
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldActionInterceptor);

            // this.actionInvokers = 第二个参数
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, fieldActionInvokers);

            il.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// 生成代理类型的接口实现方法
        /// </summary>
        /// <param name="builder">类型生成器</param>
        /// <param name="actionMethods">接口的方法</param>
        /// <param name="fieldActionInterceptor">拦截器字段</param>
        /// <param name="fieldActionInvokers">action执行器字段</param> 
        private static void BuildMethods(TypeBuilder builder, MethodInfo[] actionMethods, FieldBuilder fieldActionInterceptor, FieldBuilder fieldActionInvokers)
        {
            const MethodAttributes implementAttribute = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.HideBySig;

            for (var i = 0; i < actionMethods.Length; i++)
            {
                var actionMethod = actionMethods[i];
                var actionParameters = actionMethod.GetParameters();
                var parameterTypes = actionParameters.Select(p => p.ParameterType).ToArray();

                var iL = builder
                    .DefineMethod(actionMethod.Name, implementAttribute, CallingConventions.Standard, actionMethod.ReturnType, parameterTypes)
                    .GetILGenerator();

                // this.actionInterceptor
                iL.Emit(OpCodes.Ldarg_0);
                iL.Emit(OpCodes.Ldfld, fieldActionInterceptor);

                // this.actionInvokers[i]
                iL.Emit(OpCodes.Ldarg_0);
                iL.Emit(OpCodes.Ldfld, fieldActionInvokers);
                iL.Emit(OpCodes.Ldc_I4, i);
                iL.Emit(OpCodes.Ldelem_Ref);

                // var arguments = new object[parameters.Length]
                var arguments = iL.DeclareLocal(typeof(object[]));
                iL.Emit(OpCodes.Ldc_I4, actionParameters.Length);
                iL.Emit(OpCodes.Newarr, typeof(object));
                iL.Emit(OpCodes.Stloc, arguments);

                for (var j = 0; j < actionParameters.Length; j++)
                {
                    iL.Emit(OpCodes.Ldloc, arguments);
                    iL.Emit(OpCodes.Ldc_I4, j);
                    iL.Emit(OpCodes.Ldarg, j + 1);

                    var parameterType = parameterTypes[j];
                    if (parameterType.IsValueType || parameterType.IsGenericParameter)
                    {
                        iL.Emit(OpCodes.Box, parameterType);
                    }
                    iL.Emit(OpCodes.Stelem_Ref);
                }

                // 加载arguments参数
                iL.Emit(OpCodes.Ldloc, arguments);

                // Intercep(actionInvoker, arguments)
                iL.Emit(OpCodes.Callvirt, interceptMethod);

                if (actionMethod.ReturnType == typeof(void))
                {
                    iL.Emit(OpCodes.Pop);
                }

                iL.Emit(OpCodes.Castclass, actionMethod.ReturnType);
                iL.Emit(OpCodes.Ret);
            }
        }
    }
}