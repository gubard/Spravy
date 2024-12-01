namespace Spravy.Client.Extensions;

public static class AsyncUnaryCallExtension
{
    public static async ValueTask<Result<T>> ToValueTaskResultValueOnly<T>(this AsyncUnaryCall<T> call)
        where T : notnull
    {
        var value = await call;

        return value.ToResult();
    }

    public static async ValueTask<Result> ToValueTaskResultOnly<T>(this AsyncUnaryCall<T> call)
    {
        var value = await call;

        return Result.Success;
    }
}