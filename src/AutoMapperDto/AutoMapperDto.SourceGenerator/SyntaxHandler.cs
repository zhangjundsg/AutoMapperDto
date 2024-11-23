using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoMapperDto.SourceGenerator;

/// <summary>
/// 语法处理
/// </summary>

internal sealed class SyntaxHandler
{
    private const string _attributeNamespace = "AutoMapperDto";
    private const string _attrubuteName = "Mapper";
    /// <summary>
    /// 是否目标节点
    /// </summary>
    /// <param name="syntaxNode"></param>
    /// <returns></returns>
    public bool IsTargetNode(SyntaxNode syntaxNode)
    {
        return syntaxNode is TypeDeclarationSyntax node &&
                node.AttributeLists.Any(attr => attr.Attributes.Any(a => a.Name.ToString().Contains(_attrubuteName)));
    }
    /// <summary>
    /// 获取映射Attribute<T>
    /// </summary>
    /// <param name="typeNode"></param>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public AttributeSyntax? GetMapperAttribute(TypeDeclarationSyntax typeNode, Compilation compilation)
    {
        foreach (var attributeList in typeNode.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var attrName = attribute.Name.ToString();
                var attrNamespace = compilation.GetSemanticModel(attribute.SyntaxTree)?.GetSymbolInfo(attribute).Symbol?.ContainingNamespace.ToDisplayString() ?? "";

                if (attrNamespace.Equals(_attributeNamespace) && attrName.StartsWith(_attrubuteName, StringComparison.Ordinal))
                    return attribute;
            }
        }
        return default;
    }
    /// <summary>
    /// 获取映射Mapper<T> T类型名称
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public string? GetGenericTypeFromMapperAttribute(AttributeSyntax attribute, Compilation compilation)
    {
        if (attribute.Name is GenericNameSyntax genericNameSyntax &&
            genericNameSyntax.TypeArgumentList.Arguments.Count == 1)
        {
            var typeSyntax = genericNameSyntax.TypeArgumentList.Arguments[0];
            var semanticModel = compilation.GetSemanticModel(typeSyntax.SyntaxTree);

            if (semanticModel.GetSymbolInfo(typeSyntax).Symbol is INamedTypeSymbol typeSymbol)
                return typeSymbol.ToDisplayString();
        }
        return default;
    }
    /// <summary>
    /// 获取类型属性
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public IEnumerable<IPropertySymbol> GetNonPrivateProperties(Compilation compilation, string typeName)
    {
        // 根据类型名获取类型符号
        if (compilation.GetTypeByMetadataName(typeName) is not ITypeSymbol typeSymbol)
            return [];

        // 筛选属性
        return typeSymbol.GetMembers().OfType<IPropertySymbol>();
    }
    /// <summary>
    /// 获取namespace
    /// </summary>
    /// <param name="typeDeclaration"></param>
    /// <returns></returns>
    public string? GetNamespace(TypeDeclarationSyntax typeDeclaration)
    {
        var fileScopedNamespaceNode = typeDeclaration.AncestorsAndSelf()
        .OfType<FileScopedNamespaceDeclarationSyntax>()
        .FirstOrDefault();

        if (fileScopedNamespaceNode != null)
            return fileScopedNamespaceNode.Name.ToString();

        return default;
    }
}