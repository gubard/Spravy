namespace Spravy.Ui.Features.Authentication.Settings;

public class LoginViewModelSetting : IViewModelSetting<LoginViewModelSetting>
{
    public LoginViewModelSetting(LoginViewModel viewModel)
    {
        Login = viewModel.Login;
    }

    public LoginViewModelSetting() { }

    static LoginViewModelSetting()
    {
        Default = new();
    }

    public string Login { get; set; } = string.Empty;
    public static LoginViewModelSetting Default { get; }
}
