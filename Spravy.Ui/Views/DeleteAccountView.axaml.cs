using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class DeleteAccountView : ReactiveUserControl<DeleteAccountViewModel>
{
    public DeleteAccountView()
    {
        InitializeComponent();
    }
}