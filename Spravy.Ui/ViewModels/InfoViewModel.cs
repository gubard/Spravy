namespace Spravy.Ui.ViewModels;

public class InfoViewModel : DialogableViewModelBase
{
    private readonly Func<IDialogable, Cvtar> okTask;

    public InfoViewModel(
        IDialogable content,
        Func<IDialogable, Cvtar> okTask,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        Content = content;
        this.okTask = okTask;
        OkCommand = SpravyCommand.Create(OkAsync, errorHandler, taskProgressService);
    }

    public SpravyCommand OkCommand { get; }
    public IDialogable Content { get; }

    public override string ViewId => TypeCache<InfoViewModel>.Name;

    private Cvtar OkAsync(CancellationToken ct)
    {
        return okTask.Invoke(Content);
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