namespace Spravy.Ui.Services;

public class DialogViewer : IDialogViewer
{
    private readonly IServiceFactory serviceFactory;

    public DialogViewer(IServiceFactory serviceFactory)
    {
        this.serviceFactory = serviceFactory;
    }

    public ConfiguredValueTaskAwaitable<Result> ShowContentDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();

        return this.InvokeUiBackgroundAsync(() =>
            {
                setupView.Invoke(content);

                return Result.Success;
            })
            .IfSuccessAsync(
                () =>
                    ShowView(
                        content,
                        serviceFactory.CreateService<MainView>().ContentDialogControl
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowProgressDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();

        return this.InvokeUiBackgroundAsync(() =>
            {
                setupView.Invoke(content);

                return Result.Success;
            })
            .IfSuccessAsync(
                () =>
                    ShowView(
                        content,
                        serviceFactory.CreateService<MainView>().ProgressDialogControl
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowErrorDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();

        return this.InvokeUiBackgroundAsync(() =>
            {
                setupView.Invoke(content);

                return Result.Success;
            })
            .IfSuccessAsync(
                () =>
                    ShowView(content, serviceFactory.CreateService<MainView>().ErrorDialogControl),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowInfoErrorDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();

        return this.InvokeUiBackgroundAsync(() =>
            {
                setupView.Invoke(content);

                return Result.Success;
            })
            .IfSuccessAsync(
                () =>
                {
                    var infoViewModel = serviceFactory.CreateService<InfoViewModel>();
                    infoViewModel.Content = content;
                    infoViewModel.OkTask = view => okTask.Invoke((TView)view);

                    return ShowView(
                        infoViewModel,
                        serviceFactory.CreateService<MainView>().ErrorDialogControl
                    );
                },
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowInfoInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();

        return this.InvokeUiBackgroundAsync(() =>
            {
                setupView.Invoke(content);

                return Result.Success;
            })
            .IfSuccessAsync(
                () =>
                {
                    var infoViewModel = serviceFactory.CreateService<InfoViewModel>();
                    infoViewModel.Content = content;
                    infoViewModel.OkTask = view => okTask.Invoke((TView)view);

                    return ShowView(
                        infoViewModel,
                        serviceFactory.CreateService<MainView>().InputDialogControl
                    );
                },
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowInfoContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();

        return this.InvokeUiBackgroundAsync(() =>
            {
                setupView.Invoke(content);

                return Result.Success;
            })
            .IfSuccessAsync(
                () =>
                {
                    var infoViewModel = serviceFactory.CreateService<InfoViewModel>();
                    infoViewModel.Content = content;
                    infoViewModel.OkTask = view => okTask.Invoke((TView)view);

                    return ShowView(
                        infoViewModel,
                        serviceFactory.CreateService<MainView>().ContentDialogControl
                    );
                },
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowInputDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();

        if (content is null)
        {
            return new Result(new NotFoundTypeError(typeof(TView)))
                .ToValueTaskResult()
                .ConfigureAwait(false);
        }

        return this.InvokeUiBackgroundAsync(() =>
            {
                setupView.Invoke(content);

                return Result.Success;
            })
            .IfSuccessAsync(
                () =>
                    ShowView(content, serviceFactory.CreateService<MainView>().InputDialogControl),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> CloseProgressDialogAsync(CancellationToken ct)
    {
        return this.PostUiBackground(
                () =>
                    SafeCloseUi(serviceFactory.CreateService<MainView>().ProgressDialogControl, ct),
                ct
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

    public ConfiguredValueTaskAwaitable<Result> ShowConfirmContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> initialized,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        var confirmViewModel = serviceFactory.CreateService<ConfirmViewModel>();
        confirmViewModel.Content = content;
        confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);
        confirmViewModel.Initialized = view => initialized.Invoke((TView)view);

        return this.PostUiBackground(
                () =>
                {
                    setupView.Invoke(content);

                    return Result.Success;
                },
                ct
            )
            .IfSuccessAsync(
                () =>
                    ShowView(
                        confirmViewModel,
                        serviceFactory.CreateService<MainView>().ContentDialogControl
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowConfirmInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> initialized,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        var confirmViewModel = serviceFactory.CreateService<ConfirmViewModel>();
        confirmViewModel.Content = content;
        confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);
        confirmViewModel.Initialized = view => initialized.Invoke((TView)view);

        return this.PostUiBackground(
                () =>
                {
                    setupView.Invoke(content);

                    return Result.Success;
                },
                ct
            )
            .IfSuccessAsync(
                () =>
                    ShowView(
                        confirmViewModel,
                        serviceFactory.CreateService<MainView>().InputDialogControl
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> CloseContentDialogAsync(CancellationToken ct)
    {
        return this.InvokeUiBackgroundAsync(
            () => SafeCloseUi(serviceFactory.CreateService<MainView>().ContentDialogControl, ct)
        );
    }

    public ConfiguredValueTaskAwaitable<Result> CloseErrorDialogAsync(CancellationToken ct)
    {
        return this.InvokeUiBackgroundAsync(
            () => SafeCloseUi(serviceFactory.CreateService<MainView>().ErrorDialogControl, ct)
        );
    }

    public ConfiguredValueTaskAwaitable<Result> CloseInputDialogAsync(CancellationToken ct)
    {
        return this.InvokeUiBackgroundAsync(
            () => SafeCloseUi(serviceFactory.CreateService<MainView>().InputDialogControl, ct)
        );
    }

    private ConfiguredValueTaskAwaitable<Result> ShowView(
        object content,
        DialogControl dialogControl
    )
    {
        return this.InvokeUiBackgroundAsync(() =>
        {
            if (dialogControl.IsOpen)
            {
                return Result.Success;
            }

            dialogControl.Dialog = content;
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
}
