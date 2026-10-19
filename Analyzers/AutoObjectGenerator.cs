using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer
{
    class SymbolGenerator
    {
        public class StackHandle : IDisposable
        {
            SymbolGenerator generator;
            public StackHandle(SymbolGenerator generator)
            {
                this.generator = generator;
                generator.PushStackImpl();
            }
            public void Dispose()
            {
                generator.PopStack();
                GC.SuppressFinalize(this);
            }
        }
        public List<int> stack;
        public SymbolGenerator()
        {
            stack = [];
        }
        public string GetSymbol()
        {
            return $"_e{stack[^1]++}";
        }
        private void PushStackImpl()
        {
            int nextStack = stack.Count > 0 ? stack[^1] : 0;
            stack.Add(nextStack);
        }
        public StackHandle PushStack()
        {
            return new StackHandle(this);
        }
        public void PopStack()
        {
            stack.RemoveAt(stack.Count - 1);
        }
    }
    [Generator]
    public class AutoObjectGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) =>
                        s is ClassDeclarationSyntax cds &&
                        cds.Modifiers.Any(SyntaxKind.PartialKeyword),
                    transform: static (ctx, _) =>
                    {
                        var classSyntax = (ClassDeclarationSyntax)ctx.Node;
                        var symbol = ctx.SemanticModel.GetDeclaredSymbol(classSyntax);
                        if (symbol is null)
                            return null;

                        foreach (var attr in symbol.GetAttributes())
                        {
                            var name = attr.AttributeClass?.Name;
                            if (name is "AutoIgnore" or "AutoIgnoreAttribute")
                                return null;
                        }

                        for (var baseType = symbol.BaseType; baseType is not null; baseType = baseType.BaseType)
                        {
                            if (baseType.ToDisplayString() == "UnityAssetLib.Object")
                                return symbol;
                        }

                        return null;
                    })
                .Where(static s => s is not null)!;

            context.RegisterSourceOutput(classDeclarations, Generate);
        }
        static SymbolGenerator symbolGen = new SymbolGenerator();

        private static string GetKVConvert(ITypeSymbol kt, ITypeSymbol vt, string arg)
        {
            using var _ = symbolGen.PushStack();
            var arg1 = symbolGen.GetSymbol();
            var arg2 = symbolGen.GetSymbol();
            var kStr = GetConvertStr(kt, arg1) ?? $"({kt.ToDisplayString()}){arg1}";
            var vStr = GetConvertStr(vt, arg2) ?? $"({vt.ToDisplayString()}){arg2}";
            return $"GetPair<{kt.ToDisplayString()}, {vt.ToDisplayString()}>({arg}, {arg1}=>({kStr}), {arg2}=>({vStr}), _s)";
        }

        private static string GetConvertStr(ITypeSymbol type, string arg)
        {

            if (type.SpecialType == SpecialType.System_String)
            {
                return $"Encoding.UTF8.GetString((byte[]){arg})";
            }

            ITypeSymbol? elementType = null;
            if (IsSubclassOfObject(type))
            {
                return $"Create<{type.ToDisplayString()}>({arg}, _s)";
            }
            var nts = type as INamedTypeSymbol;
            var isArray = type is IArrayTypeSymbol ats;
            var isList = !isArray &&
                         nts != null && nts.Name == "List" && nts.TypeArguments.Length == 1;
            if (isArray) elementType = ((IArrayTypeSymbol)type).ElementType;
            if (isList) elementType = ((INamedTypeSymbol)type).TypeArguments[0];

            if (elementType != null)
            {
                using var _ = symbolGen.PushStack();
                var uid = symbolGen.GetSymbol();
                return $"GetList<{elementType.ToDisplayString()}>({arg}, {uid}=>({GetConvertStr(elementType, uid)}), _s){(isList ? ".ToList()" : "")}";
            }
            else if (nts != null)
            {
                if (type.Name == "Dictionary")
                {
                    using var _ = symbolGen.PushStack();
                    var kt = nts.TypeArguments[0];
                    var vt = nts.TypeArguments[1];
                    var uid = symbolGen.GetSymbol();
                    return $"GetList<KeyValuePair<{kt.ToDisplayString()}, {vt.ToDisplayString()}>>({arg}, {uid}=>({GetKVConvert(kt, vt, uid)}), _s).ToDictionary()";
                }
                else if (nts.Name == "KeyValuePair")
                {
                    var kt = nts.TypeArguments[0];
                    var vt = nts.TypeArguments[1];
                    return GetKVConvert(kt, vt, arg);
                }
            }
            if (type.SpecialType == SpecialType.System_Boolean)
            {
                var uid = symbolGen.GetSymbol();
                return $"{arg} is byte {uid} ? {uid} == 0 : (bool){arg}";
            }
            return $"({type.ToDisplayString()}){arg}";
        }
        private static void Generate(SourceProductionContext context, ISymbol? iSymbol)
        {
            using var _ = symbolGen.PushStack();
            var typeSymbol = iSymbol as INamedTypeSymbol;
            if (typeSymbol == null) return;
            var ns = typeSymbol.ContainingNamespace.IsGlobalNamespace
                ? null : typeSymbol.ContainingNamespace.ToDisplayString();

            var sb = new StringBuilder();
            sb.AppendLine($"#nullable enable");
            sb.AppendLine($"using System.Linq;");
            sb.AppendLine($"using System.Collections.Generic;");
            sb.AppendLine($"using System.Text;");
            if (ns != null)
                sb.AppendLine($"namespace {ns};");

            var baseType = typeSymbol.BaseType;

            var chain = new List<INamedTypeSymbol>();
            for (var t = typeSymbol; t != null; t = t.ContainingType)
                chain.Add(t);
            chain.Reverse();

            for (int i = 0; i < chain.Count; i++)
            {
                var t = chain[i];
                var baseClause = (i == chain.Count - 1 && baseType != null) ? $" : {baseType.ToDisplayString()}" : "";
                sb.AppendLine($"partial class {t.Name}{baseClause}");
                sb.AppendLine("{");
            }

            sb.AppendLine("    public override void LoadDataFrom_Generated(Dictionary<string, object?> data, UnityAssetLib.SerializedFile _s)");
            sb.AppendLine("    {");
            sb.AppendLine("        base.LoadDataFrom_Generated(data, _s);");

            foreach (var field in GetAllFields(typeSymbol))
            {
                if (field.IsStatic)
                    continue;

                var ignoreAttribute = field.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "AutoIgnoreAttribute");
                if (ignoreAttribute != null)
                {
                    continue;
                }
                var loadNameAttr = field.GetAttributes()
                    .FirstOrDefault(a => a.AttributeClass?.Name == "AutoFieldNameAttribute");
                var optionalAttribute = field.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "AutoOptionalAttribute");
                var conditionAttrs = field.GetAttributes()
                    .Where(a => a.AttributeClass != null && IsConditionAttribute(a.AttributeClass))
                    .ToList();
                var keyName = loadNameAttr != null
                    ? loadNameAttr.ConstructorArguments[0].Value?.ToString()
                    : field.Name;

                var fieldType = field.Type;
                var fieldName = field.Name;

                var valueStr = GetConvertStr(fieldType, "data[\"" + keyName + "\"]!");
                List<string> conditions = []; 
                if (optionalAttribute != null)
                {
                    sb.AppendLine($"        //{optionalAttribute.AttributeClass!.Name}");
                    conditions.Add($"data.ContainsKey(\"{keyName}\")");
                }
                foreach (var attr in conditionAttrs)
                {
                    sb.AppendLine($"        //{attr.AttributeClass!.Name}");
                    var args = string.Join(", ", attr.ConstructorArguments.Select(a => a.Value?.ToString() ?? "null"));
                    var attrType = attr.AttributeClass!;
                    conditions.Add($"{attrType.ToDisplayString()}.Evaluate(data, _s, {args})");
                }
                //sb.AppendLine($"        try {{ {fieldName} = {valueStr}; }} catch {{ }}");
                if (conditions.Count > 0)
                {
                    sb.AppendLine($"        if ({string.Join(" && ", conditions)})");
                    sb.AppendLine($"            {fieldName} = {valueStr};");
                }
                else
                {
                    sb.AppendLine($"        {fieldName} = {valueStr};");
                }
            }

            sb.AppendLine("    }");

            for (int i = 0; i < chain.Count; i++)
                sb.AppendLine("}");

            var fileName = $"{string.Join("_", chain.Select(t => t.Name))}_AutoObject.g.cs";
            context.AddSource(fileName, sb.ToString());
        }

        private static IEnumerable<IFieldSymbol> GetAllFields(INamedTypeSymbol type)
        {
            foreach (var field in type.GetMembers().OfType<IFieldSymbol>())
            {
                if (!field.IsStatic)
                    yield return field;
            }
        }
        private static bool IsConditionAttribute(ITypeSymbol? type)
        {
            while (type != null)
            {
                if (type.Name == "AutoObjectConditionAttribute")
                    return true;
                type = (type as INamedTypeSymbol)?.BaseType;
            }
            return false;
        }
        private static bool IsSubclassOfObject(ITypeSymbol? type)
        {
            while (type != null)
            {
                if (type.Name == "Object" && type.ContainingNamespace.ToDisplayString() != "System")
                    return true;
                type = (type as INamedTypeSymbol)?.BaseType;
            }
            return false;
        }
    }
}
