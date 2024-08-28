namespace Spravy.Ui.Features.ErrorHandling.ViewModels;

public class ExceptionViewModel : DialogableViewModelBase
{
    private readonly Exception exception;

    public ExceptionViewModel(Exception exception)
    {
        this.exception = exception;
    }

    public string Message => $"{exception.GetType()}{Environment.NewLine}{exception.Message}";

    public override string ToString()
    {
        return exception.ToString();
    }

    public override string ViewId
    {
        get => $"{TypeCache<ExceptionViewModel>.Type}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
