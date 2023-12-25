using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ButtonView : ReactiveUserControl<ButtonViewModel>
{
    public ButtonView()
    {
        InitializeComponent();
    }
}