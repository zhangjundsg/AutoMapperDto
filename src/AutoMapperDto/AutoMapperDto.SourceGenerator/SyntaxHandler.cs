using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoMapperDto.SourceGenerator;

internal sealed class SyntaxHandler
{
    public bool IsTargetNode(SyntaxNode syntaxNode)
    {
        return syntaxNode is TypeDeclarationSyntax node &&
                node.AttributeLists.Any(attr => attr.Attributes.Any(a => a.Name.ToString().Contains("Mapper")));
    }
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
    public string? GetGenericTypeFromMapperAttribute(AttributeSyntax attribute)
    {
        if (attribute.Name is GenericNameSyntax genericNameSyntax &&
            genericNameSyntax.TypeArgumentList.Arguments.Count == 1)
        {
            return genericNameSyntax.TypeArgumentList.Arguments[0].ToString();
        }
        return default;
    }
    public IEnumerable<IPropertySymbol> GetNonPrivateProperties(Compilation compilation, string typeName)
    {
        // 根据类型名获取类型符号
        var typeSymbol = compilation.GetSymbolsWithName(typeName, SymbolFilter.Type).FirstOrDefault() as ITypeSymbol;

        if (typeSymbol == null)
            return Enumerable.Empty<IPropertySymbol>();

        // 筛选非私有属性
        return typeSymbol.GetMembers()
            .OfType<IPropertySymbol>(); // 只获取属性
            //.Where(prop => prop.DeclaredAccessibility != Accessibility.Private); // 筛选非私有
    }
}