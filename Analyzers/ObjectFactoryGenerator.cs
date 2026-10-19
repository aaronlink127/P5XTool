using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Analyzer
{
    [Generator]
    public class ObjectFactoryGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
                .Where(static c => c is not null);

            var collected =
                context.CompilationProvider.Combine(classDeclarations.Collect());

            context.RegisterSourceOutput(collected, static (spc, source) => Execute(source.Left, source.Right, spc));
        }
        private static void Execute(
            Compilation compilation,
            ImmutableArray<ClassDeclarationSyntax> classes,
            SourceProductionContext context)
        {
            var baseType = compilation.GetTypeByMetadataName("UnityAssetLib.Object");
            if (baseType == null) return;
            if (!SymbolEqualityComparer.Default.Equals(
                baseType.ContainingAssembly,
                compilation.Assembly))
            {
                return;
            }
            var sb = new StringBuilder();
            sb.AppendLine($"#nullable enable");
            sb.AppendLine($"namespace UnityAssetLib;");

            sb.AppendLine("public static partial class ObjectFactory");
            sb.AppendLine("{");
            sb.AppendLine("    static partial void GenerateObjectMap()");
            sb.AppendLine("    {");
            sb.AppendLine("        _objMap = new() {");
            foreach (var classSyntax in classes)
            {
                var model = compilation.GetSemanticModel(classSyntax.SyntaxTree);
                if (model.GetDeclaredSymbol(classSyntax) is not INamedTypeSymbol type)
                    continue;

                if (type.Arity > 0)
                    continue;

                if (!DerivesFrom(type, baseType))
                    continue;

                sb.AppendLine(
                    $"""            ["{type.Name}"] = typeof({type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}),""");
            }
            sb.AppendLine("        };");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            context.AddSource($"ObjectMap_AutoObject.g.cs", sb.ToString());
        }
        private static bool DerivesFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
        {
            for (var t = type.BaseType; t is not null; t = t.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(t, baseType))
                    return true;
            }
            return false;
        }
    }
}
