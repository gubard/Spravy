namespace Spravy.Domain.Extensions;

public static class TaskExtension
{
    public static Task WhenAll(this IEnumerable<Task> tasks)
    {
        return Task.WhenAll(tasks);
    }
}