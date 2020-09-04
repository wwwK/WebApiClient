using Microsoft.CodeAnalysis;

namespace WebApiClientCore.ProxyTypeBuilder
{
    [Generator]
    public class ProxyTypeGenerator : ISourceGenerator
    {
        public void Initialize(InitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new HttpApiSyntaxReceiver());
        }

        public void Execute(SourceGeneratorContext context)
        {
            if (context.SyntaxReceiver is HttpApiSyntaxReceiver receiver)
            {
                foreach (var httpApi in receiver.GetHttpApiTypes(context.Compilation))
                {
                    var builder = new ProxyTypeBuilder(httpApi);
                    context.AddSource(builder.CtorName, builder.ToSourceText());
                }
            }
        }
    }
}
