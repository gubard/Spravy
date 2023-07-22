using System.Windows.Input;

namespace Spravy.Ui.Interfaces;

public interface IChangeCurrentStatusToDoItemCommand
{
    ICommand AddSubToDoItemToCurrentCommand { get; }
    ICommand RemoveSubToDoItemFromCurrentCommand { get; }
}