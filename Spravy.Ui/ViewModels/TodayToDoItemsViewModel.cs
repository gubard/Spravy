using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.ViewModels;

public class TodayToDoItemsViewModel : NavigatableViewModelBase, IRefresh
{
    private readonly PageHeaderViewModel pageHeaderViewModel;

    public TodayToDoItemsViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }

    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel
    {
        get => pageHeaderViewModel;
        [MemberNotNull(nameof(pageHeaderViewModel))]
        init
        {
            pageHeaderViewModel = value;
            pageHeaderViewModel.Header = "Today to-do";
            pageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;
        }
    }

    public ICommand InitializedCommand { get; }
    public override string ViewId => TypeCache<TodayToDoItemsViewModel>.Type.Name;

    private ValueTask<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ValueTask<Result> SaveStateAsync()
    {
        return Result.SuccessValueTask;
    }

    public override ValueTask<Result> SetStateAsync(object setting)
    {
        return Result.SuccessValueTask;
    }

    public ValueTask<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetTodayToDoItemsAsync(cancellationToken)
            .ConfigureAwait(false)
            .IfSuccessAsync(
                ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, false, cancellationToken)
                    .ConfigureAwait(false)
            );
    }
}