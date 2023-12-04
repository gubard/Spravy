using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Threading;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddToDoItemViewModel : NavigatableViewModelBase
{
    private ToDoItemNotify? parent;
    private string name = string.Empty;
    private ToDoItemType type;

    public AddToDoItemViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        ToDoItemTypes = new(Enum.GetValues<ToDoItemType>());
    }

    public ICommand InitializedCommand { get; }
    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required PathViewModel PathViewModel { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    public ToDoItemType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }

    public ToDoItemNotify? Parent
    {
        get => parent;
        set => this.RaiseAndSetIfChanged(ref parent, value);
    }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var parents = await ToDoService.GetParentsAsync(Parent.ThrowIfNull().Id, cancellationToken)
            .ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                PathViewModel.Items.Clear();
                PathViewModel.Items.Add(new RootItem());
                PathViewModel.Items.AddRange(parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));
            }
        );
    }
}