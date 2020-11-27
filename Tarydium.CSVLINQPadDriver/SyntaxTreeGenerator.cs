using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Tarydium.CSVLINQPadDriver
{
	public class SyntaxTreeGenerator
	{
		private readonly string nameSpace;
		private readonly string className;

		public SyntaxTreeGenerator(string nameSpace, string className)
		{
			this.nameSpace = nameSpace;
			this.className = className;
		}

		public SyntaxTree GetSyntaxTree(IEnumerable<DataFile> files)
		{
			var syntaxFactory = CompilationUnit();

			var usingList = new[]
			{
				"System",
				"System.Collections.Generic"
			}.Select(name => UsingDirective(ParseName(name))).ToArray();

			syntaxFactory = syntaxFactory.AddUsings(usingList);

			var classBuilder = new ContextClassBuilder(className);

			foreach (var file in files)
			{
				classBuilder.AddTable(file);
			}

			var members = classBuilder.Build().ToArray();

			var namespaceDeclaration = NamespaceDeclaration(ParseName(nameSpace)).AddMembers(members);

			syntaxFactory = syntaxFactory.AddMembers(namespaceDeclaration);

			return syntaxFactory.SyntaxTree;
		}

		private class ContextClassBuilder
		{
			private ClassDeclarationSyntax contextClass;

			private List<ClassDeclarationSyntax> dataClasses = new List<ClassDeclarationSyntax>();

			public ContextClassBuilder(string className)
			{
				contextClass = ClassDeclaration(className)
					.AddModifiers(Token(SyntaxKind.PublicKeyword));
			}

			public ContextClassBuilder AddTable(DataFile table)
			{
				dataClasses.Add(DataClassConstructor.CreateClass(table));

				var property = ContextClassConstructor.CreateProperty(table.ClassName);
				var field = ContextClassConstructor.CreateField(table.ClassName);

				contextClass = contextClass.AddMembers(property, field);

				return this;
			}

			public IEnumerable<ClassDeclarationSyntax> Build()
			{
				return dataClasses.Prepend(contextClass);
			}
		}

		private static class ContextClassConstructor
		{
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

			public static FieldDeclarationSyntax CreateField(string fieldName)
			{
				var call = ImplicitArrayCreationExpression(
					InitializerExpression(
						SyntaxKind.ArrayInitializerExpression,
						SingletonSeparatedList<ExpressionSyntax>(
							ObjectCreationExpression(
								IdentifierName(fieldName)))));

				var argumentList = ArgumentList(SeparatedList(new[]
				{
					Argument(ParenthesizedLambdaExpression(call))
				}));

				var objectCreation = ObjectCreationExpression(
					Token(SyntaxKind.NewKeyword),
					ParseTypeName($"Lazy<IEnumerable<{fieldName}>>"), argumentList, null);

				var equalsClause = EqualsValueClause(objectCreation);

				var variableDeclaration = VariableDeclaration(ParseTypeName($"Lazy<IEnumerable<{fieldName}>>"))
					.AddVariables(VariableDeclarator($"_{fieldName}").WithInitializer(equalsClause));

				return FieldDeclaration(variableDeclaration).AddModifiers(Token(SyntaxKind.PrivateKeyword));
			}
		}

		private static class DataClassConstructor
		{
			public static ClassDeclarationSyntax CreateClass(DataFile file)
			{
				var property = file.Headers.Select(header => CreateProperty("string", header)).ToArray();

				return ClassDeclaration(file.ClassName)
					.AddModifiers(Token(SyntaxKind.PublicKeyword))
					.AddMembers(property);
			}

			/// public Type Name { get; set; }
			private static PropertyDeclarationSyntax CreateProperty(string propertyType, string propertyName)
			{
				return PropertyDeclaration(ParseTypeName(propertyType), propertyName)
					.AddModifiers(Token(SyntaxKind.PublicKeyword))
					.AddAccessorListAccessors(
						AccessorDeclaration(SyntaxKind.GetAccessorDeclaration),
						AccessorDeclaration(SyntaxKind.SetAccessorDeclaration));
			}
		}
	}
}
