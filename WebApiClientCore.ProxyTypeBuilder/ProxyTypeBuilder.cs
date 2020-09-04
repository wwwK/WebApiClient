using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApiClientCore.ProxyTypeBuilder
{
    class ProxyTypeBuilder
    {
        private readonly INamedTypeSymbol httpApi;

        public IEnumerable<string> Usings
        {
            get
            {
                yield return "using System;";
                yield return "using WebApiClientCore;";
            }
        }

        public string Namespace => this.httpApi.ContainingNamespace.ToString();

        public string BaseInterface => this.httpApi.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        public string Name => $"____{this.httpApi.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}";

        public string CtorName => $"____{this.httpApi.Name}";

        public ProxyTypeBuilder(INamedTypeSymbol httpApi)
        {
            this.httpApi = httpApi;
        }

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

        public SourceText ToSourceText()
        {
            var code = this.ToString();
            return SourceText.From(code, Encoding.UTF8);
        }
    }
}
