using System.Linq.Expressions;
using System.Reflection;

namespace Spravy.Domain.Extensions;

public static class ConstructorInfoExtension
{
    public static NewExpression ToNew(this ConstructorInfo constructor)
    {
        return Expression.New(constructor);
    }

    public static NewExpression ToNew(
        this ConstructorInfo constructor,
        IEnumerable<Expression> expressions
    )
    {
        return Expression.New(constructor, expressions);
    }

    public static NewExpression ToNew(
        this ConstructorInfo constructor,
        params Expression[] expressions
    )
    {
        return Expression.New(constructor, expressions);
    }
}