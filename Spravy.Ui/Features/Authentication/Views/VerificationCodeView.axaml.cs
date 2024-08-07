namespace Spravy.Ui.Features.Authentication.Views;

public partial class VerificationCodeView : UserControl
{
    public const string VerificationCodeTextBoxName = "verification-code-text-box";
    public const string VerificationEmailButtonName = "verification-email-button";

    public VerificationCodeView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not VerificationCodeView view)
            {
                return;
            }

            if (view.DataContext is not VerificationCodeViewModel viewModel)
            {
                return;
            }

            viewModel.Commands.Initialized.Command.Execute(viewModel);
        };
    }
}
