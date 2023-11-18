using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using Material.Icons;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
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
    private DateOnly dueDate;
    private TypeOfPeriodicity typeOfPeriodicity;
    private object? periodicity;
    private List<IDisposable> periodicitySubs = new();

    public ToDoItemPeriodicityViewModel() : base("to-do-item-periodicity")
    {
        ChangeDueDateCommand = CreateCommandFromTask(ChangeDueDateAsync);
        CompleteToDoItemCommand = CreateCommandFromTask(CompleteToDoItemAsync);
        SubscribeProperties();
        Commands.Add(new(MaterialIconKind.Check, CompleteToDoItemCommand, "Complete"));
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

    public object? Periodicity
    {
        get => periodicity;
        set => this.RaiseAndSetIfChanged(ref periodicity, value);
    }

    public TypeOfPeriodicity TypeOfPeriodicity
    {
        get => typeOfPeriodicity;
        set => this.RaiseAndSetIfChanged(ref typeOfPeriodicity, value);
    }

    public DateOnly DueDate
    {
        get => dueDate;
        set => this.RaiseAndSetIfChanged(ref dueDate, value);
    }

    private Task ChangeDueDateAsync()
    {
        return DialogViewer.ShowDateConfirmDialogAsync(
            value =>
            {
                DueDate = value.ToDateOnly();

                return DialogViewer.CloseInputDialogAsync();
            },
            calendar => calendar.SelectedDate = DueDate.ToDateTime()
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
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(Id, true).ConfigureAwait(false);
                            break;
                        case CompleteStatus.Skip:
                            await ToDoService.SkipToDoItemAsync(Id).ConfigureAwait(false);
                            break;
                        case CompleteStatus.Fail:
                            await ToDoService.FailToDoItemAsync(Id).ConfigureAwait(false);
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
        var item = await ToDoService.GetToDoItemAsync(Id).ConfigureAwait(false);

        switch (item)
        {
            case ToDoItemGroup:
                await Navigator.NavigateToAsync<ToDoItemGroupViewModel>(x => x.Id = item.Id);

                return;
            case ToDoItemPeriodicity toDoItemPeriodicity:
                Link = item.Link?.AbsoluteUri ?? string.Empty;
                IsFavorite = item.IsFavorite;
                Name = item.Name;
                Type = ToDoItemType.Periodicity;
                Description = item.Description;
                DueDate = toDoItemPeriodicity.DueDate;
                ChildrenType = toDoItemPeriodicity.ChildrenType;
                SetTypeOfPeriodicity(toDoItemPeriodicity.Periodicity);
                var source = item.Items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)).ToArray();
                await ToDoSubItemsViewModel.UpdateItemsAsync(source, this);
                SubscribeItems(source);

                await Dispatcher.UIThread.InvokeAsync(
                    () =>
                    {
                        PathViewModel.Items.Clear();
                        PathViewModel.Items.Add(new RootItem());
                        PathViewModel.Items.AddRange(item.Parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));
                    }
                );

                SubscribeProperties();

                break;
            case ToDoItemPlanned:
                await Navigator.NavigateToAsync<ToDoItemPlannedViewModel>(x => x.Id = item.Id);
                return;
            case ToDoItemValue:
                await Navigator.NavigateToAsync<ToDoItemValueViewModel>(x => x.Id = item.Id);

                return;
            case ToDoItemPeriodicityOffset:
                await Navigator.NavigateToAsync<ToDoItemPeriodicityOffsetViewModel>(x => x.Id = item.Id);

                return;
            case ToDoItemCircle:
                await Navigator.NavigateToAsync<ToDoItemCircleViewModel>(x => x.Id = item.Id);

                return;
            case ToDoItemStep:
                await Navigator.NavigateToAsync<ToDoItemStepViewModel>(x => x.Id = item.Id);

                return;
            default: throw new ArgumentOutOfRangeException(nameof(item));
        }
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
                        await ToDoService.UpdateToDoItemCompleteStatusAsync(itemNotify.Id, x).ConfigureAwait(false);
                        await RefreshToDoItemAsync();
                    }
                );
            }

            itemNotify.WhenAnyValue(x => x.IsCompleted).Skip(1).Subscribe(OnNextIsComplete);
        }
    }


    private async void OnNextDueDate(DateOnly x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemDueDateAsync(Id, x).ConfigureAwait(false);
                await RefreshToDoItemAsync();
            }
        );
    }

    private async void OnNextTypeOfPeriodicity(TypeOfPeriodicity x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemTypeOfPeriodicityAsync(Id, x).ConfigureAwait(false);
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
        yield return this.WhenAnyValue(x => x.Link).Skip(1).Subscribe(OnNextLink);
    }

    private async void OnNextChildrenType(ToDoItemChildrenType x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemChildrenTypeAsync(Id, x).ConfigureAwait(false);
                await RefreshToDoItemAsync();
            }
        );
    }

    private void SetTypeOfPeriodicity(IPeriodicity per)
    {
        foreach (var periodicitySub in periodicitySubs)
        {
            periodicitySub.Dispose();
        }

        periodicitySubs.Clear();

        switch (per)
        {
            case AnnuallyPeriodicity annuallyPeriodicity:
            {
                TypeOfPeriodicity = TypeOfPeriodicity.Annually;
                var periodicityViewModel = Resolver.Get<DayOfYearSelectorViewModel>();

                foreach (var day in annuallyPeriodicity.Days)
                {
                    var item = periodicityViewModel.Items.Single(x => x.Month == day.Month);
                    var d = item.Days.Items.Single(x => x.Day == day.Day);
                    d.IsSelected = true;
                }

                foreach (var item in periodicityViewModel.Items)
                {
                    foreach (var i in item.Days.Items)
                    {
                        periodicitySubs.Add(
                            i.WhenAnyValue(x => x.IsSelected).Skip(1).Subscribe(OnNextSelectedDayOfYear)
                        );
                    }
                }

                Periodicity = periodicityViewModel;

                break;
            }
            case DailyPeriodicity:
            {
                TypeOfPeriodicity = TypeOfPeriodicity.Daily;
                var periodicityViewModel = Resolver.Get<DailyPeriodicityViewModel>();
                Periodicity = periodicityViewModel;

                break;
            }
            case MonthlyPeriodicity monthlyPeriodicity:
            {
                TypeOfPeriodicity = TypeOfPeriodicity.Monthly;
                var periodicityViewModel = Resolver.Get<DayOfMonthSelectorViewModel>();

                foreach (var selectItem in periodicityViewModel.Items)
                {
                    if (monthlyPeriodicity.Days.Contains(selectItem.Day))
                    {
                        selectItem.IsSelected = true;
                    }

                    periodicitySubs.Add(
                        selectItem.WhenAnyValue(x => x.IsSelected).Skip(1).Subscribe(OnNextSelectedDaysOfMonth)
                    );
                }

                Periodicity = periodicityViewModel;

                break;
            }
            case WeeklyPeriodicity weeklyPeriodicity:
            {
                TypeOfPeriodicity = TypeOfPeriodicity.Weekly;
                var periodicityViewModel = Resolver.Get<DayOfWeekSelectorViewModel>();

                foreach (var selectItem in periodicityViewModel.Items)
                {
                    if (weeklyPeriodicity.Days.Contains(selectItem.DayOfWeek))
                    {
                        selectItem.IsSelected = true;
                    }

                    periodicitySubs.Add(
                        selectItem.WhenAnyValue(x => x.IsSelected).Skip(1).Subscribe(OnNextSelectedDaysOfWeek)
                    );
                }

                Periodicity = periodicityViewModel;

                break;
            }
            default: throw new ArgumentOutOfRangeException(nameof(per));
        }
    }

    private async void OnNextSelectedDaysOfWeek(bool value)
    {
        var viewModel = Periodicity.ThrowIfNull().As<DayOfWeekSelectorViewModel>().ThrowIfNull();
        var weeklyPeriodicity =
            new WeeklyPeriodicity(viewModel.Items.Where(x => x.IsSelected).Select(x => x.DayOfWeek));

        await SafeExecuteAsync(
            () => ToDoService.UpdateToDoItemWeeklyPeriodicityAsync(Id, weeklyPeriodicity).ConfigureAwait(false)
        );
    }

    private async void OnNextSelectedDaysOfMonth(bool value)
    {
        var viewModel = Periodicity.ThrowIfNull().As<DayOfMonthSelectorViewModel>().ThrowIfNull();

        await SafeExecuteAsync(
            () => ToDoService.UpdateToDoItemMonthlyPeriodicityAsync(
                    Id,
                    new(viewModel.Items.Where(x => x.IsSelected).Select(x => x.Day))
                )
                .ConfigureAwait(false)
        );
    }

    private async void OnNextSelectedDayOfYear(bool value)
    {
        var viewModel = Periodicity.ThrowIfNull().As<DayOfYearSelectorViewModel>().ThrowIfNull();

        var annuallyPeriodicity = new AnnuallyPeriodicity(
            viewModel.Items.SelectMany(x => x.Days.Items.Select(y => new DayOfYear(x.Month, y.Day)))
        );

        await SafeExecuteAsync(
            () => ToDoService.UpdateToDoItemAnnuallyPeriodicityAsync(Id, annuallyPeriodicity).ConfigureAwait(false)
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