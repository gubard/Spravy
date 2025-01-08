namespace Spravy.Ui.ViewModels;

public partial class MainProgressBarViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool isIndeterminate;

    [ObservableProperty]
    private double maximum;

    [ObservableProperty]
    private double value;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Value))
        {
            IsIndeterminate = Math.Abs(Value) < 0.1;
        }
    }
}