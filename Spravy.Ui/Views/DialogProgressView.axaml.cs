using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class DialogProgressView : ReactiveUserControl<DialogProgressViewModel>
{
    public DialogProgressView()
    {
        InitializeComponent();
    }
}