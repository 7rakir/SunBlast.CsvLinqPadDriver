using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using static CsvLinqPadDriver.RoslynExtensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CsvLinqPadDriver
{
    internal class SyntaxTreeBuilder
    {
        private ClassDeclarationSyntax contextClass;

        private readonly List<MemberDeclarationSyntax> dataClasses = new();

        public SyntaxTreeBuilder(string className)
        {
            contextClass = ClassDeclaration(className).AsPublic();
        }

        public void AddModel(FileModel fileModel)
        {
            dataClasses.Add(DataClassGenerator.CreateClass(fileModel));

            var property = ContextClassGenerator.CreateProperty(fileModel);

            contextClass = contextClass.AddMembers(property);
        }

        public SyntaxTree Build(string nameSpace)
        {
            var extensionClass = DataClassExtensionsGenerator.CreateClass();

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
                "CsvLinqPadDriver.DataExtensions",
                "CsvParser",
            }.Select(name => UsingDirective(ParseName(name)));
        }

        private static class ContextClassGenerator
        {
            public static MemberDeclarationSyntax CreateProperty(FileModel model)
            {
                return PropertyDeclaration(EnumerableType(model.ClassName), model.ClassName)
                    .AsPublic()
                    .Returning(
                        "CsvReader",
                        Type("ReadFile", model.ClassName),
                        CreateStringLiteral(model.FilePath));
            }
        }

        private static class DataClassGenerator
        {
            public static ClassDeclarationSyntax CreateClass(FileModel model)
            {
                if (!CodeGenerator.IsValidLanguageIndependentIdentifier(model.ClassName))
                {
                    throw new ArgumentException($"Invalid name '{model.ClassName}'", model.ClassName);
                }

                var properties = model.Headers.Select(CreateProperty).ToArray();

                return ClassDeclaration(model.ClassName).AsPublic().AddMembers(properties);
            }

            private static MemberDeclarationSyntax CreateProperty(string propertyName)
            {
                return FieldDeclaration(VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)))
                        .AddVariables(VariableDeclarator(propertyName)))
                    .AddModifiers(Token(SyntaxKind.PublicKeyword));
            }
        }

        private static class DataClassExtensionsGenerator
        {
            public static ClassDeclarationSyntax CreateClass()
            {
                var delayed = MethodDeclaration(EnumerableType("Nodes"), "WhereDelayed")
                    .AsPublicStatic()
                    .AddParameterListParameters(
                        ThisParameter(EnumerableType("Nodes"), "enumerable"),
                        Parameter("DateTime", "timeOfGatheringDiagnostics"))
                    .Returning(
                        "enumerable",
                        Type("WhereDelayed", "Nodes"),
                        IdentifierName("timeOfGatheringDiagnostics"));

                var cortex = MethodDeclaration(EnumerableType("CortexDocument"), "Parse")
                    .AsPublicStatic()
                    .AddParameterListParameters(ThisParameter(EnumerableType("Cortex_Documents"), "enumerable"))
                    .Returning("enumerable", IdentifierName("ParseCortex"));

                return ClassDeclaration("ModelExtensions").AsPublicStatic().AddMembers(delayed, cortex);
            }
        }

        private static ParameterSyntax ThisParameter(TypeSyntax type, string name)
        {
            return SyntaxFactory.Parameter(Identifier(name))
                .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                .WithType(type);
        }

        private static TypeSyntax EnumerableType(string typeName)
        {
            return Type("IEnumerable", typeName);
        }

        private static GenericNameSyntax Type(string name, string typeName)
        {
            return GenericName(name).AddTypeArgumentListArguments(IdentifierName(typeName));
        }

        private static ParameterSyntax Parameter(string type, string name)
        {
            return SyntaxFactory.Parameter(Identifier(name)).WithType(IdentifierName(type));
        }
    }
}