using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemGroupViewModel : ToDoItemViewModel, IRefreshToDoItem
{
    public ToDoItemGroupViewModel() : base("to-do-item-group")
    {
        SubscribeProperties();
    }

    public override async Task RefreshToDoItemAsync()
    {
        UnsubscribeProperties();
        Path.Items ??= new();
        var item = await ToDoService.GetToDoItemAsync(Id);

        switch (item)
        {
            case ToDoItemGroup:
                Link = item.Link?.AbsoluteUri ?? string.Empty;
                Name = item.Name;
                Description = item.Description;
                Type = ToDoItemType.Group;
                IsFavorite = item.IsFavorite;
                var source = item.Items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)).ToArray();
                await ToDoSubItemsViewModel.UpdateItemsAsync(source, this);
                SubscribeItems(source);
                Path.Items.Clear();
                Path.Items.Add(new RootItem());
                Path.Items.AddRange(item.Parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));
                SubscribeProperties();

                break;
            case ToDoItemPeriodicity toDoItemPeriodicity:
                Navigator.NavigateTo<ToDoItemPeriodicityViewModel>(x => x.Id = toDoItemPeriodicity.Id);

                return;
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

    private void UnsubscribeProperties()
    {
        foreach (var propertySubscribe in PropertySubscribes)
        {
            propertySubscribe.Dispose();
        }

        PropertySubscribes.Clear();
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
        yield return this.WhenAnyValue(x => x.Link).Skip(1).Subscribe(OnNextLink);
    }
}