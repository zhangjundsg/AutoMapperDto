using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoMapperDto.SourceGenerator;

internal sealed class SyntaxReceiver: ISyntaxReceiver
{
    public readonly HashSet<TypeDeclarationSyntax?> _syntaxClasses = [];
    
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax or RecordDeclarationSyntax)
            _syntaxClasses.Add(syntaxNode as TypeDeclarationSyntax);
    }
}