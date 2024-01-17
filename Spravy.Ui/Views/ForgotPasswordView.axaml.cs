using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ForgotPasswordView : ReactiveUserControl<ForgotPasswordViewModel>
{
    public ForgotPasswordView()
    {
        InitializeComponent();
    }
}