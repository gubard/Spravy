using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class VerificationCodeView : ReactiveUserControl<VerificationCodeViewModel>
{
    public VerificationCodeView()
    {
        InitializeComponent();
    }
}