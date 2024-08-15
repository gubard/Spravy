namespace Spravy.Ui.Setting;

public class EmailOrLoginInputViewModelSetting
    : IViewModelSetting<EmailOrLoginInputViewModelSetting>
{
    public static EmailOrLoginInputViewModelSetting Default { get; } = new();

    public EmailOrLoginInputViewModelSetting() { }

    public EmailOrLoginInputViewModelSetting(EmailOrLoginInputViewModel viewModel)
    {
        EmailOrLogin = viewModel.EmailOrLogin;
    }

    public string EmailOrLogin { get; set; } = string.Empty;
}
