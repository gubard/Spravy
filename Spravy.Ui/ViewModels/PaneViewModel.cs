namespace Spravy.Ui.ViewModels;

public class PaneViewModel : ViewModelBase
{
    public PaneViewModel(AccountNotify account)
    {
        Account = account;

        Account
            .WhenAnyValue(x => x.Login)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(IsShowPasswordGenerator)));
    }

    public AccountNotify Account { get; }

    public bool IsShowPasswordGenerator
    {
        get => Account.Login is "vafnir" or "admin";
    }
}
