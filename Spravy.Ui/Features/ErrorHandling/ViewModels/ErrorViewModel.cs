namespace Spravy.Ui.Features.ErrorHandling.ViewModels;

public class ErrorViewModel : DialogableViewModelBase
{
    private readonly AvaloniaList<Error> errors = new();

    public ErrorViewModel(
        IEnumerable<Error> errors,
        IClipboardService clipboardService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.errors.AddRange(errors);

        CopyErrorCommand = SpravyCommand.Create(
            ct => clipboardService.SetTextAsync(
                this.errors.Select(x => $"{x.Id}{Environment.NewLine}{x.Message}").JoinString(Environment.NewLine),
                ct
            ),
            errorHandler,
            taskProgressService
        );
    }

    public override string ViewId => TypeCache<ErrorViewModel>.Name;
    public SpravyCommand CopyErrorCommand { get; }
    public IEnumerable<Error> Errors => errors;

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