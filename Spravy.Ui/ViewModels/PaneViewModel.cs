namespace Spravy.Ui.ViewModels;

public class PaneViewModel : ViewModelBase
{
    public PaneViewModel(AccountNotify account)
    {
        Account = account;
        Account.PropertyChanged += OnPropertyChanged;
    }

    public AccountNotify Account { get; }

    public bool IsShowPasswordGenerator
    {
        get => Account.Login is "vafnir" or "admin";
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Account.Login))
        {
            OnPropertyChanged(nameof(IsShowPasswordGenerator));
        }
    }
}
