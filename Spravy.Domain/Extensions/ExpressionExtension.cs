namespace Spravy.Domain.Extensions;

public static class ExpressionExtension
{
    public static BinaryExpression ToAssign(this Expression expression, Expression right)
    {
        return Expression.Assign(expression, right);
    }

    public static MemberExpression ToProperty(this Expression expression, PropertyInfo property)
    {
        return Expression.Property(expression, property);
    }

    public static MemberExpression ToField(this Expression expression, FieldInfo field)
    {
        return Expression.Field(expression, field);
    }

    public static MemberExpression ToMember(this Expression expression, MemberInfo member)
    {
        switch (member)
        {
            case PropertyInfo property:
            {
                return expression.ToProperty(property);
            }
            case FieldInfo field:
            {
                return expression.ToField(field);
            }
            default:
            {
                throw new UnreachableException();
            }
        }
    }

    public static LambdaExpression ToLambda(this Expression expression)
    {
        return Expression.Lambda(expression);
    }

    public static UnaryExpression ToConvert(this Expression expression, Type type)
    {
        return Expression.Convert(expression, type);
    }

    public static InvocationExpression ToInvoke(
        this Expression expression,
        IEnumerable<Expression> expressions
    )
    {
        return Expression.Invoke(expression, expressions);
    }

    public static InvocationExpression ToInvoke(
        this Expression expression,
        params Expression[] expressions
    )
    {
        return Expression.Invoke(expression, expressions);
    }

    public static LambdaExpression ToLambda(
        this Expression expression,
        params ParameterExpression[] parameters
    )
    {
        return Expression.Lambda(expression, parameters);
    }

    public static BlockExpression ToBlock(
        this IEnumerable<ParameterExpression> variables,
        params Expression[] expressions
    )
    {
        return Expression.Block(variables, expressions);
    }

    public static BlockExpression ToBlock(
        this IEnumerable<ParameterExpression> variables,
        IEnumerable<Expression> expressions
    )
    {
        return Expression.Block(variables, expressions);
    }
}
