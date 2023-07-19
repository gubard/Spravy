using System.Windows.Input;

namespace Spravy.Ui.Interfaces;

public interface IToDoItemView
{
    public ICommand CompleteSubToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand ChangeParentToDoItemCommand { get; }
}