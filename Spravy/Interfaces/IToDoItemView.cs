using System.Windows.Input;

namespace Spravy.Interfaces;

public interface IToDoItemView
{
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
}