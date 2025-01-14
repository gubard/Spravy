namespace Spravy.Ui.Models;

public partial class AccountNotify : NotifyBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAdmin))]
    private string login = string.Empty;

    public bool IsAdmin => Login is "vafnir" or "admin";
}