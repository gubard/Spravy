using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Controls;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddTimerViewModel : ViewModelBase
{
    private DateTimeOffset dueDateTime = DateTimeOffset.Now.ToCurrentDay();
    private bool isFavorite;
    private ToDoItemNotify? item;

    public AddTimerViewModel()
    {
        ChangeDueDateTimeCommand = CreateCommandFromTask(ChangeDueDateTimeAsync);
        ChangeItemCommand = CreateCommandFromTask(ChangeItemAsync);
    }

    public Guid EventId { get; set; }
    public ICommand ChangeDueDateTimeCommand { get; }
    public ICommand ChangeItemCommand { get; }

    [Inject]
    public required IScheduleService ScheduleService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required SplitView SplitView { get; init; }

    private Task ChangeItemAsync()
    {
        return DialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
            itemNotify =>
            {
                Item = new()
                {
                    Id = itemNotify.Id,
                    Name = itemNotify.Name
                };

                return DialogViewer.CloseInputDialogAsync();
            },
            view =>
            {
                if(Item is null)
                {
                    return;
                }

                view.ViewModel.ThrowIfNull().DefaultSelectedItemId = Item.Id;
            }
        );
    }

    private Task ChangeDueDateTimeAsync()
    {
        return DialogViewer.ShowDateTimeConfirmDialogAsync(
            value =>
            {
                DueDateTime = value;

                return DialogViewer.CloseInputDialogAsync();
            },
            calendar => calendar.SelectedDate = DateTimeOffset.Now.ToCurrentDay().DateTime,
            clock => clock.SelectedTime = TimeSpan.Zero
        );
    }

    public ToDoItemNotify? Item
    {
        get => item;
        set => this.RaiseAndSetIfChanged(ref item, value);
    }

    public DateTimeOffset DueDateTime
    {
        get => dueDateTime;
        set => this.RaiseAndSetIfChanged(ref dueDateTime, value);
    }

    public bool IsFavorite
    {
        get => isFavorite;
        set => this.RaiseAndSetIfChanged(ref isFavorite, value);
    }
}