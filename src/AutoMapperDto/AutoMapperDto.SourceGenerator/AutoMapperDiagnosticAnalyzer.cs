using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace AutoMapperDto.SourceGenerator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AutoMapperDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        private readonly static DiagnosticDescriptor _descriptor = new(
                        id: "Tips001",
                        title: "Class or Record 需要 partial 关键字",
                        messageFormat: "The class or record '{0}' 在使用[Mapper<T>]映射实体时需要[partial]关键字",
                        category: "MapperGenerator",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [_descriptor];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.RecordDeclaration);
        }
        private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            // 检查是否包含 Mapper<T> 属性
            var hasMapperAttribute = typeDeclaration.AttributeLists
                .SelectMany(attrList => attrList.Attributes)
                .Any(attr => attr.Name.ToString().StartsWith("Mapper", StringComparison.Ordinal));

            if (!hasMapperAttribute)
                return;

            // 检查是否是 partial
            if (!typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                var diagnostic = Diagnostic.Create(
                    _descriptor,
                    typeDeclaration.Identifier.GetLocation(),
                    typeDeclaration.Identifier.Text);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
