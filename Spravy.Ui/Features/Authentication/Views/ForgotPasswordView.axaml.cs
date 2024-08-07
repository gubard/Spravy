namespace Spravy.Ui.Features.Authentication.Views;

public partial class ForgotPasswordView : UserControl
{
    public ForgotPasswordView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ForgotPasswordView view)
            {
                return;
            }

            if (view.DataContext is not ForgotPasswordViewModel viewModel)
            {
                return;
            }

            UiHelper.ForgotPasswordViewInitialized.Execute(viewModel);
        };
    }
}
