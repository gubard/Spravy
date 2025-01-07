namespace Spravy.Ui.Models;

public partial class AccountNotify : NotifyBase
{
    [ObservableProperty]
    private string login = string.Empty;

    public bool IsAdmin => Login is "vafnir" or "admin";

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Login))
        {
            OnPropertyChanged(nameof(IsAdmin));
        }
    }
}