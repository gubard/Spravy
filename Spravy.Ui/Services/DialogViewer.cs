using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;
using DialogHostAvalonia;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;

namespace Spravy.Ui.Services;

public class DialogViewer : IDialogViewer
{
    public const string ContentDialogHostIdentifier = "ContentDialogHost";
    public const string ErrorDialogHostIdentifier = "ErrorDialogHost";
    public const string InputDialogHostIdentifier = "InputDialogHost";
    public const string ProgressDialogHostIdentifier = "ProgressDialogHost";

    [Inject]
    public required IKernel Resolver { get; init; }

    public ConfiguredValueTaskAwaitable<Result> ShowContentDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .IfSuccessAsync(
                () => ShowView(content, ContentDialogHostIdentifier).ConfigureAwait(false),
                cancellationToken
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowProgressDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .IfSuccessAsync(
                () => ShowView(content, ProgressDialogHostIdentifier).ConfigureAwait(false),
                cancellationToken
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowErrorDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .IfSuccessAsync(
                () => ShowView(content, ErrorDialogHostIdentifier).ConfigureAwait(false),
                cancellationToken
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowInfoErrorDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .IfSuccessAsync(
                () =>
                {
                    var infoViewModel = Resolver.Get<InfoViewModel>();
                    infoViewModel.Content = content;
                    infoViewModel.OkTask = view => okTask.Invoke((TView)view);
                    cancellationToken.ThrowIfCancellationRequested();

                    return ShowView(infoViewModel, ErrorDialogHostIdentifier).ConfigureAwait(false);
                },
                cancellationToken
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowInfoInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .IfSuccessAsync(
                () =>
                {
                    var infoViewModel = Resolver.Get<InfoViewModel>();
                    infoViewModel.Content = content;
                    infoViewModel.OkTask = view => okTask.Invoke((TView)view);
                    cancellationToken.ThrowIfCancellationRequested();

                    return ShowView(infoViewModel, InputDialogHostIdentifier).ConfigureAwait(false);
                },
                cancellationToken
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowInfoContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();
        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .IfSuccessAsync(
                () =>
                {
                    var infoViewModel = Resolver.Get<InfoViewModel>();
                    infoViewModel.Content = content;
                    infoViewModel.OkTask = view => okTask.Invoke((TView)view);
                    cancellationToken.ThrowIfCancellationRequested();

                    return ShowView(infoViewModel, ContentDialogHostIdentifier).ConfigureAwait(false);
                },
                cancellationToken
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowInputDialogAsync<TView>(
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .IfSuccessAsync(
                () => ShowView(content, InputDialogHostIdentifier).ConfigureAwait(false),
                cancellationToken
            );
    }

    public ConfiguredValueTaskAwaitable<Result> CloseProgressDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ProgressDialogHostIdentifier, cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> CloseLastDialogAsync(CancellationToken cancellationToken)
    {
        if (DialogHost.IsDialogOpen(ProgressDialogHostIdentifier))
        {
            return SafeClose(ProgressDialogHostIdentifier, cancellationToken)
                .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false), cancellationToken);
        }

        if (DialogHost.IsDialogOpen(ErrorDialogHostIdentifier))
        {
            return SafeClose(ErrorDialogHostIdentifier, cancellationToken)
                .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false), cancellationToken);
        }

        if (DialogHost.IsDialogOpen(InputDialogHostIdentifier))
        {
            return SafeClose(InputDialogHostIdentifier, cancellationToken)
                .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false), cancellationToken);
        }

        if (DialogHost.IsDialogOpen(ContentDialogHostIdentifier))
        {
            return SafeClose(ContentDialogHostIdentifier, cancellationToken)
                .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false), cancellationToken);
        }

        return false.ToResult().ToValueTaskResult().ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> ShowConfirmContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .IfSuccessAsync(
                () =>
                {
                    var confirmViewModel = Resolver.Get<ConfirmViewModel>();
                    confirmViewModel.Content = content;
                    confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
                    confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);
                    cancellationToken.ThrowIfCancellationRequested();

                    return ShowView(confirmViewModel, ContentDialogHostIdentifier).ConfigureAwait(false);
                },
                cancellationToken
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ShowConfirmInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .IfSuccessAsync(
                () =>
                {
                    var confirmViewModel = Resolver.Get<ConfirmViewModel>();
                    confirmViewModel.Content = content;
                    confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
                    confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);
                    cancellationToken.ThrowIfCancellationRequested();

                    return ShowView(confirmViewModel, InputDialogHostIdentifier).ConfigureAwait(false);
                },
                cancellationToken
            );
    }

    public ConfiguredValueTaskAwaitable<Result> CloseContentDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ContentDialogHostIdentifier, cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> CloseErrorDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ErrorDialogHostIdentifier, cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> CloseInputDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(InputDialogHostIdentifier, cancellationToken);
    }

    private ValueTask<Result> ShowView(object content, string identifier)
    {
        if (DialogHost.IsDialogOpen(identifier))
        {
            return Result.SuccessValueTask;
        }

        return this.InvokeUIBackgroundAsync(
            () => DialogHost.Show(
                content,
                identifier
            )
        );
    }

    private ConfiguredValueTaskAwaitable<Result> SafeClose(string identifier, CancellationToken cancellationToken)
    {
        if (!DialogHost.IsDialogOpen(identifier))
        {
            return Result.AwaitableFalse;
        }

        var content = DialogHost.GetDialogSession(identifier).ThrowIfNull().Content.ThrowIfNull();

        return Result.AwaitableFalse.IfSuccessAsync(
                () =>
                {
                    if (content is ISaveState saveState)
                    {
                        return saveState.SaveStateAsync(cancellationToken);
                    }

                    return Result.AwaitableFalse;
                },
                cancellationToken
            )
            .IfSuccessAsync(
                () => this.InvokeUIBackgroundAsync(() => DialogHost.Close(identifier)),
                cancellationToken
            );
    }
}