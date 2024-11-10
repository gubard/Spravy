namespace Spravy.Ui.Services;

public class DialogViewer : IDialogViewer
{
    private readonly MainViewModel mainViewModel;

    public DialogViewer(IRootViewFactory rootViewFactory)
    {
        mainViewModel = rootViewFactory.CreateMainViewModel();
    }

    public Cvtar ShowDialogAsync(
        DialogViewLayer layer,
        IDialogable dialogable,
        CancellationToken ct
    )
    {
        return dialogable
            .LoadStateAsync(ct)
            .IfSuccessAsync(() => dialogable.RefreshAsync(ct), ct)
            .IfSuccessAsync(() => ShowViewAsync(layer, dialogable), ct);
    }

    public Cvtar CloseDialogAsync(DialogViewLayer layer, CancellationToken ct)
    {
        return CloseDialogAsync(layer)
            .IfSuccessAsync(
                () => GetDialogContent(layer).IfSuccessAsync(d => d.SaveStateAsync(ct), ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> CloseLastDialogAsync(CancellationToken ct)
    {
        if (mainViewModel.ProgressDialogIsOpen)
        {
            return CloseDialogAsync(DialogViewLayer.Progress)
                .IfSuccessAsync(() => true.ToResult(), ct);
        }

        if (mainViewModel.ErrorDialogIsOpen)
        {
            return CloseDialogAsync(DialogViewLayer.Error)
                .IfSuccessAsync(() => true.ToResult(), ct);
        }

        if (mainViewModel.InputDialogIsOpen)
        {
            return CloseDialogAsync(DialogViewLayer.Input)
                .IfSuccessAsync(() => true.ToResult(), ct);
        }

        if (mainViewModel.ContentDialogIsOpen)
        {
            return CloseDialogAsync(DialogViewLayer.Content)
                .IfSuccessAsync(() => true.ToResult(), ct);
        }

        return false.ToResult().ToValueTaskResult().ConfigureAwait(false);
    }

    private Cvtar ShowViewAsync(DialogViewLayer layer, IDialogable dialogable)
    {
        return layer switch
        {
            DialogViewLayer.Error => this.InvokeUiBackgroundAsync(() =>
            {
                mainViewModel.ErrorDialogIsOpen = true;
                mainViewModel.ErrorDialogContent = dialogable;

                return Result.Success;
            }),
            DialogViewLayer.Progress => this.InvokeUiBackgroundAsync(() =>
            {
                mainViewModel.ProgressDialogIsOpen = true;
                mainViewModel.ProgressDialogContent = dialogable;

                return Result.Success;
            }),
            DialogViewLayer.Input => this.InvokeUiBackgroundAsync(() =>
            {
                mainViewModel.InputDialogIsOpen = true;
                mainViewModel.InputDialogContent = dialogable;

                return Result.Success;
            }),
            DialogViewLayer.Content => this.InvokeUiBackgroundAsync(() =>
            {
                mainViewModel.ContentDialogIsOpen = true;
                mainViewModel.ContentDialogContent = dialogable;

                return Result.Success;
            }),
            _ => new Result(new DialogViewLayerOutOfRangeError(layer))
                .ToValueTaskResult()
                .ConfigureAwait(false),
        };
    }

    private Cvtar CloseDialogAsync(DialogViewLayer layer)
    {
        return layer switch
        {
            DialogViewLayer.Error => this.InvokeUiBackgroundAsync(() =>
            {
                mainViewModel.ErrorDialogIsOpen = false;

                return Result.Success;
            }),
            DialogViewLayer.Progress => this.InvokeUiBackgroundAsync(() =>
            {
                mainViewModel.ProgressDialogIsOpen = false;

                return Result.Success;
            }),
            DialogViewLayer.Input => this.InvokeUiBackgroundAsync(() =>
            {
                mainViewModel.InputDialogIsOpen = false;

                return Result.Success;
            }),
            DialogViewLayer.Content => this.InvokeUiBackgroundAsync(() =>
            {
                mainViewModel.ContentDialogIsOpen = false;

                return Result.Success;
            }),
            _ => new Result(new DialogViewLayerOutOfRangeError(layer))
                .ToValueTaskResult()
                .ConfigureAwait(false),
        };
    }

    private Result<IDialogable> GetDialogContent(DialogViewLayer layer)
    {
        return layer switch
        {
            DialogViewLayer.Error => mainViewModel.ErrorDialogContent.ToResult(),
            DialogViewLayer.Progress => mainViewModel.ProgressDialogContent.ToResult(),
            DialogViewLayer.Input => mainViewModel.InputDialogContent.ToResult(),
            DialogViewLayer.Content => mainViewModel.ContentDialogContent.ToResult(),
            _ => new(new DialogViewLayerOutOfRangeError(layer)),
        };
    }
}
