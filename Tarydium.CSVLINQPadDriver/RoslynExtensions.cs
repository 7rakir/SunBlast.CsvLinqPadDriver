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

		public static ObjectCreationExpressionSyntax WithSingleArgument(this ObjectCreationExpressionSyntax creation, ExpressionSyntax expression)
		{
			return creation.WithArgumentList(CreateSingleArgumentList(expression));
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

		public static ObjectCreationExpressionSyntax CreateConstructorCall(string typeName)
		{
			return ObjectCreationExpression(Token(SyntaxKind.NewKeyword), ParseTypeName(typeName),
				null, null);
		}

		public static LiteralExpressionSyntax CreateStringLiteral(string value)
		{
			return LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value));
		}
	}
}