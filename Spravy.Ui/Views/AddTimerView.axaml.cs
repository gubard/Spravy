using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class AddTimerView : ReactiveUserControl<AddTimerViewModel>
{
    public AddTimerView()
    {
        InitializeComponent();
    }
}