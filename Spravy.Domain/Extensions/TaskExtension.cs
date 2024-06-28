namespace Spravy.Domain.Extensions;

public static class TaskExtension
{
    public static Task WhenAll(this IEnumerable<Task> tasks)
    {
        return Task.WhenAll(tasks);
    }

    public static async Task<Result> WhenAll(
        this ConfiguredTaskAwaitable<Result> task,
        params ConfiguredTaskAwaitable<Result>[] tasks
    )
    {
        var result = await task;
        var errors = result.Errors;

        foreach (var t in tasks)
        {
            var r = await t;
            errors = errors.Combine(r.Errors);
        }

        return new(errors);
    }

    public static async Task<Result> ToResult<TReturn>(
        this ConfiguredTaskAwaitable<Result<TReturn>> task
    )
        where TReturn : notnull
    {
        var result = await task;

        return new(result.Errors);
    }
}
