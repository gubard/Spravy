namespace Spravy.Ui.Interfaces;

public interface IUiApplicationService
{
    ConfiguredValueTaskAwaitable<Result> RefreshCurrentViewAsync(CancellationToken cancellationToken);
    Result<Type> GetCurrentViewType();
}