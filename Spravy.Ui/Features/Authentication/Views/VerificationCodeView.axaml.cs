namespace Spravy.Ui.Features.Authentication.Views;

public partial class VerificationCodeView : ReactiveUserControl<VerificationCodeViewModel>
{
    public const string VerificationCodeTextBoxName = "verification-code-text-box";
    public const string VerificationEmailButtonName = "verification-email-button";

    public VerificationCodeView()
    {
        InitializeComponent();
    }
}