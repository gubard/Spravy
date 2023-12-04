using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddTimerViewModel : ViewModelBase
{
    private readonly TaskWork changeItemWork;
    private readonly TaskWork changeDueDateTimeWork;
    private DateTimeOffset dueDateTime = DateTimeOffset.Now.ToCurrentDay();
    private bool isFavorite;
    private ToDoShortItemNotify? item;

    public AddTimerViewModel()
    {
        changeItemWork = new(ChangeItemAsync);
        changeDueDateTimeWork = new(ChangeDueDateTimeAsync);
        ChangeDueDateTimeCommand = CreateCommandFromTask(changeItemWork.RunAsync);
        ChangeItemCommand = CreateCommandFromTask(changeDueDateTimeWork.RunAsync);
    }

    public Guid EventId { get; set; }
    public ICommand ChangeDueDateTimeCommand { get; }
    public ICommand ChangeItemCommand { get; }

    private async Task ChangeItemAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
            async itemNotify =>
            {
                await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                await this.InvokeUIBackgroundAsync(
                    () => Item = new()
                    {
                        Id = itemNotify.Id,
                        Name = itemNotify.Name
                    }
                );
            },
            view =>
            {
                if (Item is null)
                {
                    return;
                }

                view.DefaultSelectedItemId = Item.Id;
            },
            cancellationToken
        ).ConfigureAwait(false);
    }

    private async Task ChangeDueDateTimeAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowDateTimeConfirmDialogAsync(
            async value =>
            {
                await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await this.InvokeUIBackgroundAsync(() => DueDateTime = value);
            },
            calendar =>
            {
                calendar.SelectedDate = DateTimeOffset.Now.ToCurrentDay().DateTime;
                calendar.SelectedTime = TimeSpan.Zero;
            },
            cancellationToken
        ).ConfigureAwait(false);
    }

    public ToDoShortItemNotify? Item
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