using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using ExtensionFramework.AvaloniaUi.Controls;
using ExtensionFramework.AvaloniaUi.Interfaces;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Interfaces;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Core.Enums;
using Spravy.Core.Interfaces;
using Spravy.Interfaces;
using Spravy.Models;

namespace Spravy.ViewModels;

public class ToDoItemViewModel : RoutableViewModelBase, IItemsViewModel<ToDoItemNotify>, IToDoItemOrderChanger
{
    private string name = string.Empty;
    private Guid id;
    private TypeOfPeriodicity typeOfPeriodicity;
    private DateTimeOffset? dueDate;
    private bool isComplete;

    public ToDoItemViewModel() : base("to-do-item")
    {
        DeleteSubToDoItemCommand = CreateCommandFromTask<ToDoItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommand<ToDoItemNotify>(ChangeToDoItem);
        AddToDoItemCommand = CreateCommand(AddToDoItem);
        ChangeToDoItemByPathCommand = CreateCommand<ToDoItemParentNotify>(ChangeToDoItemByPath);
        ToRootItemCommand = CreateCommand(ToRootItem);
        TypeOfPeriodicities = new(Enum.GetValues<TypeOfPeriodicity>());
        this.WhenAnyValue(x => x.DueDate).Subscribe(OnNextDueDate);
        this.WhenAnyValue(x => x.TypeOfPeriodicity).Subscribe(OnNextTypeOfPeriodicity);
        this.WhenAnyValue(x => x.Id).Subscribe(OnNextId);
        this.WhenAnyValue(x => x.IsComplete).Subscribe(OnNextIsComplete);
        this.WhenAnyValue(x => x.Name).Subscribe(OnNextName);
    }

    public AvaloniaList<TypeOfPeriodicity> TypeOfPeriodicities { get; }
    public AvaloniaList<ToDoItemNotify> Items { get; } = new();
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand AddToDoItemCommand { get; }
    public ICommand ChangeToDoItemByPathCommand { get; }
    public ICommand ToRootItemCommand { get; }

    [Inject]
    public required IToDoService? ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Inject]
    public required PathControl? Path { get; set; }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public bool IsComplete
    {
        get => isComplete;
        set => this.RaiseAndSetIfChanged(ref isComplete, value);
    }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public DateTimeOffset? DueDate
    {
        get => dueDate;
        set => this.RaiseAndSetIfChanged(ref dueDate, value);
    }

    public TypeOfPeriodicity TypeOfPeriodicity
    {
        get => typeOfPeriodicity;
        set => this.RaiseAndSetIfChanged(ref typeOfPeriodicity, value);
    }

    private async void OnNextId(Guid x)
    {
        await RefreshToDoItemAsync();
    }

    private async void OnNextTypeOfPeriodicity(TypeOfPeriodicity x)
    {
        if (ToDoService is null)
        {
            return;
        }

        await ToDoService.UpdateTypeOfPeriodicityAsync(Id, x);
        await RefreshToDoItemAsync();
    }

    private async void OnNextName(string x)
    {
        if (ToDoService is null)
        {
            return;
        }

        await ToDoService.UpdateNameToDoItemAsync(Id, x);
        await RefreshToDoItemAsync();
    }

    private void AddToDoItem()
    {
        Navigator.NavigateTo<AddToDoItemViewModel>(vm => vm.Parent = Mapper.Map<ToDoItemNotify>(this));
    }

    private void ChangeToDoItemByPath(ToDoItemParentNotify item)
    {
        Id = item.Id;
    }

    private async void OnNextIsComplete(bool x)
    {
        if (ToDoService is null)
        {
            return;
        }

        await ToDoService.UpdateCompleteStatusAsync(Id, x);
        await RefreshToDoItemAsync();
    }

    private void ChangeToDoItem(ToDoItemNotify item)
    {
        Id = item.Id;
    }

    private void ToRootItem()
    {
        Navigator.NavigateTo<RootToDoItemViewModel>();
    }

    private async Task DeleteSubToDoItemAsync(ToDoItemNotify item)
    {
        await ToDoService.DeleteToDoItemAsync(item.Id);
        await RefreshToDoItemAsync();
    }

    private async void OnNextDueDate(DateTimeOffset? x)
    {
        if (ToDoService is null)
        {
            return;
        }

        await ToDoService.UpdateDueDateAsync(Id, x);
    }

    public async Task RefreshToDoItemAsync()
    {
        if (Path is null)
        {
            return;
        }

        if (ToDoService is null)
        {
            return;
        }

        Path.Items ??= new AvaloniaList<object>();
        var item = await ToDoService.GetToDoItemAsync(Id);
        DueDate = item.DueDate;
        Name = item.Name;
        IsComplete = item.IsComplete;
        TypeOfPeriodicity = item.TypeOfPeriodicity;
        Items.Clear();
        Items.AddRange(item.Items.Select(x => Mapper.Map<ToDoItemNotify>(x)).OrderBy(x => x.OrderIndex));
        SubscribeItems();
        Path.Items.Clear();
        Path.Items.Add(new RootItem());
        Path.Items.AddRange(item.Parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));
    }

    private void SubscribeItems()
    {
        foreach (var itemNotify in Items)
        {
            async void OnNextIsComplete(bool x)
            {
                if (ToDoService is null)
                {
                    return;
                }

                try
                {
                    await ToDoService.UpdateCompleteStatusAsync(itemNotify.Id, x);
                }
                catch (Exception e)
                {
                    Navigator.NavigateTo<IExceptionViewModel>(viewModel => viewModel.Exception = e);
                }
            }

            itemNotify.WhenAnyValue(x => x.IsComplete).Subscribe(OnNextIsComplete);
        }
    }
}