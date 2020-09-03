using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WebApiClientCore.ProxyTypeBuilder
{
    class ProxyTypeBuilder
    {
        private readonly INamedTypeSymbol httpApi;

        public string HintName => this.httpApi.Name;

        public ProxyTypeBuilder(INamedTypeSymbol httpApi)
        {
            this.httpApi = httpApi;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            // System.Diagnostics.Debugger.Launch();
            return builder.ToString();
        }

        public SourceText ToSourceText()
        {
            return SourceText.From(this.ToString(), Encoding.UTF8);
        }
    }
}
