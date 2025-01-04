namespace Spravy.Ui.Services;

public class DialogViewer : IDialogViewer
{
    private readonly MainViewModel mainViewModel;

    public DialogViewer(IRootViewFactory rootViewFactory)
    {
        mainViewModel = rootViewFactory.CreateMainViewModel();
    }

    public Cvtar ShowDialogAsync(DialogViewLayer layer, IDialogable dialogable, CancellationToken ct)
    {
        return dialogable.LoadStateAsync(ct)
           .IfSuccessAsync(() => dialogable.RefreshAsync(ct), ct)
           .IfSuccessAsync(() => ShowView(layer, dialogable), ct);
    }

    public Cvtar CloseDialogAsync(DialogViewLayer layer, CancellationToken ct)
    {
        return CloseDialog(layer)
           .IfSuccessAsync(() => GetDialogContent(layer).IfSuccessAsync(d => d.SaveStateAsync(ct), ct), ct);
    }

    public Result<bool> CloseLastDialog(CancellationToken ct)
    {
        if (mainViewModel.ProgressDialogIsOpen)
        {
            return CloseDialog(DialogViewLayer.Progress).IfSuccess(() => true.ToResult());
        }

        if (mainViewModel.ErrorDialogIsOpen)
        {
            return CloseDialog(DialogViewLayer.Error).IfSuccess(() => true.ToResult());
        }

        if (mainViewModel.InputDialogIsOpen)
        {
            return CloseDialog(DialogViewLayer.Input).IfSuccess(() => true.ToResult());
        }

        if (mainViewModel.ContentDialogIsOpen)
        {
            return CloseDialog(DialogViewLayer.Content).IfSuccess(() => true.ToResult());
        }

        return false.ToResult();
    }

    private Result ShowView(DialogViewLayer layer, IDialogable dialogable)
    {
        return layer switch
        {
            DialogViewLayer.Error => this.PostUiBackground(
                () =>
                {
                    mainViewModel.ErrorDialogIsOpen = true;
                    mainViewModel.ErrorDialogContent = dialogable;

                    return Result.Success;
                },
                CancellationToken.None
            ),
            DialogViewLayer.Progress => this.PostUiBackground(
                () =>
                {
                    mainViewModel.ProgressDialogIsOpen = true;
                    mainViewModel.ProgressDialogContent = dialogable;

                    return Result.Success;
                },
                CancellationToken.None
            ),
            DialogViewLayer.Input => this.PostUiBackground(
                () =>
                {
                    mainViewModel.InputDialogIsOpen = true;
                    mainViewModel.InputDialogContent = dialogable;

                    return Result.Success;
                },
                CancellationToken.None
            ),
            DialogViewLayer.Content => this.PostUiBackground(
                () =>
                {
                    mainViewModel.ContentDialogIsOpen = true;
                    mainViewModel.ContentDialogContent = dialogable;

                    return Result.Success;
                },
                CancellationToken.None
            ),
            _ => new(new DialogViewLayerOutOfRangeError(layer)),
        };
    }

    private Result CloseDialog(DialogViewLayer layer)
    {
        return layer switch
        {
            DialogViewLayer.Error => this.PostUiBackground(
                () =>
                {
                    mainViewModel.ErrorDialogIsOpen = false;

                    return Result.Success;
                },
                CancellationToken.None
            ),
            DialogViewLayer.Progress => this.PostUiBackground(
                () =>
                {
                    mainViewModel.ProgressDialogIsOpen = false;

                    return Result.Success;
                },
                CancellationToken.None
            ),
            DialogViewLayer.Input => this.PostUiBackground(
                () =>
                {
                    mainViewModel.InputDialogIsOpen = false;

                    return Result.Success;
                },
                CancellationToken.None
            ),
            DialogViewLayer.Content => this.PostUiBackground(
                () =>
                {
                    mainViewModel.ContentDialogIsOpen = false;

                    return Result.Success;
                },
                CancellationToken.None
            ),
            _ => new(new DialogViewLayerOutOfRangeError(layer)),
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