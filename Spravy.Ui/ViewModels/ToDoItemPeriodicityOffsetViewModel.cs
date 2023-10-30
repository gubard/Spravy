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

public class ToDoItemPeriodicityOffsetViewModel : ToDoItemViewModel, IRefreshToDoItem
{
    private DateTimeOffset dueDate;
    private ushort daysOffset;
    private ushort monthsOffset;
    private ushort yearsOffset;
    private ushort weeksOffset;
    private ToDoItemChildrenType childrenType;

    public ToDoItemPeriodicityOffsetViewModel() : base("to-do-item-periodicity-offset")
    {
        CompleteToDoItemCommand = CreateCommandFromTask(CompleteToDoItemAsync);
        ChangeDueDateCommand = CreateCommandFromTask(ChangeDueDate);
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

    public DateTimeOffset DueDate
    {
        get => dueDate;
        set => this.RaiseAndSetIfChanged(ref dueDate, value);
    }

    public ushort DaysOffset
    {
        get => daysOffset;
        set => this.RaiseAndSetIfChanged(ref daysOffset, value);
    }

    public ushort MonthsOffset
    {
        get => monthsOffset;
        set => this.RaiseAndSetIfChanged(ref monthsOffset, value);
    }

    public ushort WeeksOffset
    {
        get => weeksOffset;
        set => this.RaiseAndSetIfChanged(ref weeksOffset, value);
    }

    public ushort YearsOffset
    {
        get => yearsOffset;
        set => this.RaiseAndSetIfChanged(ref yearsOffset, value);
    }

    private Task ChangeDueDate()
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
                Navigator.NavigateTo<ToDoItemPlannedViewModel>(x => x.Id = toDoItemPlanned.Id);
                return;
            case ToDoItemValue toDoItemValue:
                Navigator.NavigateTo<ToDoItemValueViewModel>(x => x.Id = toDoItemValue.Id);

                return;

            case ToDoItemPeriodicityOffset toDoItemPeriodicityOffset:
                IsFavorite = toDoItemPeriodicityOffset.IsFavorite;
                Name = toDoItemPeriodicityOffset.Name;
                Type = ToDoItemType.PeriodicityOffset;
                Description = toDoItemPeriodicityOffset.Description;
                DueDate = toDoItemPeriodicityOffset.DueDate;
                DaysOffset = toDoItemPeriodicityOffset.DaysOffset;
                WeeksOffset = toDoItemPeriodicityOffset.WeeksOffset;
                MonthsOffset = toDoItemPeriodicityOffset.MonthsOffset;
                YearsOffset = toDoItemPeriodicityOffset.YearsOffset;
                ChildrenType = toDoItemPeriodicityOffset.ChildrenType;
                var source = toDoItemPeriodicityOffset.Items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)).ToArray();
                await ToDoSubItemsViewModel.UpdateItemsAsync(source, this);
                SubscribeItems(source);
                Path.Items.Clear();
                Path.Items.Add(new RootItem());
                Path.Items.AddRange(item.Parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));

                break;
            default: throw new ArgumentOutOfRangeException(nameof(item));
        }

        SubscribeProperties();
    }

    private void SubscribeProperties()
    {
        PropertySubscribes.AddRange(GetSubscribeProperties());
    }

    private IEnumerable<IDisposable> GetSubscribeProperties()
    {
        yield return this.WhenAnyValue(x => x.Id).Skip(1).Subscribe(OnNextId);
        yield return this.WhenAnyValue(x => x.Name).Skip(1).Subscribe(OnNextName);
        yield return this.WhenAnyValue(x => x.Description).Skip(1).Subscribe(OnNextDescription);
        yield return this.WhenAnyValue(x => x.Type).Skip(1).Subscribe(OnNextType);
        yield return this.WhenAnyValue(x => x.DueDate).Skip(1).Subscribe(OnNextDueDate);
        yield return this.WhenAnyValue(x => x.DueDate).Skip(1).Subscribe(OnNextDueDate);
        yield return this.WhenAnyValue(x => x.DaysOffset).Skip(1).Subscribe(OnNextDaysOffset);
        yield return this.WhenAnyValue(x => x.WeeksOffset).Skip(1).Subscribe(OnNextWeeksOffset);
        yield return this.WhenAnyValue(x => x.MonthsOffset).Skip(1).Subscribe(OnNextMonthsOffset);
        yield return this.WhenAnyValue(x => x.YearsOffset).Skip(1).Subscribe(OnNextYearsOffset);
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

    private async void OnNextYearsOffset(ushort x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemYearsOffsetAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }

    private async void OnNextMonthsOffset(ushort x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemMonthsOffsetAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }

    private async void OnNextWeeksOffset(ushort x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemWeeksOffsetAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }

    private async void OnNextDaysOffset(ushort x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemDaysOffsetAsync(Id, x);
                await RefreshToDoItemAsync();
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

    private void SubscribeItems(IEnumerable<ToDoSubItemNotify> items)
    {
        foreach (var itemNotify in items.OfType<ToDoSubItemValueNotify>())
        {
            async void OnNextIsComplete(bool x)
            {
                await SafeExecuteAsync(
                    async () =>
                    {
                        await ToDoService.UpdateToDoItemCompleteStatusAsync(itemNotify.Id, x);
                        await RefreshToDoItemAsync();
                    }
                );
            }

            itemNotify.WhenAnyValue(x => x.IsCompleted).Skip(1).Subscribe(OnNextIsComplete);
        }
    }

    private Task CompleteToDoItemAsync()
    {
        return DialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemView>(
            _ => DialogViewer.CloseInputDialogAsync(),
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                viewModel.SetCompleteStatus();

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
                        default:
                            throw new ArgumentOutOfRangeException(nameof(status), status, null);
                    }

                    await RefreshToDoItemAsync();
                    await DialogViewer.CloseInputDialogAsync();
                };
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