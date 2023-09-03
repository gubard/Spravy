using System.Windows.Input;
using Ninject;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class PaneViewModel : ViewModelBase
{
    public PaneViewModel()
    {
        ToRootToDoItemViewCommand = CreateCommand(ToRootToDoItemView);
        ToSearchViewCommand = CreateCommand(ToSearchView);
    }

    public ICommand ToRootToDoItemViewCommand { get; }
    public ICommand ToSearchViewCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    private void ToRootToDoItemView()
    {
        Navigator.NavigateTo<RootToDoItemViewModel>();
    }

    private void ToSearchView()
    {
        Navigator.NavigateTo<SearchViewModel>();
    }
}