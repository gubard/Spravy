namespace Spravy.Ui.Features.Authentication.Views;

public partial class LoginView : MainUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
        DefaultFocusTextBox = LoginTextBox;

        Initialized += (s, e) =>
        {
            if (s is not LoginView view)
            {
                return;
            }

            UiHelper.LoginViewInitialized.Execute(view);
        };
    }
}
