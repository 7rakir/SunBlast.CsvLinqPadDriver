using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CsvLinqPadDriver
{
    internal class SyntaxTreeBuilder
    {
        private readonly ContextClass contextClass;
        private readonly DataClasses dataClasses = new();
        private readonly ExtensionClass extensionClass = new();

        public SyntaxTreeBuilder(string className)
        {
            contextClass = new ContextClass(className);
        }

        public void AddModel(FileModel fileModel)
        {
            contextClass.AddProperty(fileModel);
            dataClasses.AddClass(fileModel);
            extensionClass.EnsureExtension(fileModel);
        }

        public SyntaxTree Build(string nameSpace)
        {
            var members = dataClasses.Declarations.Prepend(contextClass.Declaration).Append(extensionClass.Declaration).ToArray();

            var namespaceDeclaration = NamespaceDeclaration(ParseName(nameSpace)).AddMembers(members);

            var usingList = GetUsingDirectives().ToArray();

            return CompilationUnit()
                .AddUsings(usingList)
                .AddMembers(namespaceDeclaration)
                .SyntaxTree;
        }

        private static IEnumerable<UsingDirectiveSyntax> GetUsingDirectives()
        {
            return new[]
            {
                "System",
                "System.Collections.Generic",
                "System.IO",
                "System.Linq",
                "CsvLinqPadDriver.Csv",
                "CsvLinqPadDriver.UserExtensions.Dynamic"
            }.Select(name => UsingDirective(ParseName(name)));
        }

        private class ContextClass
        {
            public ClassDeclarationSyntax Declaration { get; private set; }
            
            public ContextClass(string name)
            {
                Declaration = ClassDeclaration(name).AddModifiers(Token(SyntaxKind.PublicKeyword));
            }
            
            public void AddProperty(FileModel model)
            {
                var property = ParseMemberDeclaration(
                    $@"public IEnumerable<{model.ClassName}> {model.ClassName} => CsvReader.ReadFile<{model.ClassName}>(@""{model.FilePath}"");");

                Declaration = Declaration.AddMembers(property!);
            }
        }

        private class DataClasses
        {
            private readonly List<ClassDeclarationSyntax> declarations = new();

            public IEnumerable<ClassDeclarationSyntax> Declarations => declarations.AsReadOnly();

            public void AddClass(FileModel model)
            {
                var properties = model.Headers.Select(CreateField).ToArray();

                var dataClass = ClassDeclaration(model.ClassName).AddModifiers(Token(SyntaxKind.PublicKeyword)).AddMembers(properties);
                
                declarations.Add(dataClass);
            }

            private static MemberDeclarationSyntax CreateField(string propertyName)
            {
                return FieldDeclaration(VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)))
                        .AddVariables(VariableDeclarator(propertyName)))
                    .AddModifiers(Token(SyntaxKind.PublicKeyword));
            }
        }

        /// <summary>
        /// Represents dynamically generated extensions.
        /// E.g. if the schema model contains 'Nodes', node-related extensions are generated that in turn call 'dynamic' extensions from UserExtensions.Dynamic
        /// </summary>
        private class ExtensionClass
        {
            public ClassDeclarationSyntax Declaration { get; private set; }
            
            private static readonly Dictionary<string, MemberDeclarationSyntax> ConditionalExtensionMethods = new()
            {
                { "Nodes", NodesDelayedExtension! },
                { "Cortex_Documents", CortexParsingExtension! },
            };
            
            public ExtensionClass()
            {
                Declaration = ClassDeclaration("DataExtensions")
                    .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword));
            }

            public void EnsureExtension(FileModel fileModel)
            {
                if(ConditionalExtensionMethods.TryGetValue(fileModel.ClassName, out var extension))
                {
                    Declaration = Declaration.AddMembers(extension);
                }
            }

            private static MemberDeclarationSyntax? NodesDelayedExtension =>
                ParseMemberDeclaration(
                    "public static IEnumerable<Nodes> WhereDelayed(this IEnumerable<Nodes> enumerable, DateTime timeOfGatheringDiagnostics) => enumerable.WhereDelayed<Nodes>(timeOfGatheringDiagnostics);");

            private static MemberDeclarationSyntax? CortexParsingExtension =>
                ParseMemberDeclaration(
                    "public static IEnumerable<CortexDocument> Parse(this IEnumerable<Cortex_Documents> enumerable) => enumerable.ParseCortex();");
        }
    }
}