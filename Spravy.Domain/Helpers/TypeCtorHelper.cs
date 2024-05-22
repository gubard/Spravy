namespace Spravy.Domain.Helpers;

public static class TypeCtorHelper<TType, TParam>
{
    public static readonly Func<TParam, TType> CtorFunc;

    static TypeCtorHelper()
    {
        var ctorParams = new[]
        {
            typeof(TParam),
        };

        var variable = typeof(TParam).ToVariableAutoName();
        var ctor = typeof(TType).GetConstructor(ctorParams).ThrowIfNull();
        CtorFunc = (Func<TParam, TType>)ctor.ToNew(variable).ToLambda(variable).Compile();
    }
}