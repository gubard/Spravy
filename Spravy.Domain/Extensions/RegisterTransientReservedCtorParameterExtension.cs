using System.Linq.Expressions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class RegisterTransientReservedCtorParameterExtension
{
    public static void RegisterTransientReservedCtorParameter<T, TParameter>(
        this IRegisterTransientReservedCtorParameter register,
        Expression expression
    )
    {
        var type = typeof(T);
        var constructor = type.GetSingleConstructorOrNull().ThrowIfNull();

        var parameter = constructor
            .GetParameters()
            .Single(x => x.ParameterType == typeof(TParameter));

        var identifier = new ReservedCtorParameterIdentifier(type, constructor, parameter);
        register.RegisterTransientReservedCtorParameter(identifier, expression);
    }

    public static void RegisterTransientReservedCtorParameterDel<T, TParameter>(
        this IRegisterTransientReservedCtorParameter register,
        Delegate del
    )
    {
        var type = typeof(T);
        var constructor = type.GetSingleConstructorOrNull().ThrowIfNull();

        var parameter = constructor
            .GetParameters()
            .Single(x => x.ParameterType == typeof(TParameter));

        var identifier = new ReservedCtorParameterIdentifier(type, constructor, parameter);

        var variables = del.Method
            .GetParameters()
            .Select(x => x.ParameterType.ToVariableAutoName());

        var call = del.ToCall(variables);
        register.RegisterTransientReservedCtorParameter(identifier, call);
    }
}