namespace Spravy.Ui.Features.ErrorHandling.ViewModels;

public class ErrorViewModel : DialogableViewModelBase
{
    private readonly AvaloniaList<Error> errors = new();

    public ErrorViewModel(IEnumerable<Error> errors)
    {
        this.errors.AddRange(errors);
    }

    public override string ViewId
    {
        get => TypeCache<ErrorViewModel>.Type.Name;
    }

    public IEnumerable<Error> Errors => errors;

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
