namespace Spravy.Ui.ViewModels;

public class ConfirmViewModel : DialogableViewModelBase
{
    private readonly Func<IDialogable, Cvtar> cancelTask;
    private readonly Func<IDialogable, Cvtar> confirmTask;

    public ConfirmViewModel(
        IDialogable content,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        Func<IDialogable, Cvtar> confirmTask,
        Func<IDialogable, Cvtar> cancelTask
    )
    {
        this.confirmTask = confirmTask;
        this.cancelTask = cancelTask;
        Content = content;
        ConfirmCommand = SpravyCommand.Create(ConfirmAsync, errorHandler, taskProgressService);
        CancelCommand = SpravyCommand.Create(CancelAsync, errorHandler, taskProgressService);
    }

    public SpravyCommand CancelCommand { get; }
    public SpravyCommand ConfirmCommand { get; }
    public IDialogable Content { get; }

    public override string ViewId => TypeCache<ConfirmViewModel>.Name;

    private Cvtar CancelAsync(CancellationToken ct)
    {
        return cancelTask.Invoke(Content);
    }

    private Cvtar ConfirmAsync(CancellationToken ct)
    {
        return confirmTask.Invoke(Content);
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Content.LoadStateAsync(ct);
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Content.SaveStateAsync(ct);
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Content.RefreshAsync(ct);
    }
}