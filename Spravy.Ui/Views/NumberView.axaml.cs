using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class NumberView : ReactiveUserControl<NumberViewModel>
{
    public NumberView()
    {
        InitializeComponent();
    }
}