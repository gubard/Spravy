using Avalonia.ReactiveUI;
using Spravy.Ui.Features.Authentication.ViewModels;

namespace Spravy.Ui.Features.Authentication.Views;

public partial class VerificationCodeView : ReactiveUserControl<VerificationCodeViewModel>
{
    public VerificationCodeView()
    {
        InitializeComponent();
    }
}