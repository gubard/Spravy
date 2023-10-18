using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Material.Icons;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class ToDoItemPeriodicityViewModel : ToDoItemViewModel, IRefreshToDoItem
{
    private ToDoItemChildrenType childrenType;
    private DateTimeOffset dueDate;
    private TypeOfPeriodicity typeOfPeriodicity;
    private PeriodicityViewModel? periodicity;
    private IDisposable? periodicitySub;

    public ToDoItemPeriodicityViewModel() : base("to-do-item-periodicity")
    {
        ChangeDueDateCommand = CreateCommandFromTask(ChangeDueDate);
        CompleteToDoItemCommand = CreateCommandFromTask(CompleteToDoItemAsync);
        SubscribeProperties();
        Commands.Add(new(MaterialIconKind.Check, CompleteToDoItemCommand));
    }

    [Inject]
    public required IKernel Resolver { get; init; }

    public ICommand CompleteToDoItemCommand { get; }
    public ICommand ChangeDueDateCommand { get; }

    public ToDoItemChildrenType ChildrenType
    {
        get => childrenType;
        set => this.RaiseAndSetIfChanged(ref childrenType, value);
    }

    public PeriodicityViewModel? Periodicity
    {
        get => periodicity;
        set => this.RaiseAndSetIfChanged(ref periodicity, value);
    }

    public TypeOfPeriodicity TypeOfPeriodicity
    {
        get => typeOfPeriodicity;
        set => this.RaiseAndSetIfChanged(ref typeOfPeriodicity, value);
    }

    public DateTimeOffset DueDate
    {
        get => dueDate;
        set => this.RaiseAndSetIfChanged(ref dueDate, value);
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

    public override async Task RefreshToDoItemAsync()
    {
        UnsubscribeProperties();
        Path.Items ??= new AvaloniaList<object>();
        var item = await ToDoService.GetToDoItemAsync(Id);

        switch (item)
        {
            case ToDoItemGroup toDoItemGroup:
                Navigator.NavigateTo<ToDoItemGroupViewModel>(x => x.Id = toDoItemGroup.Id);

                return;
            case ToDoItemPeriodicity toDoItemPeriodicity:
                IsPinned = item.IsPinned;
                Name = item.Name;
                Type = ToDoItemType.Periodicity;
                Description = item.Description;
                DueDate = toDoItemPeriodicity.DueDate;
                ChildrenType = toDoItemPeriodicity.ChildrenType;
                SetTypeOfPeriodicity(toDoItemPeriodicity.Periodicity);
                var source = item.Items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)).ToArray();
                await ToDoSubItemsViewModel.UpdateItemsAsync(source, this);
                SubscribeItems(source);
                Path.Items.Clear();
                Path.Items.Add(new RootItem());
                Path.Items.AddRange(item.Parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));

                break;
            case ToDoItemPlanned toDoItemPlanned:
                Navigator.NavigateTo<ToDoItemPlannedViewModel>(x => x.Id = toDoItemPlanned.Id);
                return;
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

    private async void OnNextTypeOfPeriodicity(TypeOfPeriodicity x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemTypeOfPeriodicityAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
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
        yield return this.WhenAnyValue(x => x.TypeOfPeriodicity).Skip(1).Subscribe(OnNextTypeOfPeriodicity);
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

    private void SetTypeOfPeriodicity(IPeriodicity per)
    {
        switch (per)
        {
            case AnnuallyPeriodicity annuallyPeriodicity:
            {
                TypeOfPeriodicity = TypeOfPeriodicity.Annually;
                var periodicityViewModel = Resolver.Get<AnnuallyPeriodicityViewModel>();
                periodicityViewModel.DayOfYearSelector.SelectedDaysOfYear.AddRange(annuallyPeriodicity.Days);
                periodicitySub?.Dispose();
                Periodicity = periodicityViewModel;

                periodicitySub = Observable
                    .FromEventPattern<EventHandler<SelectedSelectedDaysOfYearEventArgs>,
                        SelectedSelectedDaysOfYearEventArgs>(
                        h => periodicityViewModel.DayOfYearSelector.SelectedDaysOfYearChanged += h,
                        h => periodicityViewModel.DayOfYearSelector.SelectedDaysOfYearChanged -= h
                    )
                    .Subscribe(OnNextSelectedDayOfYear);

                break;
            }
            case DailyPeriodicity:
            {
                TypeOfPeriodicity = TypeOfPeriodicity.Daily;
                var periodicityViewModel = Resolver.Get<DailyPeriodicityViewModel>();
                periodicitySub?.Dispose();
                Periodicity = periodicityViewModel;

                break;
            }
            case MonthlyPeriodicity monthlyPeriodicity:
            {
                TypeOfPeriodicity = TypeOfPeriodicity.Monthly;
                var periodicityViewModel = Resolver.Get<MonthlyPeriodicityViewModel>();
                periodicityViewModel.DayOfMonthSelector.SelectedDaysOfMonth.AddRange(monthlyPeriodicity.Days);
                periodicitySub?.Dispose();
                Periodicity = periodicityViewModel;

                periodicitySub = Observable
                    .FromEventPattern<EventHandler<SelectedDaysOfMonthChangedEventArgs>,
                        SelectedDaysOfMonthChangedEventArgs>(
                        h => periodicityViewModel.DayOfMonthSelector.SelectedDaysOfMonthChanged += h,
                        h => periodicityViewModel.DayOfMonthSelector.SelectedDaysOfMonthChanged -= h
                    )
                    .Subscribe(OnNextSelectedDaysOfMonth);

                break;
            }
            case WeeklyPeriodicity weeklyPeriodicity:
            {
                TypeOfPeriodicity = TypeOfPeriodicity.Weekly;
                var periodicityViewModel = Resolver.Get<WeeklyPeriodicityViewModel>();
                periodicityViewModel.DayOfWeekSelector.SelectedDaysOfWeek.AddRange(weeklyPeriodicity.Days);
                periodicitySub?.Dispose();
                Periodicity = periodicityViewModel;

                periodicitySub = Observable
                    .FromEventPattern<EventHandler<SelectedDaysOfWeekChangedEventArgs>,
                        SelectedDaysOfWeekChangedEventArgs>(
                        h => periodicityViewModel.DayOfWeekSelector.SelectedDaysOfWeekChanged += h,
                        h => periodicityViewModel.DayOfWeekSelector.SelectedDaysOfWeekChanged -= h
                    )
                    .Subscribe(OnNextSelectedDaysOfWeek);

                break;
            }
            default: throw new ArgumentOutOfRangeException(nameof(per));
        }
    }

    private async void OnNextSelectedDaysOfWeek(EventPattern<SelectedDaysOfWeekChangedEventArgs> x)
    {
        var weeklyPeriodicity = new WeeklyPeriodicity(x.EventArgs.NewSelectedDaysOfWeek);
        await SafeExecuteAsync(() => ToDoService.UpdateToDoItemWeeklyPeriodicityAsync(Id, weeklyPeriodicity));
    }

    private async void OnNextSelectedDaysOfMonth(EventPattern<SelectedDaysOfMonthChangedEventArgs> x)
    {
        await SafeExecuteAsync(
            () => ToDoService.UpdateToDoItemMonthlyPeriodicityAsync(Id, new(x.EventArgs.NewDaysOfMonth))
        );
    }

    private async void OnNextSelectedDayOfYear(EventPattern<SelectedSelectedDaysOfYearEventArgs> x)
    {
        var annuallyPeriodicity = new AnnuallyPeriodicity(x.EventArgs.NewSelectedDaysOfYear);
        await SafeExecuteAsync(() => ToDoService.UpdateToDoItemAnnuallyPeriodicityAsync(Id, annuallyPeriodicity));
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