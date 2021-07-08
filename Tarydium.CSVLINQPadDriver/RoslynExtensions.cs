using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Tarydium.CSVLINQPadDriver
{
	public static class RoslynExtensions
	{
		public static InvocationExpressionSyntax WithSingleArgument(this InvocationExpressionSyntax invocation, ExpressionSyntax expression)
		{
			return invocation.WithArgumentList(CreateSingleArgumentList(expression));
		}

		public static GenericNameSyntax WithSingleTypeArgument(this GenericNameSyntax genericName, TypeSyntax type)
		{
			return genericName.WithTypeArgumentList(
				TypeArgumentList(SingletonSeparatedList(type)));
		}

		private static ArgumentListSyntax CreateSingleArgumentList(ExpressionSyntax expression)
		{
			return ArgumentList(SingletonSeparatedList(Argument(expression)));
		}

		public static LiteralExpressionSyntax CreateStringLiteral(string value)
		{
			return LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value));
		}

		public static PropertyDeclarationSyntax AsPublic(this PropertyDeclarationSyntax declarationSyntax)
		{
			return declarationSyntax.AddModifiers(Token(SyntaxKind.PublicKeyword));
		}
		
		public static ClassDeclarationSyntax AsPublic(this ClassDeclarationSyntax declarationSyntax)
		{
			return declarationSyntax.AddModifiers(Token(SyntaxKind.PublicKeyword));
		}
	}
}