namespace Spravy.Ui.Interfaces;

public interface IUiApplicationService
{
    ConfiguredValueTaskAwaitable<Result> RefreshCurrentViewAsync(CancellationToken ct);
    Result<Type> GetCurrentViewType();
    Result<TView> GetCurrentView<TView>() where TView: notnull;
}