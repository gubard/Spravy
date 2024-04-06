using System;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Spravy.Ui.Services;

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

    public Header4View DeleteText =>
        new(
            "DeleteToDoItemView.Header",
            new
            {
                ToDoItemName
            }
        );

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        var toDoItemToStringOptions = new ToDoItemToStringOptions(Enum.GetValues<ToDoItemStatus>(), ToDoItemId);

        return ToDoService.GetToDoItemAsync(ToDoItemId, cancellationToken)
            .IfSuccessAsync(
                item => ToDoService.ToDoItemToStringAsync(toDoItemToStringOptions, cancellationToken)
                    .IfSuccessAsync(
                        childrenText => ToDoService.GetParentsAsync(ToDoItemId, cancellationToken)
                            .IfSuccessAsync(
                                parents => this.InvokeUIBackgroundAsync(
                                        () =>
                                        {
                                            Path = new RootItem().To<object>()
                                                .ToEnumerable()
                                                .Concat(Mapper.Map<ToDoItemParentNotify[]>(parents.ToArray()))
                                                .ToArray();

                                            ToDoItemName = item.Name;
                                            ChildrenText = childrenText;
                                        }
                                    )
                            )
                    )
            );
    }
}