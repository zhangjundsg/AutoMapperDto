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
    /// <summary>
    /// 是否目标节点
    /// </summary>
    /// <param name="syntaxNode"></param>
    /// <returns></returns>
    public bool IsTargetNode(SyntaxNode syntaxNode)
    {
        return syntaxNode is TypeDeclarationSyntax node &&
                node.AttributeLists.Any(attr => attr.Attributes.Any(a => a.Name.ToString().Contains("Mapper")));
    }
    /// <summary>
    /// 获取映射Attribute<T>
    /// </summary>
    /// <param name="typeNode"></param>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public AttributeSyntax? GetMapperAttribute(TypeDeclarationSyntax typeNode, string attributeName = "Mapper")
    {
        foreach (var attributeList in typeNode.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var attrName = attribute.Name.ToString();

                if (attrName.StartsWith(attributeName, StringComparison.Ordinal) && attrName.Contains(attributeName))
                {
                    return attribute;
                }
            }
        }
        return default;
    }
    /// <summary>
    /// 获取映射Mapper<T> T类型名称
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public string? GetGenericTypeFromMapperAttribute(AttributeSyntax attribute)
    {
        if (attribute.Name is GenericNameSyntax genericNameSyntax &&
            genericNameSyntax.TypeArgumentList.Arguments.Count == 1)
        {
            return genericNameSyntax.TypeArgumentList.Arguments[0].ToString();
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
        if (compilation.GetSymbolsWithName(typeName, SymbolFilter.Type).FirstOrDefault() is not ITypeSymbol typeSymbol)
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