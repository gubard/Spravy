namespace Spravy.Ui.Services;

public class DialogViewer : IDialogViewer
{
    public const string ContentDialogHostIdentifier = "ContentDialogHost";
    public const string ErrorDialogHostIdentifier = "ErrorDialogHost";
    public const string InputDialogHostIdentifier = "InputDialogHost";
    public const string ProgressDialogHostIdentifier = "ProgressDialogHost";
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
            .IfSuccessAsync(() => ShowView(content, ContentDialogHostIdentifier), ct);
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
            .IfSuccessAsync(() => ShowView(content, ProgressDialogHostIdentifier), ct);
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
            .IfSuccessAsync(() => ShowView(content, ErrorDialogHostIdentifier), ct);
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

                    return ShowView(infoViewModel, ErrorDialogHostIdentifier);
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

                    return ShowView(infoViewModel, InputDialogHostIdentifier);
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

                    return ShowView(infoViewModel, ContentDialogHostIdentifier);
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
            .IfSuccessAsync(() => ShowView(content, InputDialogHostIdentifier), ct);
    }

    public ConfiguredValueTaskAwaitable<Result> CloseProgressDialogAsync(CancellationToken ct)
    {
        return SafeClose(ProgressDialogHostIdentifier, ct);
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> CloseLastDialogAsync(CancellationToken ct)
    {
        if (DialogHost.IsDialogOpen(ProgressDialogHostIdentifier))
        {
            return SafeClose(ProgressDialogHostIdentifier, ct)
                .IfSuccessAsync(
                    () => true.ToResult().ToValueTaskResult().ConfigureAwait(false),
                    ct
                );
        }

        if (DialogHost.IsDialogOpen(ErrorDialogHostIdentifier))
        {
            return SafeClose(ErrorDialogHostIdentifier, ct)
                .IfSuccessAsync(
                    () => true.ToResult().ToValueTaskResult().ConfigureAwait(false),
                    ct
                );
        }

        if (DialogHost.IsDialogOpen(InputDialogHostIdentifier))
        {
            return SafeClose(InputDialogHostIdentifier, ct)
                .IfSuccessAsync(
                    () => true.ToResult().ToValueTaskResult().ConfigureAwait(false),
                    ct
                );
        }

        if (DialogHost.IsDialogOpen(ContentDialogHostIdentifier))
        {
            return SafeClose(ContentDialogHostIdentifier, ct)
                .IfSuccessAsync(
                    () => true.ToResult().ToValueTaskResult().ConfigureAwait(false),
                    ct
                );
        }

        return false.ToResult().ToValueTaskResult().ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> ShowConfirmContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        var confirmViewModel = serviceFactory.CreateService<ConfirmViewModel>();
        confirmViewModel.Content = content;
        confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);

        confirmViewModel.Initialized = () =>
            this.InvokeUiBackgroundAsync(() =>
            {
                setupView.Invoke(content);

                return Result.Success;
            });

        return ShowView(confirmViewModel, ContentDialogHostIdentifier);
    }

    public ConfiguredValueTaskAwaitable<Result> ShowConfirmInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken ct
    )
        where TView : ViewModelBase
    {
        var content = serviceFactory.CreateService<TView>();
        var confirmViewModel = serviceFactory.CreateService<ConfirmViewModel>();
        confirmViewModel.Content = content;
        confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);

        confirmViewModel.Initialized = () =>
            this.InvokeUiBackgroundAsync(() =>
            {
                setupView.Invoke(content);

                return Result.Success;
            });

        return ShowView(confirmViewModel, InputDialogHostIdentifier);
    }

    public ConfiguredValueTaskAwaitable<Result> CloseContentDialogAsync(CancellationToken ct)
    {
        return SafeClose(ContentDialogHostIdentifier, ct);
    }

    public ConfiguredValueTaskAwaitable<Result> CloseErrorDialogAsync(CancellationToken ct)
    {
        return SafeClose(ErrorDialogHostIdentifier, ct);
    }

    public ConfiguredValueTaskAwaitable<Result> CloseInputDialogAsync(CancellationToken ct)
    {
        return SafeClose(InputDialogHostIdentifier, ct);
    }

    private ConfiguredValueTaskAwaitable<Result> ShowView(object content, string identifier)
    {
        if (DialogHost.IsDialogOpen(identifier))
        {
            return Result.AwaitableSuccess;
        }

        return this.InvokeUiBackgroundAsync(() =>
        {
            DialogHost.Show(content, identifier);

            return Result.Success;
        });
    }

    private ConfiguredValueTaskAwaitable<Result> SafeClose(string identifier, CancellationToken ct)
    {
        if (!DialogHost.IsDialogOpen(identifier))
        {
            return Result.AwaitableSuccess;
        }

        return this.InvokeUiBackgroundAsync(
                () =>
                    DialogHost
                        .GetDialogSession(identifier)
                        .IfNotNull("DialogSession")
                        .IfSuccess(session => session.Content.IfNotNull(nameof(session.Content)))
            )
            .IfSuccessAsync(
                content =>
                {
                    if (content is ISaveState saveState)
                    {
                        return saveState.SaveStateAsync(ct);
                    }

                    return Result.AwaitableSuccess;
                },
                ct
            )
            .IfSuccessAsync(
                () =>
                    this.InvokeUiBackgroundAsync(() =>
                    {
                        DialogHost.Close(identifier);

                        return Result.Success;
                    }),
                ct
            );
    }
}
