using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CsvLinqPadDriver
{
    internal static class RoslynExtensions
    {
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

        public static MethodDeclarationSyntax AsPublicStatic(this MethodDeclarationSyntax declarationSyntax)
        {
            return declarationSyntax.AddModifiers(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.StaticKeyword));
        }

        public static ClassDeclarationSyntax AsPublicStatic(this ClassDeclarationSyntax declarationSyntax)
        {
            return declarationSyntax.AddModifiers(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.StaticKeyword));
        }

        public static MemberDeclarationSyntax Returning(this MethodDeclarationSyntax declarationSyntax, string target,
            SimpleNameSyntax call, params ExpressionSyntax[] callArguments)
        {
            return declarationSyntax.WithExpressionBody(
                ArrowExpressionClause(
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(target),
                                call))
                        .AddArgumentListArguments(callArguments.Select(Argument).ToArray())));
        }
        
        public static PropertyDeclarationSyntax Returning(this PropertyDeclarationSyntax declarationSyntax, string target,
            SimpleNameSyntax call, params ExpressionSyntax[] callArguments)
        {
            return declarationSyntax.WithExpressionBody(
                ArrowExpressionClause(
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(target),
                                call))
                        .AddArgumentListArguments(callArguments.Select(Argument).ToArray())));
        }
    }
}