namespace Spravy.Domain.Extensions;

public static class TypeExtension
{
    public static ConstructorInfo? GetSingleConstructorOrNull(this Type type)
    {
        var constructors = type.GetConstructors();

        if (constructors.Length == 0)
        {
            return null;
        }

        if (constructors.Length > 1)
        {
            throw new ToManyConstructorsException(type, 1, constructors.Length);
        }

        return constructors[0];
    }

    public static NewExpression ToNew(this Type type)
    {
        return Expression.New(type);
    }

    public static ParameterExpression ToVariable(this Type type, string name)
    {
        return Expression.Variable(type, name);
    }

    public static ParameterExpression ToVariable(this Type type)
    {
        return Expression.Variable(type);
    }

    public static ParameterExpression ToVariableAutoName(this Type type)
    {
        return type.ToVariable(RandomStringGuid.Digits.GetRandom());
    }

    public static ParameterExpression ToParameter(this Type type, string name)
    {
        return Expression.Parameter(type, name);
    }

    public static ParameterExpression ToParameter(this Type type)
    {
        return Expression.Parameter(type);
    }

    public static ParameterExpression ToParameterAutoName(this Type type)
    {
        return type.ToParameter(RandomStringGuid.Digits.GetRandom());
    }

    public static LabelTarget ToLabel(this Type type)
    {
        return Expression.Label(type);
    }

    public static NewArrayExpression ToNewArrayInit(this Type type)
    {
        return Expression.NewArrayInit(type);
    }

    public static NewArrayExpression ToNewArrayInit(
        this Type type,
        IEnumerable<Expression> expressions
    )
    {
        return Expression.NewArrayInit(type, expressions);
    }

    public static bool IsClosure(this Type type)
    {
        if (type.ToString() == "System.Runtime.CompilerServices.Closure")
        {
            return true;
        }

        return false;
    }

    public static object? GetDefaultValue(this Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }

        return null;
    }

    public static bool IsEnumerable(this Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        if (type.GenericTypeArguments.Length != 1)
        {
            return false;
        }

        var genericType = type.GetGenericTypeDefinition();

        if (genericType != typeof(IEnumerable<>))
        {
            return false;
        }

        return true;
    }
}
