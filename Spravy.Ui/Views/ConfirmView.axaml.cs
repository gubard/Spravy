using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ConfirmView : ReactiveUserControl<ConfirmViewModel>
{
    public ConfirmView()
    {
        InitializeComponent();
    }
}