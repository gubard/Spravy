namespace Spravy.Ui.Models;

public class AccountNotify : NotifyBase
{
    private string login = string.Empty;

    public string Login
    {
        get => login;
        set => this.RaiseAndSetIfChanged(ref login, value);
    }
}