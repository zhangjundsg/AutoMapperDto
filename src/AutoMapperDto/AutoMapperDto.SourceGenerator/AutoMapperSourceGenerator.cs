using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoMapperDto.SourceGenerator;

[Generator(firstLanguage: LanguageNames.CSharp)]
internal sealed class AutoMapperSourceGenerator : IIncrementalGenerator
{
    private const string _ignoreAttribute = "AutoMapperDto.Ignore";
    private const string _mapperAsAttribute = "AutoMapperDto.MapperAs";
    private const string _defaultNamespace = "AutoMapperDto.SourceGenerator";
    private INamedTypeSymbol? _ignoreAttributeSymbol;
    private INamedTypeSymbol? _mapperAsSymbol;
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
        // ��ǰ�ڵ���������ģ��
        var semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
        if (semanticModel == null)
            return default;

        // ��ȡ Mapper<T> ����
        var mapperAttribute = _syntaxHandler.GetMapperAttribute(syntax, semanticModel);
        if (mapperAttribute == null)
            return default;

        // ��ȡ����������
        var genericTypeName = _syntaxHandler.GetGenericTypeFromMapperAttribute(mapperAttribute, compilation);
        if (genericTypeName == null)
            return default;

        // ��ǰ�ڵ������Ϣ
        var typeSymbol = semanticModel.GetDeclaredSymbol(syntax);
        if (typeSymbol == null)
            return default;

        if (!syntax.Modifiers.Any(o => o.ValueText.Contains("partial")))
            return default;

        var nameSpace = _syntaxHandler.GetNamespace(syntax) ?? _defaultNamespace;
        var modifiers = string.Join(" ", syntax.Modifiers.Select(o => o.ValueText));
        var keyword = syntax.Keyword.ValueText;
        var className = syntax.Identifier.ValueText;

        //��ǰ������������
        var currentHasProperty = _syntaxHandler.GetNonPrivateProperties(compilation, typeSymbol.ToDisplayString());
        var ignoreAttributeSymbol = _ignoreAttributeSymbol ??= compilation.GetTypeByMetadataName(_ignoreAttribute);
        var mapperAsAttributeSymbol = _mapperAsSymbol ??= compilation.GetTypeByMetadataName(_mapperAsAttribute);

        var text = new StringBuilder();
        text.AppendLine($"namespace {nameSpace};");
        text.AppendLine($"{modifiers} {keyword} {className}");
        text.AppendLine("{");

        // ��ȡ T ���͵Ĺ������� �ų����Ignore���Ѵ���property
        var properties = _syntaxHandler.GetNonPrivateProperties(compilation, genericTypeName)
        .Where(p => p.DeclaredAccessibility == Accessibility.Public
        && !p.GetAttributes().Any(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, ignoreAttributeSymbol))
        && !currentHasProperty.Any(existing => existing.Name.Equals(p.Name)));

        var mapperDic = new Dictionary<string, string>();
        // �������
        foreach (var property in properties)
        {
            var mapperAsAttr = property.GetAttributes().FirstOrDefault(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, mapperAsAttributeSymbol));

            if ((string?)mapperAsAttr?.ConstructorArguments[0].Value is var name && name is not null)
                mapperDic.Add(property.Name, name);

            text.AppendLine($"   // < generate from=\"{typeSymbol.ToDisplayString()}.{property.Name}\" >");
            text.AppendLine($@"   public {property.Type.ToDisplayString()} {name ?? property.Name} {{ get; set; }}");
        }
        text.AppendLine("}");

        // ���ӳ��
        text.AppendLine(GenerateMapperExtensions(source: genericTypeName, target: className, properties, mapperDic));

        return text.ToString();
    }

    private string GenerateMapperExtensions(string source, string target, IEnumerable<IPropertySymbol> properties, Dictionary<string, string> mapperName)
    {
        var text = new StringBuilder();
        text.AppendLine("public static partial class MapperExtensions")
            .AppendLine("{")
            .AppendLine($"   public static {target} As{target}(this {source} @source)")
            .AppendLine("   {")
            .AppendLine($"       return new {target}()")
            .AppendLine("       {");

        foreach (var property in properties)
        {
            _ = mapperName.TryGetValue(property.Name, out string? name);
            text.AppendLine($"           {name ?? property.Name} = @source.{property.Name},");
        }

        text.AppendLine("       };")
            .AppendLine("   }")
            .AppendLine("}");

        return text.ToString();
    }
}