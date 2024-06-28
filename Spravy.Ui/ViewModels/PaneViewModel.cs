namespace Spravy.Ui.ViewModels;

public class PaneViewModel : ViewModelBase
{
    public PaneViewModel(AccountNotify account)
    {
        Account = account;
    }

    public AccountNotify Account { get; }
}
