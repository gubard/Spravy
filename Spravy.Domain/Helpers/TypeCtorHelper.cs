using System.Reflection;
using Spravy.Domain.Extensions;

namespace Spravy.Domain.Helpers;

public static class TypeCtorHelper<TType, TParam>
{
    private static readonly Type[] CtorParams;
    public static readonly ConstructorInfo Ctor;
    public static readonly Func<TParam, TType> CtorFunc;

    static TypeCtorHelper()
    {
        CtorParams = new[]
        {
            typeof(TParam),
        };

        var variable = typeof(TParam).ToVariableAutoName();
        Ctor = typeof(TType).GetConstructor(CtorParams).ThrowIfNull();
        CtorFunc = (Func<TParam, TType>)Ctor.ToNew(variable).ToLambda(variable).Compile();
    }
}