using Microsoft.CodeAnalysis;

namespace AutoMapperDto.SourceGenerator;

public sealed class AutoMapperSourceGenerator: ISourceGenerator
{
    private const string MapperAttribute = "AutoMapper.MapAttribute";
    
    public void Initialize(GeneratorInitializationContext context) =>
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

    public void Execute(GeneratorExecutionContext context)
    {
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