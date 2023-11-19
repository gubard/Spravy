using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Spravy.Domain.Extensions;
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
                if (Item is null)
                {
                    return;
                }

                view.DefaultSelectedItemId = Item.Id;
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
            calendar =>
            {
                calendar.SelectedDate = DateTimeOffset.Now.ToCurrentDay().DateTime;
                calendar.SelectedTime = TimeSpan.Zero;
            }
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