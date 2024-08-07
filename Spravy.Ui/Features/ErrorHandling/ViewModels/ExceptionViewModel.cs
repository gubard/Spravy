namespace Spravy.Ui.Features.ErrorHandling.ViewModels;

public partial class ExceptionViewModel : ViewModelBase
{
    public ExceptionViewModel()
    {
        PropertyChanged += OnPropertyChanged;
    }

    [ObservableProperty]
    private Exception? exception;

    [ObservableProperty]
    private string message = string.Empty;

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Exception))
        {
            OnPropertyChanged(nameof(Message));
        }
    }
}
