namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class PasswordItemSettingsViewModel : ViewModelBase
{
    public PasswordItemSettingsViewModel(PasswordItemEntityNotify item)
    {
        Item = item;
    }

    public PasswordItemEntityNotify Item { get; }
}
