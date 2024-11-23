using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoMapperDto.SourceGenerator
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AutoMapperCodeFixProvider))]
    [Shared]
    internal class AutoMapperCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ["Tips001"];

        public sealed override FixAllProvider? GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // 找到目标 TypeDeclarationSyntax
            if (root.FindNode(diagnosticSpan) is not TypeDeclarationSyntax typeDeclaration)
                return;

            // 注册修复操作
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "添加 'partial' 关键字",
                    createChangedDocument: c => AddPartialModifierAsync(context.Document, typeDeclaration, c),
                    equivalenceKey: "AddPartialModifier"),
                diagnostic);
        }
        private async Task<Document> AddPartialModifierAsync(Document document, TypeDeclarationSyntax typeDeclaration, CancellationToken cancellationToken)
        {
            // 添加 partial 修饰符
            var partialToken = SyntaxFactory.Token(SyntaxKind.PartialKeyword);
            var newModifiers = typeDeclaration.Modifiers.Add(partialToken);

            var newTypeDeclaration = typeDeclaration.WithModifiers(newModifiers);
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(typeDeclaration, newTypeDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
