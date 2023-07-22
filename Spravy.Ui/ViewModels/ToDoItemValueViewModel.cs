using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using ExtensionFramework.Core.Common.Extensions;
using ReactiveUI;
using Spravy.Domain.Enums;
using Spravy.Domain.Models;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class ToDoItemValueViewModel : ToDoItemViewModel
{
    private TypeOfPeriodicity typeOfPeriodicity;
    private DateTimeOffset? dueDate;
    private bool isComplete;

    public ToDoItemValueViewModel() : base("to-do-item-value")
    {
        CompleteToDoItemCommand = CreateCommandFromTask(CompleteToDoItemAsync);
        SubscribeProperties();
    }


    public ICommand CompleteToDoItemCommand { get; }

    public bool IsComplete
    {
        get => isComplete;
        set => this.RaiseAndSetIfChanged(ref isComplete, value);
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

    private async Task CompleteToDoItemAsync()
    {
        await DialogViewer.ShowDialogAsync<CompleteToDoItemView>(
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                viewModel.IsDialog = true;
                viewModel.Item = Mapper.Map<ToDoSubItemValueNotify>(this);
            }
        );

        await RefreshToDoItemAsync();
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
            case ToDoItemValue toDoItemValue:
                Name = toDoItemValue.Name;
                Type = ToDoItemType.Value;
                IsComplete = toDoItemValue.IsComplete;
                TypeOfPeriodicity = toDoItemValue.TypeOfPeriodicity;
                DueDate = toDoItemValue.DueDate;
                Description = toDoItemValue.Description;
                Items.Clear();
                CompletedItems.Clear();
                var source = toDoItemValue.Items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)).ToArray();
                Items.AddRange(source.Where(x => x.Status != ToDoItemStatus.Complete).OrderBy(x => x.OrderIndex));
                CompletedItems.AddRange(
                    source.Where(x => x.Status == ToDoItemStatus.Complete).OrderBy(x => x.OrderIndex)
                );
                SubscribeItems(Items);
                SubscribeItems(CompletedItems);
                Path.Items.Clear();
                Path.Items.Add(new RootItem());
                Path.Items.AddRange(item.Parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(item));
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
        PropertySubscribes.AddRange(GetSubscribeProperties());
    }

    private IEnumerable<IDisposable> GetSubscribeProperties()
    {
        yield return this.WhenAnyValue(x => x.DueDate).Skip(1).Subscribe(OnNextDueDate);
        yield return this.WhenAnyValue(x => x.TypeOfPeriodicity).Skip(1).Subscribe(OnNextTypeOfPeriodicity);
        yield return this.WhenAnyValue(x => x.Id).Skip(1).Subscribe(OnNextId);
        yield return this.WhenAnyValue(x => x.IsComplete).Skip(1).Subscribe(OnNextIsComplete);
        yield return this.WhenAnyValue(x => x.Name).Skip(1).Subscribe(OnNextName);
        yield return this.WhenAnyValue(x => x.Description).Skip(1).Subscribe(OnNextDescription);
        yield return this.WhenAnyValue(x => x.Type).Skip(1).Subscribe(OnNextType);
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