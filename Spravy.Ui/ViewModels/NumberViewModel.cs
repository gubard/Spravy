namespace Spravy.Ui.ViewModels;

public class NumberViewModel : ViewModelBase
{
    [Reactive]
    public decimal Value { get; set; }
}