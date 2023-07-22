using System.Windows.Input;

namespace Spravy.Ui.Interfaces;

public interface ICompleteSubToDoItemCommand
{
    public ICommand CompleteSubToDoItemCommand { get; }
}