using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Ninject;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Features.Localizations.Models;
using Spravy.Ui.Models;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class DeleteToDoItemViewModel : ViewModelBase
{
    public DeleteToDoItemViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        this.WhenAnyValue(x => x.ToDoItemName).Subscribe(_ => this.RaisePropertyChanged(nameof(DeleteText)));
    }

    public ICommand InitializedCommand { get; }

    [Reactive]
    public object[] Path { get; set; } = Array.Empty<object>();

    [Reactive]
    public Guid ToDoItemId { get; set; }

    [Reactive]
    public string ToDoItemName { get; set; } = string.Empty;

    [Reactive]
    public string ChildrenText { get; set; } = string.Empty;

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; set; }

    public HeaderView DeleteText =>
        new(
            "DeleteToDoItemView.Header",
            new
            {
                ToDoItemName
            }
        );

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        var item = await ToDoService.GetToDoItemAsync(ToDoItemId, cancellationToken);
        var toDoItemToStringOptions = new ToDoItemToStringOptions(Enum.GetValues<ToDoItemStatus>(), ToDoItemId);
        var childrenText = await ToDoService.ToDoItemToStringAsync(toDoItemToStringOptions, cancellationToken);
        var parents = await ToDoService.GetParentsAsync(ToDoItemId, cancellationToken).ConfigureAwait(false);

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                Path = new RootItem().To<object>()
                    .ToEnumerable()
                    .Concat(parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)))
                    .ToArray();

                ToDoItemName = item.Name;
                ChildrenText = childrenText;
            }
        );
    }
}