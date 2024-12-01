namespace Spravy.Ui.Features.Authentication.Settings;

public class LoginViewModelSetting : IViewModelSetting<LoginViewModelSetting>
{
    static LoginViewModelSetting()
    {
        Default = new();
    }

    public LoginViewModelSetting(LoginViewModel viewModel)
    {
        Login = viewModel.Login;
    }

    public LoginViewModelSetting()
    {
    }

    public string Login { get; set; } = string.Empty;
    public static LoginViewModelSetting Default { get; }
}