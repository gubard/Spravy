using System.Windows.Input;

namespace Spravy.Ui.Interfaces;

public interface IChangePinnedToDoItemCommand
{
    ICommand AddSubToDoItemToPinnedCommand { get; }
    ICommand RemoveSubToDoItemFromPinnedCommand { get; }
}