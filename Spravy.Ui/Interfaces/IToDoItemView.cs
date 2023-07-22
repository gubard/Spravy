using System.Windows.Input;

namespace Spravy.Ui.Interfaces;

public interface IToDoItemView
{
    ICommand DeleteSubToDoItemCommand { get; }
    ICommand ChangeToDoItemCommand { get; }
    ICommand ChangeParentToDoItemCommand { get; }
}