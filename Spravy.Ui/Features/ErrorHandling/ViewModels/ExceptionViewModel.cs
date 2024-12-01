namespace Spravy.Ui.Features.ErrorHandling.ViewModels;

public class ExceptionViewModel : DialogableViewModelBase
{
    private readonly Exception exception;

    public ExceptionViewModel(
        Exception exception,
        IClipboardService clipboardService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.exception = exception;

        CopyErrorCommand = SpravyCommand.Create(
            ct => clipboardService.SetTextAsync(Message, ct),
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand CopyErrorCommand { get; }
    public string Message => $"{exception.GetType()}{Environment.NewLine}{exception.Message}";
    public override string ViewId => $"{TypeCache<ExceptionViewModel>.Type}";

    public override string ToString()
    {
        return exception.ToString();
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}