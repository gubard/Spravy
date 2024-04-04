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

    public ValueTask<Result> ShowContentDialogAsync<TView>(
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
            .ConfigureAwait(false)
            .IfSuccessAsync(() => ShowView(content, ContentDialogHostIdentifier).ConfigureAwait(false));
    }

    public ValueTask<Result> ShowProgressDialogAsync<TView>(
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
            .ConfigureAwait(false)
            .IfSuccessAsync(() => ShowView(content, ProgressDialogHostIdentifier).ConfigureAwait(false));
    }

    public ValueTask<Result> ShowErrorDialogAsync<TView>(
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
            .ConfigureAwait(false)
            .IfSuccessAsync(() => ShowView(content, ErrorDialogHostIdentifier).ConfigureAwait(false));
    }

    public ValueTask<Result> ShowInfoErrorDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .ConfigureAwait(false)
            .IfSuccessAsync(
                () =>
                {
                    var infoViewModel = Resolver.Get<InfoViewModel>();
                    infoViewModel.Content = content;
                    infoViewModel.OkTask = view => okTask.Invoke((TView)view);
                    cancellationToken.ThrowIfCancellationRequested();

                    return ShowView(infoViewModel, ErrorDialogHostIdentifier).ConfigureAwait(false);
                }
            );
    }

    public ValueTask<Result> ShowInfoInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .ConfigureAwait(false)
            .IfSuccessAsync(
                () =>
                {
                    var infoViewModel = Resolver.Get<InfoViewModel>();
                    infoViewModel.Content = content;
                    infoViewModel.OkTask = view => okTask.Invoke((TView)view);
                    cancellationToken.ThrowIfCancellationRequested();

                    return ShowView(infoViewModel, InputDialogHostIdentifier).ConfigureAwait(false);
                }
            );
    }

    public ValueTask<Result> ShowInfoContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();
        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .ConfigureAwait(false)
            .IfSuccessAsync(
                () =>
                {
                    var infoViewModel = Resolver.Get<InfoViewModel>();
                    infoViewModel.Content = content;
                    infoViewModel.OkTask = view => okTask.Invoke((TView)view);
                    cancellationToken.ThrowIfCancellationRequested();

                    return ShowView(infoViewModel, ContentDialogHostIdentifier).ConfigureAwait(false);
                }
            );
    }

    public ValueTask<Result> ShowInputDialogAsync<TView>(
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
            .ConfigureAwait(false)
            .IfSuccessAsync(() => ShowView(content, InputDialogHostIdentifier).ConfigureAwait(false));
    }

    public ValueTask<Result> CloseProgressDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ProgressDialogHostIdentifier);
    }

    public ValueTask<Result<bool>> CloseLastDialogAsync(CancellationToken cancellationToken)
    {
        if (DialogHost.IsDialogOpen(ProgressDialogHostIdentifier))
        {
            return SafeClose(ProgressDialogHostIdentifier)
                .ConfigureAwait(false)
                .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false));
        }

        if (DialogHost.IsDialogOpen(ErrorDialogHostIdentifier))
        {
            return SafeClose(ErrorDialogHostIdentifier)
                .ConfigureAwait(false)
                .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false));
        }

        if (DialogHost.IsDialogOpen(InputDialogHostIdentifier))
        {
            return SafeClose(InputDialogHostIdentifier)
                .ConfigureAwait(false)
                .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false));
        }

        if (DialogHost.IsDialogOpen(ContentDialogHostIdentifier))
        {
            return SafeClose(ContentDialogHostIdentifier)
                .ConfigureAwait(false)
                .IfSuccessAsync(() => true.ToResult().ToValueTaskResult().ConfigureAwait(false));
        }

        return false.ToResult().ToValueTaskResult();
    }

    public ValueTask<Result> ShowConfirmContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .ConfigureAwait(false)
            .IfSuccessAsync(
                () =>
                {
                    var confirmViewModel = Resolver.Get<ConfirmViewModel>();
                    confirmViewModel.Content = content;
                    confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
                    confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);
                    cancellationToken.ThrowIfCancellationRequested();

                    return ShowView(confirmViewModel, ContentDialogHostIdentifier).ConfigureAwait(false);
                }
            );
    }

    public ValueTask<Result> ShowConfirmInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        return this.InvokeUIBackgroundAsync(() => setupView.Invoke(content))
            .ConfigureAwait(false)
            .IfSuccessAsync(
                () =>
                {
                    var confirmViewModel = Resolver.Get<ConfirmViewModel>();
                    confirmViewModel.Content = content;
                    confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
                    confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);
                    cancellationToken.ThrowIfCancellationRequested();

                    return ShowView(confirmViewModel, InputDialogHostIdentifier).ConfigureAwait(false);
                }
            );
    }

    public ValueTask<Result> CloseContentDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ContentDialogHostIdentifier);
    }

    public ValueTask<Result> CloseErrorDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ErrorDialogHostIdentifier);
    }

    public ValueTask<Result> CloseInputDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(InputDialogHostIdentifier);
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

    private ValueTask<Result> SafeClose(string identifier)
    {
        if (!DialogHost.IsDialogOpen(identifier))
        {
            return Result.SuccessValueTask;
        }

        var content = DialogHost.GetDialogSession(identifier).ThrowIfNull().Content.ThrowIfNull();

        return Result.AwaitableFalse.IfSuccessAsync(
                () =>
                {
                    if (content is ISaveState saveState)
                    {
                        return saveState.SaveStateAsync().ConfigureAwait(false);
                    }

                    return Result.AwaitableFalse;
                }
            )
            .ConfigureAwait(false)
            .IfSuccessAsync(
                () => this.InvokeUIBackgroundAsync(() => DialogHost.Close(identifier)).ConfigureAwait(false)
            );
    }
}