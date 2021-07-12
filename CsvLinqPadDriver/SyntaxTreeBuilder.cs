using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CsvLinqPadDriver.RoslynExtensions;

namespace CsvLinqPadDriver
{
	public class SyntaxTreeBuilder
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
			var members = dataClasses.Prepend(contextClass).ToArray();
			
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
				"CsvParser"
			}.Select(name => UsingDirective(ParseName(name)));
		}

		private static class ContextClassGenerator
		{
			public static PropertyDeclarationSyntax CreateProperty(FileModel model)
			{
				return PropertyDeclaration(ParseTypeName($"IEnumerable<{model.ClassName}>"), model.ClassName)
					.AsPublic()
					.WithExpressionBody(
						ArrowExpressionClause(
							GetCsvReaderCall(model)));
			}

			private static InvocationExpressionSyntax GetCsvReaderCall(FileModel model)
			{
				return InvocationExpression(
					MemberAccessExpression(
						SyntaxKind.SimpleMemberAccessExpression,
						IdentifierName("CsvReader"),
						GenericName("ReadFile").WithSingleTypeArgument(IdentifierName(model.ClassName)))
					).WithSingleArgument(CreateStringLiteral(model.FilePath));
			}
		}

		private static class DataClassGenerator
		{
			public static ClassDeclarationSyntax CreateClass(FileModel model)
			{
				if(!CodeGenerator.IsValidLanguageIndependentIdentifier(model.ClassName))
				{
					throw new ArgumentException($"Invalid name '{model.ClassName}'", model.ClassName);
				}

				var property = model.Headers.Select(CreateProperty).ToArray();

				return ClassDeclaration(model.ClassName).AsPublic().AddMembers(property);
			}

			private static MemberDeclarationSyntax CreateProperty(string propertyName)
			{
				return PropertyDeclaration(ParseTypeName("string"), propertyName)
					.AsPublic()
					.AddAccessorListAccessors(AccessorDeclaration(SyntaxKind.GetAccessorDeclaration), AccessorDeclaration(SyntaxKind.SetAccessorDeclaration));
			}
		}
	}
}
