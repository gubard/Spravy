namespace Spravy.Ui.Features.ErrorHandling.ViewModels;

public class ExceptionViewModel : ViewModelBase
{
    public ExceptionViewModel()
    {
        this.WhenAnyValue(x => x.Exception).Subscribe(x => Message = x?.Message ?? string.Empty);
    }

    [Reactive]
    public Exception? Exception { get; set; }

    [Reactive]
    public string Message { get; set; } = string.Empty;
}
