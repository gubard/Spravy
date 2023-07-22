using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.Core.Ui.Models;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Ui.ViewModels;

public class AddRootToDoItemViewModel : RoutableViewModelBase
{
    private string name = string.Empty;

    public AddRootToDoItemViewModel() : base("add-root-to-do")
    {
        AddRootToDoItemCommand = CreateCommandFromTask(AddRootToDoItemAsync);
    }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required AppConfiguration Configuration { get; init; }

    public ICommand AddRootToDoItemCommand { get; }


    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    private async Task AddRootToDoItemAsync()
    {
        var options = Mapper.Map<AddRootToDoItemOptions>(this);
        await ToDoService.AddRootToDoItemAsync(options);
        Navigator.NavigateTo(Configuration.ViewPipe[GetType()]);
        DialogViewer.CloseDialog();
    }
}