using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Material.Icons;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class ToDoItemPlannedViewModel : ToDoItemViewModel, IRefreshToDoItem
{
    private bool isCompleted;
    private DateTimeOffset dueDate;
    private ToDoItemChildrenType childrenType;

    public ToDoItemPlannedViewModel() : base("to-do-item-value")
    {
        ChangeDueDateCommand = CreateCommandFromTask(ChangeDueDateAsync);
        CompleteToDoItemCommand = CreateCommandFromTask(CompleteToDoItemAsync);
        SubscribeProperties();
        Commands.Add(new(MaterialIconKind.Check, CompleteToDoItemCommand));
    }

    public ICommand CompleteToDoItemCommand { get; }
    public ICommand ChangeDueDateCommand { get; }

    public ToDoItemChildrenType ChildrenType
    {
        get => childrenType;
        set => this.RaiseAndSetIfChanged(ref childrenType, value);
    }

    public bool IsCompleted
    {
        get => isCompleted;
        set => this.RaiseAndSetIfChanged(ref isCompleted, value);
    }

    public DateTimeOffset DueDate
    {
        get => dueDate;
        set => this.RaiseAndSetIfChanged(ref dueDate, value);
    }

    private Task ChangeDueDateAsync()
    {
        return DialogViewer.ShowDateConfirmDialogAsync(
            value =>
            {
                DueDate = value;

                return DialogViewer.CloseInputDialogAsync();
            },
            calendar => calendar.SelectedDate = DueDate.Date
        );
    }

    private Task CompleteToDoItemAsync()
    {
        return DialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemView>(
            _ => DialogViewer.CloseInputDialogAsync(),
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();

                if (IsCompleted)
                {
                    viewModel.SetIncompleteStatus();
                }
                else
                {
                    viewModel.SetCompleteStatus();
                }

                viewModel.Complete = async status =>
                {
                    switch (status)
                    {
                        case CompleteStatus.Complete:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(Id, true);
                            break;
                        case CompleteStatus.Skip:
                            await ToDoService.SkipToDoItemAsync(Id);
                            break;
                        case CompleteStatus.Fail:
                            await ToDoService.FailToDoItemAsync(Id);
                            break;
                        case CompleteStatus.Incomplete:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(Id, false);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(status), status, null);
                    }

                    await RefreshToDoItemAsync();
                    await DialogViewer.CloseInputDialogAsync();
                };
            }
        );
    }

    private async void OnNextDueDate(DateTimeOffset x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemDueDateAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }

    private async void OnNextIsComplete(bool x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemCompleteStatusAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }

    public override async Task RefreshToDoItemAsync()
    {
        UnsubscribeProperties();
        Path.Items ??= new();
        var item = await ToDoService.GetToDoItemAsync(Id);

        switch (item)
        {
            case ToDoItemGroup toDoItemGroup:
                Navigator.NavigateTo<ToDoItemGroupViewModel>(x => x.Id = toDoItemGroup.Id);

                return;
            case ToDoItemPeriodicity toDoItemPeriodicity:
                Navigator.NavigateTo<ToDoItemPeriodicityViewModel>(x => x.Id = toDoItemPeriodicity.Id);

                return;
            case ToDoItemPlanned toDoItemPlanned:
                IsFavorite = item.IsFavorite;
                Name = toDoItemPlanned.Name;
                Type = ToDoItemType.Planned;
                IsCompleted = toDoItemPlanned.IsCompleted;
                Description = toDoItemPlanned.Description;
                DueDate = toDoItemPlanned.DueDate;
                ChildrenType = toDoItemPlanned.ChildrenType;
                var source = toDoItemPlanned.Items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)).ToArray();
                await ToDoSubItemsViewModel.UpdateItemsAsync(source, this);
                SubscribeItems(source);
                Path.Items.Clear();
                Path.Items.Add(new RootItem());
                Path.Items.AddRange(item.Parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));

                break;
            case ToDoItemValue toDoItemValue:
                Navigator.NavigateTo<ToDoItemValueViewModel>(x => x.Id = toDoItemValue.Id);

                return;
            case ToDoItemPeriodicityOffset doItemPeriodicityOffset:
                Navigator.NavigateTo<ToDoItemPeriodicityOffsetViewModel>(x => x.Id = doItemPeriodicityOffset.Id);

                return;
            default: throw new ArgumentOutOfRangeException(nameof(item));
        }

        SubscribeProperties();
    }

    private void SubscribeItems(IEnumerable<ToDoSubItemNotify> items)
    {
        foreach (var itemNotify in items.OfType<ToDoSubItemValueNotify>())
        {
            async void OnNextIsCompleteItem(bool x)
            {
                await SafeExecuteAsync(
                    async () =>
                    {
                        await ToDoService.UpdateToDoItemCompleteStatusAsync(itemNotify.Id, x);
                        await RefreshToDoItemAsync();
                    }
                );
            }

            itemNotify.WhenAnyValue(x => x.IsCompleted).Skip(1).Subscribe(OnNextIsCompleteItem);
        }
    }

    private void SubscribeProperties()
    {
        PropertySubscribes.AddRange(GetSubscribeProperties());
    }

    private IEnumerable<IDisposable> GetSubscribeProperties()
    {
        yield return this.WhenAnyValue(x => x.Id).Skip(1).Subscribe(OnNextId);
        yield return this.WhenAnyValue(x => x.IsCompleted).Skip(1).Subscribe(OnNextIsComplete);
        yield return this.WhenAnyValue(x => x.Name).Skip(1).Subscribe(OnNextName);
        yield return this.WhenAnyValue(x => x.Description).Skip(1).Subscribe(OnNextDescription);
        yield return this.WhenAnyValue(x => x.Type).Skip(1).Subscribe(OnNextType);
        yield return this.WhenAnyValue(x => x.DueDate).Skip(1).Subscribe(OnNextDueDate);
        yield return this.WhenAnyValue(x => x.ChildrenType).Skip(1).Subscribe(OnNextChildrenType);
    }

    private async void OnNextChildrenType(ToDoItemChildrenType x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemChildrenTypeAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }

    private void UnsubscribeProperties()
    {
        foreach (var propertySubscribe in PropertySubscribes)
        {
            propertySubscribe.Dispose();
        }

        PropertySubscribes.Clear();
    }
}