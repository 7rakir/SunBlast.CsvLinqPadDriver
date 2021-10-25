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

        public static MemberDeclarationSyntax Returning(this MethodDeclarationSyntax declarationSyntax, string subject,
            SimpleNameSyntax call, params ExpressionSyntax[] callArguments)
        {
            var arrow = ArrowExpressionClause(GetMemberAccessCall(subject, call, callArguments));
            return declarationSyntax.WithExpressionBody(arrow);
        }

        public static PropertyDeclarationSyntax Returning(this PropertyDeclarationSyntax declarationSyntax,
            string subject,
            SimpleNameSyntax call, params ExpressionSyntax[] callArguments)
        {
            var arrow = ArrowExpressionClause(GetMemberAccessCall(subject, call, callArguments));
            return declarationSyntax.WithExpressionBody(arrow);
        }

        private static InvocationExpressionSyntax GetMemberAccessCall(string target,
            SimpleNameSyntax call, params ExpressionSyntax[] callArguments)
        {
            return InvocationExpression(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(target),
                    call))
                .AddArgumentListArguments(callArguments.Select(Argument).ToArray());
        }
        
        public static ParameterSyntax ThisParameter(TypeSyntax type, string name)
        {
            return SyntaxFactory.Parameter(Identifier(name))
                .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                .WithType(type);
        }

        public static TypeSyntax EnumerableType(string typeName)
        {
            return Type("IEnumerable", typeName);
        }

        public static GenericNameSyntax Type(string name, string typeName)
        {
            return GenericName(name).AddTypeArgumentListArguments(IdentifierName(typeName));
        }

        public static ParameterSyntax Parameter(string type, string name)
        {
            return SyntaxFactory.Parameter(Identifier(name)).WithType(IdentifierName(type));
        }
    }
}