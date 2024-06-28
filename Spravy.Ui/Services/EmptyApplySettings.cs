namespace Spravy.Ui.Services;

public class EmptyApplySettings : IApplySettings
{
    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
