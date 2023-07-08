using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using ExtensionFramework.AvaloniaUi.Controls;
using ExtensionFramework.AvaloniaUi.Interfaces;
using ExtensionFramework.Core.DependencyInjection.Attributes;
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
    private string description = string.Empty;

    public ToDoItemViewModel() : base("to-do-item")
    {
        DeleteSubToDoItemCommand = CreateCommandFromTask<ToDoItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommand<ToDoItemNotify>(ChangeToDoItem);
        AddToDoItemCommand = CreateCommand(AddToDoItem);
        ChangeToDoItemByPathCommand = CreateCommand<ToDoItemParentNotify>(ChangeToDoItemByPath);
        ToRootItemCommand = CreateCommand(ToRootItem);
        TypeOfPeriodicities = new(Enum.GetValues<TypeOfPeriodicity>());
        this.WhenAnyValue(x => x.DueDate).Skip(1).Subscribe(OnNextDueDate);
        this.WhenAnyValue(x => x.TypeOfPeriodicity).Skip(1).Subscribe(OnNextTypeOfPeriodicity);
        this.WhenAnyValue(x => x.Id).Skip(1).Subscribe(OnNextId);
        this.WhenAnyValue(x => x.IsComplete).Skip(1).Subscribe(OnNextIsComplete);
        this.WhenAnyValue(x => x.Name).Skip(1).Subscribe(OnNextName);
        this.WhenAnyValue(x => x.Description).Skip(1).Subscribe(OnNextDescription);
    }

    public AvaloniaList<ToDoItemNotify> CompletedItems { get; } = new();
    public AvaloniaList<TypeOfPeriodicity> TypeOfPeriodicities { get; }
    public AvaloniaList<ToDoItemNotify> Items { get; } = new();
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand AddToDoItemCommand { get; }
    public ICommand ChangeToDoItemByPathCommand { get; }
    public ICommand ToRootItemCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Inject]
    public required PathControl Path { get; set; }

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

    public string Description
    {
        get => description;
        set => this.RaiseAndSetIfChanged(ref description, value);
    }

    private async void OnNextId(Guid x)
    {
        await SafeExecuteAsync(RefreshToDoItemAsync);
    }

    private async void OnNextTypeOfPeriodicity(TypeOfPeriodicity x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateTypeOfPeriodicityAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }

    private async void OnNextName(string x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateNameToDoItemAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
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
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateCompleteStatusAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
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
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateDueDateAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }

    private async void OnNextDescription(string x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateDescriptionToDoItemAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }

    public async Task RefreshToDoItemAsync()
    {
        Path.Items ??= new AvaloniaList<object>();
        var item = await ToDoService.GetToDoItemAsync(Id);
        DueDate = item.DueDate;
        Name = item.Name;
        IsComplete = item.IsComplete;
        TypeOfPeriodicity = item.TypeOfPeriodicity;
        Description = item.Description;
        Items.Clear();
        CompletedItems.Clear();
        var source = item.Items.Select(x => Mapper.Map<ToDoItemNotify>(x)).ToArray();
        Items.AddRange(source.Where(x => x.Status != ToDoItemStatus.Complete).OrderBy(x => x.OrderIndex));
        CompletedItems.AddRange(source.Where(x => x.Status == ToDoItemStatus.Complete).OrderBy(x => x.OrderIndex));
        SubscribeItems(Items);
        SubscribeItems(CompletedItems);
        Path.Items.Clear();
        Path.Items.Add(new RootItem());
        Path.Items.AddRange(item.Parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));
    }

    private void SubscribeItems(IEnumerable<ToDoItemNotify> items)
    {
        foreach (var itemNotify in items)
        {
            async void OnNextIsComplete(bool x)
            {
                await SafeExecuteAsync(
                    async () =>
                    {
                        await ToDoService.UpdateCompleteStatusAsync(itemNotify.Id, x);
                        await RefreshToDoItemAsync();
                    }
                );
            }

            itemNotify.WhenAnyValue(x => x.IsComplete).Skip(1).Subscribe(OnNextIsComplete);
        }
    }
}