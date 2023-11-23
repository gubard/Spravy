using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class NumberViewModel : ViewModelBase
{
    private decimal value;

    public decimal Value
    {
        get => value;
        set => this.RaiseAndSetIfChanged(ref this.value, value);
    }
}