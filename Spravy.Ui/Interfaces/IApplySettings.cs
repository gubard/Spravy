namespace Spravy.Ui.Interfaces;

public interface IApplySettings
{
    ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken);
}