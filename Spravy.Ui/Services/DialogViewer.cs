namespace Spravy.Ui.Services;

public class DialogViewer : IDialogViewer
{
    private readonly IServiceFactory serviceFactory;

    public DialogViewer(IServiceFactory serviceFactory)
    {
        this.serviceFactory = serviceFactory;
    }

    public Cvtar ShowDialogAsync(
        DialogViewLayer layer,
        ViewModelBase viewModel,
        CancellationToken ct
    )
    {
        return GetDialogControl(layer)
            .IfSuccessAsync(dialogControl => ShowViewAsync(viewModel, dialogControl), ct);
    }

    public Cvtar CloseDialogAsync(DialogViewLayer layer, CancellationToken ct)
    {
        return GetDialogControl(layer).IfSuccessAsync(dc => SafeCloseUiAsync(dc, ct), ct);
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> CloseLastDialogAsync(CancellationToken ct)
    {
        if (
            this.GetUiValue(
                () => serviceFactory.CreateService<MainView>().ProgressDialogControl.IsOpen
            )
        )
        {
            return SafeCloseUiAsync(
                    serviceFactory.CreateService<MainView>().ProgressDialogControl,
                    ct
                )
                .IfSuccessAsync(() => true.ToResult(), ct);
        }

        if (
            this.GetUiValue(
                () => serviceFactory.CreateService<MainView>().ErrorDialogControl.IsOpen
            )
        )
        {
            return SafeCloseUiAsync(serviceFactory.CreateService<MainView>().ErrorDialogControl, ct)
                .IfSuccessAsync(() => true.ToResult(), ct);
        }

        if (
            this.GetUiValue(
                () => serviceFactory.CreateService<MainView>().InputDialogControl.IsOpen
            )
        )
        {
            return SafeCloseUiAsync(serviceFactory.CreateService<MainView>().InputDialogControl, ct)
                .IfSuccessAsync(() => true.ToResult(), ct);
        }

        if (
            this.GetUiValue(
                () => serviceFactory.CreateService<MainView>().ContentDialogControl.IsOpen
            )
        )
        {
            return SafeCloseUiAsync(
                    serviceFactory.CreateService<MainView>().ContentDialogControl,
                    ct
                )
                .IfSuccessAsync(() => true.ToResult(), ct);
        }

        return false.ToResult().ToValueTaskResult().ConfigureAwait(false);
    }

    private Cvtar ShowViewAsync(ViewModelBase viewModel, DialogControl dialogControl)
    {
        return this.InvokeUiBackgroundAsync(() =>
        {
            if (dialogControl.IsOpen)
            {
                return Result.Success;
            }

            dialogControl.Dialog = viewModel;
            dialogControl.IsOpen = true;

            return Result.Success;
        });
    }

    private ConfiguredValueTaskAwaitable<Result> SafeCloseUiAsync(
        DialogControl dialogControl,
        CancellationToken ct
    )
    {
        if (!this.GetUiValue(() => dialogControl.IsOpen))
        {
            return Result.AwaitableSuccess;
        }

        return dialogControl
            .Dialog.IfNotNull(nameof(dialogControl.Dialog))
            .IfSuccessAsync(
                content =>
                {
                    if (content is IStateHolder saveState)
                    {
                        return saveState.SaveStateAsync(ct);
                    }

                    return Result.AwaitableSuccess;
                },
                ct
            )
            .IfSuccessAsync(
                () =>
                    this.PostUiBackground(
                        () =>
                        {
                            dialogControl.IsOpen = false;

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
    }

    public Result<DialogControl> GetDialogControl(DialogViewLayer layer)
    {
        return layer switch
        {
            DialogViewLayer.Error
                => serviceFactory.CreateService<MainView>().ErrorDialogControl.ToResult(),
            DialogViewLayer.Progress
                => serviceFactory.CreateService<MainView>().ProgressDialogControl.ToResult(),
            DialogViewLayer.Input
                => serviceFactory.CreateService<MainView>().InputDialogControl.ToResult(),
            DialogViewLayer.Content
                => serviceFactory.CreateService<MainView>().ContentDialogControl.ToResult(),
            _ => new(new DialogViewLayerOutOfRangeError(layer))
        };
    }
}
