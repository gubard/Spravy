using System.Diagnostics;
using System.Linq.Expressions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class RegisterScopeAutoInjectMemberExtension
{
    public static void RegisterScopeAutoInjectMember(
        this IRegisterScopeAutoInjectMember register,
        Expression path,
        Expression value
    )
    {
        switch (path)
        {
            case LambdaExpression lambdaExpression:
            {
                var type = lambdaExpression.Parameters.Single().Type;
                var memberExpression = lambdaExpression.Body.ThrowIfIsNot<MemberExpression>();
                var identifier = new AutoInjectMemberIdentifier(type, memberExpression.Member);
                register.RegisterScopeAutoInjectMember(identifier, value);

                break;
            }
            default:
            {
                throw new UnreachableException();
            }
        }
    }
}