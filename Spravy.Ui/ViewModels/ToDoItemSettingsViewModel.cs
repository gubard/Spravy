using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemSettingsViewModel : NavigatableViewModelBase
{
    public ToDoItemSettingsViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public override string ViewId => TypeCache<ToDoItemSettingsViewModel>.Type.Name;
    public ICommand InitializedCommand { get; }

    [Inject]
    public required IKernel Resolver { get; init; }

    [Inject]
    public required ToDoItemContentViewModel ToDoItemContent { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Reactive]
    public IApplySettings? Settings { get; set; }

    [Reactive]
    public Guid ToDoItemId { get; set; }

    private Task InitializedAsync(CancellationToken cancellationToken)
    {
        ToDoItemContent.WhenAnyValue(x => x.Type)
            .Subscribe(
                x => Settings = x switch
                {
                    ToDoItemType.Value => Resolver.Get<ValueToDoItemSettingsViewModel>().Case(vm => vm.Id = ToDoItemId),
                    ToDoItemType.Planned => Resolver.Get<PlannedToDoItemSettingsViewModel>()
                        .Case(vm => vm.Id = ToDoItemId),
                    ToDoItemType.Periodicity => Resolver.Get<PeriodicityToDoItemSettingsViewModel>()
                        .Case(vm => vm.Id = ToDoItemId),
                    ToDoItemType.PeriodicityOffset => Resolver.Get<PeriodicityOffsetToDoItemSettingsViewModel>()
                        .Case(vm => vm.Id = ToDoItemId),
                    ToDoItemType.Circle => Resolver.Get<ValueToDoItemSettingsViewModel>()
                        .Case(vm => vm.Id = ToDoItemId),
                    ToDoItemType.Step => Resolver.Get<ValueToDoItemSettingsViewModel>().Case(vm => vm.Id = ToDoItemId),
                    ToDoItemType.Group => Resolver.Get<GroupToDoItemSettingsViewModel>(),
                    _ => throw new ArgumentOutOfRangeException()
                }
            );

        return RefreshAsync(cancellationToken);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var toDoItem = await ToDoService.GetToDoItemAsync(ToDoItemId, cancellationToken)
            .ConfigureAwait(false);

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                ToDoItemContent.Name = toDoItem.Name;
                ToDoItemContent.Link = toDoItem.Link?.AbsoluteUri ?? string.Empty;
                ToDoItemContent.Type = toDoItem.Type;
            }
        );
    }

    public override void Stop()
    {
    }

    public override Task SetStateAsync(object setting)
    {
        return Task.CompletedTask;
    }

    public override Task SaveStateAsync()
    {
        return Task.CompletedTask;
    }
}