using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddRootToDoItemViewModel : RoutableViewModelBase
{
    private string name = string.Empty;

    public AddRootToDoItemViewModel() : base("add-root-to-do")
    {
    }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }
}