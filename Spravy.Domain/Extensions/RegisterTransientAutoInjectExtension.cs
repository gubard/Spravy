using System.Diagnostics;
using System.Linq.Expressions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class RegisterTransientAutoInjectExtension
{
    public static void RegisterTransientAutoInject(
        this IRegisterTransientAutoInjectMember register,
        Expression path
    )
    {
        switch (path)
        {
            case LambdaExpression lambdaExpression:
            {
                var type = lambdaExpression.Parameters.Single().Type;
                var memberExpression = lambdaExpression.Body.ThrowIfIsNot<MemberExpression>();
                var identifier = new AutoInjectMemberIdentifier(type, memberExpression.Member);
                var variable = memberExpression.Type.ToVariableAutoName();
                var lambda = variable.ToLambda(variable);
                register.RegisterTransientAutoInjectMember(identifier, lambda);

                break;
            }
            default:
            {
                throw new UnreachableException();
            }
        }
    }

    public static void RegisterTransientAutoInject(
        this IRegisterTransientAutoInjectMember register,
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
                register.RegisterTransientAutoInjectMember(identifier, value);

                break;
            }
            default:
            {
                throw new UnreachableException();
            }
        }
    }
}