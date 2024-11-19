using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace AutoMapperDto.SourceGenerator;

[Generator]
internal sealed class AutoMapperSourceGenerator : ISourceGenerator
{
    private const string MapperAttribute = "AutoMapperDto.Mapper";

    public void Initialize(GeneratorInitializationContext context) => 
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

    public void Execute(GeneratorExecutionContext context)
    {
        Debugger.Launch();
        var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
        foreach (var item in receiver._syntaxClasses)
        {
            
        }
    }

    private string? SourceGenerator(Compilation compilation)
    {
        return default;
    }
}