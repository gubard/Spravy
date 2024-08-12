namespace Spravy.Ui.Interfaces;

public interface IDialogViewer
{
    ConfiguredValueTaskAwaitable<Result> ShowDialogAsync(
        DialogViewLayer layer,
        ViewModelBase viewModel,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result> CloseDialogAsync(
        DialogViewLayer layer,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<bool>> CloseLastDialogAsync(CancellationToken ct);
}
