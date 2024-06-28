namespace Spravy.Ui.Features.PasswordGenerator.Services;

public class PasswordItemCache : IPasswordItemCache
{
    private readonly Dictionary<Guid, PasswordItemNotify> cache;
    private readonly SpravyCommandService spravyCommandService;

    public PasswordItemCache(SpravyCommandService spravyCommandService)
    {
        cache = new();
        this.spravyCommandService = spravyCommandService;
    }

    public PasswordItemNotify GetPasswordItem(Guid id)
    {
        if (cache.TryGetValue(id, out var value))
        {
            return value;
        }

        var result = new PasswordItemNotify(
            new(
                id,
                "Loading...",
                string.Empty,
                512,
                string.Empty,
                true,
                true,
                true,
                true,
                string.Empty
            ),
            spravyCommandService
        );

        cache.Add(id, result);

        return result;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateAsync(PasswordItem passwordItem)
    {
        var item = GetPasswordItem(passwordItem.Id);

        return this.InvokeUiBackgroundAsync(() =>
        {
            item.Name = passwordItem.Name;

            return Result.Success;
        });
    }
}
