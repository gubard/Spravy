using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using Material.Icons;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class ToDoItemStepViewModel : ToDoItemViewModel, IRefreshToDoItem
{
    private bool isCompleted;
    private ToDoItemChildrenType childrenType;

    public ToDoItemStepViewModel() : base("to-do-item-step")
    {
        CompleteToDoItemCommand = CreateCommandFromTask(CompleteToDoItemAsync);
        SubscribeProperties();
        Commands.Add(new(MaterialIconKind.Check, CompleteToDoItemCommand, "Complete"));
    }

    public ICommand CompleteToDoItemCommand { get; }

    public ToDoItemChildrenType ChildrenType
    {
        get => childrenType;
        set => this.RaiseAndSetIfChanged(ref childrenType, value);
    }

    public bool IsCompleted
    {
        get => isCompleted;
        set => this.RaiseAndSetIfChanged(ref isCompleted, value);
    }

    private Task CompleteToDoItemAsync()
    {
        return DialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemView>(
            _ => DialogViewer.CloseInputDialogAsync(),
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();

                if (IsCompleted)
                {
                    viewModel.SetIncompleteStatus();
                }
                else
                {
                    viewModel.SetCompleteStatus();
                }

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
                        case CompleteStatus.Incomplete:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(Id, false).ConfigureAwait(false);
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

    private async void OnNextIsComplete(bool x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemCompleteStatusAsync(Id, x).ConfigureAwait(false);
                await RefreshToDoItemAsync();
            }
        );
    }

    public override async Task RefreshToDoItemAsync()
    {
        UnsubscribeProperties();
        var item = await ToDoService.GetToDoItemAsync(Id).ConfigureAwait(false);

        switch (item)
        {
            case ToDoItemCircle:
                await Navigator.NavigateToAsync<ToDoItemCircleViewModel>(x => x.Id = item.Id);

                break;
            case ToDoItemStep step:
                Link = item.Link?.AbsoluteUri ?? string.Empty;
                IsFavorite = item.IsFavorite;
                Name = step.Name;
                Type = ToDoItemType.Step;
                IsCompleted = step.IsCompleted;
                Description = step.Description;
                ChildrenType = step.ChildrenType;
                var source = step.Items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)).ToArray();
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

                return;
            case ToDoItemGroup:
                await Navigator.NavigateToAsync<ToDoItemGroupViewModel>(x => x.Id = item.Id);

                return;
            case ToDoItemPeriodicity:
                await Navigator.NavigateToAsync<ToDoItemPeriodicityViewModel>(x => x.Id = item.Id);

                return;
            case ToDoItemPlanned:
                await Navigator.NavigateToAsync<ToDoItemPlannedViewModel>(x => x.Id = item.Id);

                return;
            case ToDoItemValue:
                await Navigator.NavigateToAsync<ToDoItemValueViewModel>(x => x.Id = item.Id);

                return;
            case ToDoItemPeriodicityOffset:
                await Navigator.NavigateToAsync<ToDoItemPeriodicityOffsetViewModel>(x => x.Id = item.Id);

                return;
            default: throw new ArgumentOutOfRangeException(nameof(item));
        }
    }

    private void SubscribeItems(IEnumerable<ToDoSubItemNotify> items)
    {
        foreach (var itemNotify in items.OfType<ToDoSubItemValueNotify>())
        {
            async void OnNextIsCompleteItem(bool x)
            {
                await SafeExecuteAsync(
                    async () =>
                    {
                        await ToDoService.UpdateToDoItemCompleteStatusAsync(itemNotify.Id, x).ConfigureAwait(false);
                        await RefreshToDoItemAsync();
                    }
                );
            }

            itemNotify.WhenAnyValue(x => x.IsCompleted).Skip(1).Subscribe(OnNextIsCompleteItem);
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

    private void UnsubscribeProperties()
    {
        foreach (var propertySubscribe in PropertySubscribes)
        {
            propertySubscribe.Dispose();
        }

        PropertySubscribes.Clear();
    }
}