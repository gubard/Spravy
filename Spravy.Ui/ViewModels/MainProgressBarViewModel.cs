namespace Spravy.Ui.ViewModels;

public partial class MainProgressBarViewModel : ViewModelBase
{
    [ObservableProperty]
    private double value;

    [ObservableProperty]
    private double maximum;

    [ObservableProperty]
    private bool isIndeterminate;

    public MainProgressBarViewModel()
    {
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Value))
        {
            IsIndeterminate = Math.Abs(Value) < 0.1;
        }
    }
}
