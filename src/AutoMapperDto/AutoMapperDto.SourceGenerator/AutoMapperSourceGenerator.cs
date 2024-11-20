using System;
using System.Diagnostics;
using System.Linq;
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
        Debugger.Launch();
        var provider = context.SyntaxProvider
        .CreateSyntaxProvider(
            predicate: static (node, _) => _syntaxHandler.IsTargetNode(node),
            transform: static (ctx, _) => ctx.Node as TypeDeclarationSyntax)
        .Where(static x => x is not null);

        context.RegisterSourceOutput(provider.Combine(context.CompilationProvider), (ctx, source) =>
        {
            var (typeDeclaration, compilation) = source;

            if (typeDeclaration is TypeDeclarationSyntax typeNode && GenerateCode(typeNode, compilation) is var codeSource && !string.IsNullOrEmpty(codeSource))
            {
                var name = "";
                ctx.AddSource(name, codeSource!);
            }
        });

    }

    private string? GenerateCode(TypeDeclarationSyntax syntax, Compilation compilation)
    {
        // 获取 Mapper<T> 属性
        var mapperAttribute = _syntaxHandler.GetMapperAttribute(syntax);

        if (mapperAttribute == null)
            return default;

        // 提取泛型类型名
        var genericTypeName = _syntaxHandler.GetGenericTypeFromMapperAttribute(mapperAttribute);

        if (genericTypeName == null)
            return default;

        // 获取 T 类型的非私有属性
        //var properties = _syntaxHandler.GetNonPrivateProperties(compilation, genericTypeName);
        foreach (var item in _syntaxHandler.GetNonPrivateProperties(compilation, genericTypeName))
        {
            var a = item.Name;
            var t = item.Type.ToDisplayString();
        }
        return default;
    }
}