namespace Spravy.Ui.Services;

public class EmptyApplySettings : IApplySettings
{
    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableSuccess;
    }
}