namespace Spravy.Ui.Setting;

public class EmailOrLoginInputViewModelSetting : IViewModelSetting<EmailOrLoginInputViewModelSetting>
{
    public EmailOrLoginInputViewModelSetting()
    {
    }

    public EmailOrLoginInputViewModelSetting(EmailOrLoginInputViewModel viewModel)
    {
        EmailOrLogin = viewModel.EmailOrLogin;
    }

    public string EmailOrLogin { get; set; } = string.Empty;
    public static EmailOrLoginInputViewModelSetting Default { get; } = new();
}