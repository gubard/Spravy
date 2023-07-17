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
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.Core.Ui.Interfaces;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Core.Enums;
using Spravy.Core.Interfaces;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class ToDoItemViewModel : RoutableViewModelBase, IItemsViewModel<ToDoItemNotify>, IToDoItemOrderChanger
{
    private string name = string.Empty;
    private Guid id;
    private TypeOfPeriodicity typeOfPeriodicity;
    private DateTimeOffset? dueDate;
    private bool isComplete;
    private string description = string.Empty;
    private List<IDisposable> propertySubscribes = new();

    public ToDoItemViewModel() : base("to-do-item")
    {
        DeleteSubToDoItemCommand = CreateCommandFromTask<ToDoItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommand<ToDoItemNotify>(ChangeToDoItem);
        AddToDoItemCommand = CreateCommandFromTask(AddToDoItemAsync);
        ChangeToDoItemByPathCommand = CreateCommand<ToDoItemParentNotify>(ChangeToDoItemByPath);
        ToRootItemCommand = CreateCommand(ToRootItem);
        SkipSubToDoItemCommand = CreateCommandFromTask<ToDoItemNotify>(SkipSubToDoItem);
        TypeOfPeriodicities = new(Enum.GetValues<TypeOfPeriodicity>());
        SubscribeProperties();
    }

    public AvaloniaList<ToDoItemNotify> CompletedItems { get; } = new();
    public AvaloniaList<TypeOfPeriodicity> TypeOfPeriodicities { get; }
    public AvaloniaList<ToDoItemNotify> Items { get; } = new();
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand AddToDoItemCommand { get; }
    public ICommand ChangeToDoItemByPathCommand { get; }
    public ICommand ToRootItemCommand { get; }
    public ICommand SkipSubToDoItemCommand { get; }

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

    private async Task AddToDoItemAsync()
    {
        await DialogViewer.ShowDialogAsync<AddToDoItemView>(
            view => view.ViewModel.ThrowIfNull().Parent = Mapper.Map<ToDoItemNotify>(this)
        );
    }

    private async Task SkipSubToDoItem(ToDoItemNotify item)
    {
        await ToDoService.SkipToDoItemAsync(item.Id);
        await RefreshToDoItemAsync();
    }

    private void ChangeToDoItemByPath(ToDoItemParentNotify item)
    {
        Navigator.NavigateTo<ToDoItemViewModel>(vm => vm.Id = item.Id);
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
        Navigator.NavigateTo<ToDoItemViewModel>(vm => vm.Id = item.Id);
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
        UnsubscribeProperties();
        Path.Items ??= new AvaloniaList<object>();
        var item = await ToDoService.GetToDoItemAsync(Id);
        Name = item.Name;
        IsComplete = item.IsComplete;
        TypeOfPeriodicity = item.TypeOfPeriodicity;
        DueDate = item.DueDate;
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
        SubscribeProperties();
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

    private void SubscribeProperties()
    {
        propertySubscribes.AddRange(GetSubscribeProperties());
    }

    private IEnumerable<IDisposable> GetSubscribeProperties()
    {
        yield return this.WhenAnyValue(x => x.DueDate).Skip(1).Subscribe(OnNextDueDate);
        yield return this.WhenAnyValue(x => x.TypeOfPeriodicity).Skip(1).Subscribe(OnNextTypeOfPeriodicity);
        yield return this.WhenAnyValue(x => x.Id).Skip(1).Subscribe(OnNextId);
        yield return this.WhenAnyValue(x => x.IsComplete).Skip(1).Subscribe(OnNextIsComplete);
        yield return this.WhenAnyValue(x => x.Name).Skip(1).Subscribe(OnNextName);
        yield return this.WhenAnyValue(x => x.Description).Skip(1).Subscribe(OnNextDescription);
    }

    private void UnsubscribeProperties()
    {
        foreach (var propertySubscribe in propertySubscribes)
        {
            propertySubscribe.Dispose();
        }

        propertySubscribes.Clear();
    }
}