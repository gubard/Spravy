namespace Spravy.Ui.Setting;

public class EmailOrLoginInputViewModelSetting
{
    public EmailOrLoginInputViewModelSetting() { }

    public EmailOrLoginInputViewModelSetting(EmailOrLoginInputViewModel viewModel)
    {
        Identifier = viewModel.EmailOrLogin;
    }

    public string Identifier { get; set; } = string.Empty;
}
