namespace Spravy.Ui.Features.ErrorHandling.ViewModels;

public class ErrorViewModel : NavigatableViewModelBase
{
    private readonly AvaloniaList<Error> errors = new();

    public ErrorViewModel(IEnumerable<Error> errors)
        : base(true)
    {
        this.errors.AddRange(errors);
    }

    public override string ViewId
    {
        get => TypeCache<ErrorViewModel>.Type.Name;
    }

    public IEnumerable<Error> Errors => errors;

    public override Result Stop()
    {
        return Result.Success;
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
