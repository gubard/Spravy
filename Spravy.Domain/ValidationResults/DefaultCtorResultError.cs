using Spravy.Domain.Models;

namespace Spravy.Domain.ValidationResults;

public class DefaultCtorResultError : Error
{
    public static readonly Guid MainId = new("1DEA9138-231C-4ED0-B650-EEDC285FD01B");

    public DefaultCtorResultError() : base(
        MainId,
        $"Default ctor for {nameof(Result)} not supported. Use {nameof(Result)}.{nameof(Result.Success)}."
    )
    {
    }
}

public class DefaultCtorResultError<TValue> : Error
{
    public static readonly Guid MainId = new("38CE2C63-F732-40A8-BBBA-0595AC623F73");

    public DefaultCtorResultError() : base(
        MainId,
        $"Default ctor for {nameof(Result<TValue>)} not supported."
    )
    {
    }
}