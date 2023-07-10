using System.Windows.Input;

namespace Spravy.Ui.Interfaces;

public interface IToDoItemView
{
    public ICommand SkipSubToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
}