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
        private ClassDeclarationSyntax contextClass;

        private readonly List<ClassDeclarationSyntax> dataClasses = new();

        private readonly HashSet<string> dataClassNames = new();

        public SyntaxTreeBuilder(string className)
        {
            contextClass = ClassDeclaration(className).AddModifiers(Token(SyntaxKind.PublicKeyword));
        }

        public void AddModel(FileModel fileModel)
        {
            dataClasses.Add(DataClassGenerator.CreateClass(fileModel));
            dataClassNames.Add(fileModel.ClassName);

            var property = ContextClassGenerator.CreateProperty(fileModel)!;

            contextClass = contextClass.AddMembers(property);
        }

        public SyntaxTree Build(string nameSpace)
        {
            var extensionClass = DataClassExtensionsGenerator.CreateClass(dataClassNames);

            var members = dataClasses.Prepend(contextClass).Append(extensionClass).ToArray();

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

        private static class ContextClassGenerator
        {
            public static MemberDeclarationSyntax? CreateProperty(FileModel model)
            {
                return ParseMemberDeclaration(
                    $@"public IEnumerable<{model.ClassName}> {model.ClassName} => CsvReader.ReadFile<{model.ClassName}>(@""{model.FilePath}"");");
            }
        }

        private static class DataClassGenerator
        {
            public static ClassDeclarationSyntax CreateClass(FileModel model)
            {
                var properties = model.Headers.Select(CreateField).ToArray();

                return ClassDeclaration(model.ClassName).AddModifiers(Token(SyntaxKind.PublicKeyword)).AddMembers(properties);
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
        private static class DataClassExtensionsGenerator
        {
            public static ClassDeclarationSyntax CreateClass(IReadOnlySet<string> dataClassNames)
            {
                var extensions = GetExtensions(dataClassNames).ToArray();
                return ClassDeclaration("DataExtensions")
                    .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                    .AddMembers(extensions);
            }

            private static IEnumerable<MemberDeclarationSyntax> GetExtensions(IReadOnlySet<string> dataClassNames)
            {
                var conditionalExtensionMethods = new Dictionary<string, MemberDeclarationSyntax>()
                {
                    { "Nodes", NodesDelayedExtension! },
                    { "Cortex_Documents", CortexParsingExtension! },
                };
                
                foreach (var (identifier, extension) in conditionalExtensionMethods)
                {
                    if (dataClassNames.Contains(identifier))
                    {
                        yield return extension;
                    }
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