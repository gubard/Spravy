namespace Spravy.Ui.Features.ErrorHandling.ViewModels;

public class ExceptionViewModel : ViewModelBase
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
}
