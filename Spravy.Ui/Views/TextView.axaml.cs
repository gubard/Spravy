using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class TextView : ReactiveUserControl<TextViewModel>
{
    public TextView()
    {
        InitializeComponent();
    }
}