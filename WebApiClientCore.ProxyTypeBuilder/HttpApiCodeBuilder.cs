using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApiClientCore.ProxyTypeBuilder
{
    /// <summary>
    /// HttpApi代码构建器
    /// </summary>
    class HttpApiCodeBuilder
    {
        /// <summary>
        /// 接口符号
        /// </summary>
        private readonly INamedTypeSymbol httpApi;

        /// <summary>
        /// 前缀
        /// </summary>
        private readonly string prefix;

        /// <summary>
        /// using
        /// </summary>
        public IEnumerable<string> Usings
        {
            get
            {
                yield return "using System;";
                yield return "using WebApiClientCore;";
            }
        }

        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace => this.httpApi.ContainingNamespace.ToString();

        /// <summary>
        /// 基础接口
        /// </summary>
        public string BaseInterface => this.httpApi.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        /// <summary>
        /// 类名
        /// </summary>
        public string Name => $"{prefix}{this.httpApi.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}";

        /// <summary>
        /// 构造器名
        /// </summary>
        public string CtorName => $"{prefix}{this.httpApi.Name}";

        /// <summary>
        /// HttpApi代码构建器
        /// </summary>
        /// <param name="httpApi"></param>
        /// <param name="prefix">类型前缀</param>
        public HttpApiCodeBuilder(INamedTypeSymbol httpApi, string prefix = "____")
        {
            this.httpApi = httpApi;
            this.prefix = prefix;
        }

        /// <summary>
        /// 转换为SourceText
        /// </summary>
        /// <returns></returns>
        public SourceText ToSourceText()
        {
            var code = this.ToString();
            return SourceText.From(code, Encoding.UTF8);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //System.Diagnostics.Debugger.Launch();
            var builder = new StringBuilder();
            foreach (var item in this.Usings)
            {
                builder.AppendLine(item);
            }
            builder.AppendLine($"namespace {this.Namespace}");
            builder.AppendLine("{");
            builder.AppendLine($"\tclass {this.Name}:{this.BaseInterface}");
            builder.AppendLine("\t{");
            builder.AppendLine("\t\tprivate readonly IActionInterceptor ____interceptor;");
            builder.AppendLine("\t\tprivate readonly IActionInvoker[] ____actionInvokers;");

            builder.AppendLine($"\t\tpublic {this.CtorName}(IActionInterceptor interceptor,IActionInvoker[] actionInvokers)");
            builder.AppendLine("\t\t{");
            builder.AppendLine("\t\t\tthis.____interceptor = interceptor;");
            builder.AppendLine("\t\t\tthis.____actionInvokers = actionInvokers;");
            builder.AppendLine("\t\t}");

            var index = 0;
            foreach (var member in this.httpApi.GetMembers())
            {
                if (member is IMethodSymbol method)
                {
                    var methodCode = BuildMethod(method, index);
                    builder.AppendLine(methodCode);
                    index += 1;
                }
            }

            builder.AppendLine("\t}");
            builder.AppendLine("}");
            return builder.ToString();
        }

        /// <summary>
        /// 构建方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string BuildMethod(IMethodSymbol method, int index)
        {
            var builder = new StringBuilder();
            var parameters = method.Parameters.Select(item => $"{item.Type} {item.Name}");
            var parameterNames = method.Parameters.Select(item => item.Name);

            builder.AppendLine($"\t\tpublic {method.ReturnType} {method.Name}( {string.Join(",", parameters)} )");
            builder.AppendLine("\t\t{");
            builder.AppendLine($"\t\t\tvar ____arguments = new object[] {{ {string.Join(",", parameterNames)} }};");
            builder.AppendLine($"\t\t\tvar ____actionInvoker = this.____actionInvokers[{index}];");
            builder.AppendLine("\t\t\tvar ____result = this.____interceptor.Intercept(____actionInvoker, ____arguments);");
            builder.AppendLine($"\t\t\treturn ({method.ReturnType})____result;");
            builder.AppendLine("\t\t}");
            return builder.ToString();
        }
    }
}
