using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public const string LoginTextBoxName = "login-text-box";
    public const string PasswordTextBoxName = "password-text-box";
    public const string CreateUserButtonName = "create-user-button";
    public const string LoginButtonName = "login-button";
    public const string RememberMeCheckBoxName = "remember-me-check-box";

    public LoginView()
    {
        InitializeComponent();
    }
}
