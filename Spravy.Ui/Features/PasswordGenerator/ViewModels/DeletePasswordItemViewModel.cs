namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class DeletePasswordItemViewModel : ViewModelBase
{
    public DeletePasswordItemViewModel(PasswordItemEntityNotify item)
    {
        Item = item;
    }

    public PasswordItemEntityNotify Item { get; }
}
