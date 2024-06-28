namespace Spravy.Domain.Extensions;

public static class DelegateExtension
{
    private static readonly MethodInfo DelegateInvokeDynamicType;

    static DelegateExtension()
    {
        var delegateType = typeof(Delegate);
        DelegateInvokeDynamicType = delegateType
            .GetMethod(nameof(Delegate.DynamicInvoke))
            .ThrowIfNull();
    }

    public static Type[] GetParameterTypes(this Delegate del)
    {
        return del.Method.GetParameters().Select(x => x.ParameterType).ToArray();
    }

    public static Expression ToCall(this Delegate del, IEnumerable<Expression> arguments)
    {
        var target = del.Target.ThrowIfNull();
        var instance = target.ToConstant();

        if (del.Method.IsRtDynamicMethod())
        {
            var args = arguments.Where(x => !x.Type.IsClosure());
            var parameters = typeof(object).ToNewArrayInit(args);

            return DelegateInvokeDynamicType
                .ToCall(del.ToConstant(), parameters)
                .ToConvert(del.Method.ReturnType);
        }

        return del.Method.ToCall(instance, arguments);
    }

    public static Expression ToCall(this Delegate del, params Expression[] arguments)
    {
        return del.ToCall(arguments.AsEnumerable());
    }
}
