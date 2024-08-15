namespace Spravy.Ui.Services;

public class EmptyApplySettings : IApplySettings
{
    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
