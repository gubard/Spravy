using System.Windows.Input;

namespace Spravy.Ui.Interfaces;

public interface IPathView
{
    ICommand ToRootItemCommand { get; }
    ICommand ChangeToDoItemByPathCommand { get; }
}