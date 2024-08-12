namespace Spravy.Ui.Services;

public class DialogViewer : IDialogViewer
{
    private readonly IServiceFactory serviceFactory;

    public DialogViewer(IServiceFactory serviceFactory)
    {
        this.serviceFactory = serviceFactory;
    }

    public ConfiguredValueTaskAwaitable<Result> ShowDialogAsync(
        DialogViewLayer layer,
        ViewModelBase viewModel,
        CancellationToken ct
    )
    {
        return GetDialogControl(layer)
            .IfSuccessAsync(dialogControl => ShowViewAsync(viewModel, dialogControl), ct);
    }

    public ConfiguredValueTaskAwaitable<Result> CloseDialogAsync(
        DialogViewLayer layer,
        CancellationToken ct
    )
    {
        return GetDialogControl(layer)
            .IfSuccess(dialogControl =>
                this.PostUiBackground(() => SafeCloseUi(dialogControl, ct), ct)
            )
            .ToValueTaskResult()
            .ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> CloseLastDialogAsync(CancellationToken ct)
    {
        return this.InvokeUiAsync(() =>
        {
            if (serviceFactory.CreateService<MainView>().ProgressDialogControl.IsOpen)
            {
                return SafeCloseUi(
                        serviceFactory.CreateService<MainView>().ProgressDialogControl,
                        ct
                    )
                    .IfSuccess(() => true.ToResult());
            }

            if (serviceFactory.CreateService<MainView>().ErrorDialogControl.IsOpen)
            {
                return SafeCloseUi(serviceFactory.CreateService<MainView>().ErrorDialogControl, ct)
                    .IfSuccess(() => true.ToResult());
            }

            if (serviceFactory.CreateService<MainView>().InputDialogControl.IsOpen)
            {
                return SafeCloseUi(serviceFactory.CreateService<MainView>().InputDialogControl, ct)
                    .IfSuccess(() => true.ToResult());
            }

            if (serviceFactory.CreateService<MainView>().ContentDialogControl.IsOpen)
            {
                return SafeCloseUi(
                        serviceFactory.CreateService<MainView>().ContentDialogControl,
                        ct
                    )
                    .IfSuccess(() => true.ToResult());
            }

            return false.ToResult();
        });
    }

    private ConfiguredValueTaskAwaitable<Result> ShowViewAsync(
        ViewModelBase viewModel,
        DialogControl dialogControl
    )
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

    private Result SafeCloseUi(DialogControl dialogControl, CancellationToken ct)
    {
        if (!dialogControl.IsOpen)
        {
            return Result.Success;
        }

        return dialogControl
            .Dialog.IfNotNull(nameof(dialogControl.Dialog))
            .IfSuccess(content =>
            {
                if (content is ISaveState saveState)
                {
                    return saveState.SaveStateAsync(ct).GetAwaiter().GetResult();
                }

                return Result.Success;
            })
            .IfSuccess(() =>
            {
                dialogControl.IsOpen = false;

                return Result.Success;
            });
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
