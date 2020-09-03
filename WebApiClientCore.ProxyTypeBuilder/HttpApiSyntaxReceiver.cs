using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.ProxyTypeBuilder
{

    class HttpApiSyntaxReceiver : ISyntaxReceiver
    {
        private readonly List<InterfaceDeclarationSyntax> interfaceList = new List<InterfaceDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InterfaceDeclarationSyntax syntax)
            {
                this.interfaceList.Add(syntax);
            }
        }

        public IEnumerable<INamedTypeSymbol> GetHttpApiTypes(Compilation compilation)
        {
            const string ihttpApiTypeName = "WebApiClientCore.IHttpApi";
            var iHttpApiType = compilation.GetTypeByMetadataName(ihttpApiTypeName);
            if (iHttpApiType == null)
            {
                yield break;
            }

            foreach (var @interface in this.interfaceList)
            {
                var model = compilation.GetSemanticModel(@interface.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(@interface);
                if (symbol != null)
                {
                    if (symbol.AllInterfaces.Any(item => item.Equals(iHttpApiType)))
                    {
                        yield return symbol;
                    }
                }
            }
        }
    }
}