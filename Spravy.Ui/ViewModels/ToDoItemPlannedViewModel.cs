using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using ExtensionFramework.Core.Common.Extensions;
using Material.Icons;
using ReactiveUI;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class ToDoItemPlannedViewModel : ToDoItemViewModel, IRefreshToDoItem
{
    private bool isCompleted;
    private DateTimeOffset dueDate;

    public ToDoItemPlannedViewModel() : base("to-do-item-value")
    {
        CompleteToDoItemCommand = CreateCommandFromTaskWithDialogProgressIndicator(CompleteToDoItemAsync);
        SubscribeProperties();
        Commands.Add(new(MaterialIconKind.Check, CompleteToDoItemCommand));
    }

    public ICommand CompleteToDoItemCommand { get; }

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

    private async Task CompleteToDoItemAsync()
    {
        await DialogViewer.ShowDialogAsync<CompleteToDoItemView>(
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                viewModel.IsDialog = true;
                viewModel.Item = Mapper.Map<ToDoSubItemNotify>(this);
            }
        );

        await RefreshToDoItemAsync();
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
        Path.Items ??= new AvaloniaList<object>();
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
                IsCurrent = item.IsCurrent;
                Name = toDoItemPlanned.Name;
                Type = ToDoItemType.Planned;
                IsCompleted = toDoItemPlanned.IsCompleted;
                Description = toDoItemPlanned.Description;
                DueDate = toDoItemPlanned.DueDate;
                var source = toDoItemPlanned.Items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)).ToArray();
                ToDoSubItemsView.ViewModel.ThrowIfNull().UpdateItems(source, this);
                SubscribeItems(source);
                Path.Items.Clear();
                Path.Items.Add(new RootItem());
                Path.Items.AddRange(item.Parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));

                break;
            case ToDoItemValue toDoItemValue:
                Navigator.NavigateTo<ToDoItemValueViewModel>(x => x.Id = toDoItemValue.Id);

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