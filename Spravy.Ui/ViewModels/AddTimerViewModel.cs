using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Controls;
using Google.Protobuf;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.EventBus.Protos;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddTimerViewModel : RoutableViewModelBase
{
    private DateTimeOffset dueDateTime = DateTimeOffset.Now.ToCurrentDay();
    private bool isFavorite;
    private ToDoItemNotify? item;

    public AddTimerViewModel() : base("add-timer")
    {
        AddTimerCommand = CreateCommandFromTaskWithDialogProgressIndicator(AddTimerAsync);
        SwitchPaneCommand = CreateCommand(SwitchPane);
        ChangeDueDateTimeCommand = CreateCommandFromTask(ChangeDueDateTimeAsync);
        ChangeItemCommand = CreateCommandFromTask(ChangeItemAsync);
    }

    public Guid EventId { get; set; }
    public ICommand AddTimerCommand { get; }
    public ICommand SwitchPaneCommand { get; }
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

                return  DialogViewer.CloseInputDialogAsync();
            },
            view =>
            {
                if (Item is null)
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

                return  DialogViewer.CloseInputDialogAsync();
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

    private void SwitchPane()
    {
        SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
    }

    private async Task AddTimerAsync()
    {
        var eventValue = new ChangeToDoItemIsFavoriteEvent
        {
            IsFavorite = IsFavorite,
            ToDoItemId = Mapper.Map<ByteString>(Item.ThrowIfNull().Id),
        };

        await using var stream = new MemoryStream();
        eventValue.WriteTo(stream);
        stream.Position = 0;
        var parameters = new AddTimerParameters(DueDateTime, EventId, await stream.ToByteArrayAsync());
        await ScheduleService.AddTimerAsync(parameters);
    }
}