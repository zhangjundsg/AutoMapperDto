using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoMapperDto.SourceGenerator;

/// <summary>
/// �﷨����
/// </summary>

internal sealed class SyntaxHandler
{
    private const string _attributeNamespace = "AutoMapperDto";
    private const string _attrubuteName = "Mapper";
    /// <summary>
    /// �Ƿ�Ŀ��ڵ�
    /// </summary>
    /// <param name="syntaxNode"></param>
    /// <returns></returns>
    public bool IsTargetNode(SyntaxNode syntaxNode)
    {
        return syntaxNode is TypeDeclarationSyntax node &&
                node.AttributeLists.Any(attr => attr.Attributes.Any(a => a.Name.ToString().Contains(_attrubuteName)));
    }
    /// <summary>
    /// ��ȡӳ��Attribute<T>
    /// </summary>
    /// <param name="typeNode"></param>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public AttributeSyntax? GetMapperAttribute(TypeDeclarationSyntax typeNode, SemanticModel semanticModel)
    {
        // ������������
        foreach (var attribute in typeNode.AttributeLists.SelectMany(attrList => attrList.Attributes))
        {
            // ��ȡ����
            if (semanticModel.GetSymbolInfo(attribute).Symbol is var symbol && symbol is null)
                continue;

            // ��������ռ�������Ƿ�ƥ��
            if (symbol.ContainingNamespace.ToDisplayString() == _attributeNamespace &&
                symbol.ContainingSymbol.Name == _attrubuteName)
            {
                return attribute;
            }
        }
        return default;
    }
    /// <summary>
    /// ��ȡӳ��Mapper<T> T��������
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
    /// ��ȡ��������
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public IEnumerable<IPropertySymbol> GetNonPrivateProperties(Compilation compilation, string typeName)
    {
        // ������������ȡ���ͷ���
        if (compilation.GetTypeByMetadataName(typeName) is not ITypeSymbol typeSymbol)
            return [];

        // ɸѡ����
        return typeSymbol.GetMembers().OfType<IPropertySymbol>();
    }
    /// <summary>
    /// ��ȡnamespace
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