namespace Spravy.Ui.ViewModels;

public class MainProgressBarViewModel : ViewModelBase
{
    public MainProgressBarViewModel()
    {
        this.WhenAnyValue(x => x.Value).Subscribe(x => IsIndeterminate = Math.Abs(x) < 0.1);
    }

    [Reactive]
    public double Value { get; set; }

    [Reactive]
    public double Maximum { get; set; }

    [Reactive]
    public bool IsIndeterminate { get; set; }
}
