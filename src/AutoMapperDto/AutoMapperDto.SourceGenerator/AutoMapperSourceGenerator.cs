using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoMapperDto.SourceGenerator;

[Generator(LanguageNames.CSharp)]
internal sealed class AutoMapperSourceGenerator : IIncrementalGenerator
{
    private readonly static SyntaxHandler _syntaxHandler = new();
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //Debugger.Launch();
        var provider = context.SyntaxProvider
        .CreateSyntaxProvider(
            predicate: static (node, _) => _syntaxHandler.IsTargetNode(node),
            transform: static (ctx, _) => ctx.Node as TypeDeclarationSyntax)
        .Where(static x => x is not null);

        context.RegisterSourceOutput(provider.Combine(context.CompilationProvider), (ctx, source) =>
        {
            var (typeDeclaration, compilation) = source;


            if (!typeDeclaration!.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                var diagnostic = Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "Tips001",
                        title: "Class/Record must be partial",
                        messageFormat: "The class or record '{0}' using [Mapper<T>] must be declared as partial.",
                        category: "MapperGenerator",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    typeDeclaration.GetLocation(),
                    typeDeclaration.Identifier.Text);

                ctx.ReportDiagnostic(diagnostic);
            }

            if (typeDeclaration is TypeDeclarationSyntax typeNode)
            {
                var codeSource = GenerateCode(typeNode, compilation);

                if (!string.IsNullOrEmpty(codeSource))
                    ctx.AddSource($"{typeNode.Identifier.ValueText}_g.cs", codeSource!);
            }
        });

    }

    private string? GenerateCode(TypeDeclarationSyntax syntax, Compilation compilation)
    {
        // ��ȡ Mapper<T> ����
        var mapperAttribute = _syntaxHandler.GetMapperAttribute(syntax);
        
        if (mapperAttribute == null)
            return default;
        
        // ��ȡ����������
        var genericTypeName = _syntaxHandler.GetGenericTypeFromMapperAttribute(mapperAttribute);

        if (genericTypeName == null)
            return default;

        var nameSpace = _syntaxHandler.GetNamespace(syntax);
        var modifiers = string.Join(" ", syntax.Modifiers.Select(o => o.ValueText.ToLower()));
        var keyword = syntax.Keyword.ValueText;
        var className = syntax.Identifier.ValueText;

        var text = new StringBuilder();
        text.AppendLine($"namespace {nameSpace ?? "AutoMapperDto.SourceGenerator"};");
        text.AppendLine($"{modifiers} {keyword} {className}");
        text.AppendLine("{");

        // ��ȡ T ���͵Ĺ�������
        foreach (var property in _syntaxHandler.GetNonPrivateProperties(compilation, genericTypeName).Where(o=>o.DeclaredAccessibility == Accessibility.Public ))
        {
            text.AppendLine($@"   public {property.Type.ToDisplayString()} {property.Name} {{ get; set; }}");
        }
        text.AppendLine("}");

        return text.ToString();
    }
}