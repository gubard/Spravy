using System.Linq.Expressions;

namespace Spravy.Domain.Exceptions;

public class RecursionTypeExpressionInvokeException : Exception
{
    public RecursionTypeExpressionInvokeException(Type type, Expression expression) : base(
        $"{type} contains in parameters {expression}.")
    {
        Type = type;
        Expression = expression;
    }

    public Type Type { get; }
    public Expression Expression { get; }
}