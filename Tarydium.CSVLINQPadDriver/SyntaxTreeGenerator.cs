using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Tarydium.CSVLINQPadDriver.RoslynExtensions;

namespace Tarydium.CSVLINQPadDriver
{
	public class SyntaxTreeGenerator
	{
		public SyntaxTree GetSyntaxTree(string nameSpace, string className, IEnumerable<FileModel> schema)
		{
			var syntaxFactory = CompilationUnit();

			var usingList = new[]
			{
				"System",
				"System.Collections.Generic",
				"System.IO",
				"System.Linq",
				"CsvParser"
			}.Select(name => UsingDirective(ParseName(name))).ToArray();

			syntaxFactory = syntaxFactory.AddUsings(usingList);

			var classBuilder = new ContextClassBuilder(className);

			foreach(var model in schema)
			{
				classBuilder.AddTable(model);
			}

			var members = classBuilder.Build().ToArray();

			var namespaceDeclaration = NamespaceDeclaration(ParseName(nameSpace)).AddMembers(members);

			syntaxFactory = syntaxFactory.AddMembers(namespaceDeclaration);

			return syntaxFactory.SyntaxTree;
		}

		private class ContextClassBuilder
		{
			private ClassDeclarationSyntax contextClass;

			private readonly List<ClassDeclarationSyntax> dataClasses = new List<ClassDeclarationSyntax>();

			public ContextClassBuilder(string className)
			{
				contextClass = ContextClassGenerator.CreateClass(className);
			}

			public void AddTable(FileModel model)
			{
				dataClasses.Add(DataClassGenerator.CreateClass(model));

				var property = ContextClassGenerator.CreateProperty(model.ClassName);
				var field = ContextClassGenerator.CreateField(model);

				contextClass = contextClass.AddMembers(property, field);
			}

			public IEnumerable<ClassDeclarationSyntax> Build()
			{
				return dataClasses.Prepend(contextClass);
			}
		}

		private static class ContextClassGenerator
		{
			public static ClassDeclarationSyntax CreateClass(string className)
			{
				return ClassDeclaration(className)
					.AddModifiers(Token(SyntaxKind.PublicKeyword));
			}

			public static PropertyDeclarationSyntax CreateProperty(string propertyName)
			{
				return PropertyDeclaration(ParseTypeName($"IEnumerable<{propertyName}>"), propertyName)
					.AddModifiers(Token(SyntaxKind.PublicKeyword))
					.WithExpressionBody(
						ArrowExpressionClause(
							MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								IdentifierName($"_{propertyName}"),
								IdentifierName("Value"))));
			}

			public static FieldDeclarationSyntax CreateField(FileModel model)
			{
				var readFileCall = GetCsvReaderCall(model);

				// field declaration and initialization

				var objectCreation = CreateConstructorCall($"Lazy<IEnumerable<{model.ClassName}>>")
					.WithSingleArgument(ParenthesizedLambdaExpression(readFileCall));

				var equalsClause = EqualsValueClause(objectCreation);

				var variableDeclaration = VariableDeclaration(ParseTypeName($"Lazy<IEnumerable<{model.ClassName}>>"))
					.AddVariables(VariableDeclarator($"_{model.ClassName}").WithInitializer(equalsClause));

				return FieldDeclaration(variableDeclaration).AddModifiers(Token(SyntaxKind.PrivateKeyword));
			}

			private static InvocationExpressionSyntax GetCsvReaderCall(FileModel model)
			{
				return InvocationExpression(MemberAccessExpression(
						SyntaxKind.SimpleMemberAccessExpression,
						IdentifierName("CsvReader"),
						GenericName(Identifier("ReadFile")).WithSingleTypeArgument(IdentifierName(model.ClassName))))
					.WithSingleArgument(CreateStringLiteral(model.FilePath));
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

				var property = model.Headers.Select(header => CreateProperty("string", header)).ToArray();

				return ClassDeclaration(model.ClassName)
					.AddModifiers(Token(SyntaxKind.PublicKeyword))
					.AddMembers(property);
			}

			private static PropertyDeclarationSyntax CreateProperty(string propertyType, string propertyName)
			{
				return PropertyDeclaration(ParseTypeName(propertyType), propertyName)
					.AddModifiers(Token(SyntaxKind.PublicKeyword))
					.AddAccessorListAccessors(AccessorDeclaration(SyntaxKind.GetAccessorDeclaration), AccessorDeclaration(SyntaxKind.SetAccessorDeclaration));
			}
		}
	}
}
