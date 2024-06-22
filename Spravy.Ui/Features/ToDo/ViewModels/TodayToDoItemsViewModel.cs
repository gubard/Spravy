namespace Spravy.Ui.Features.ToDo.ViewModels;

public class TodayToDoItemsViewModel : NavigatableViewModelBase, IRefresh, IToDoSubItemsViewModelProperty
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;

    public TodayToDoItemsViewModel(
        PageHeaderViewModel pageHeaderViewModel,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IToDoService toDoService,
        INavigator navigator,
        IUiApplicationService uiApplicationService,
        IDialogViewer dialogViewer,
        IClipboardService clipboardService,
        IOpenerLink openerLink,
        IErrorHandler errorHandler,
        IToDoCache toDoCache,
        SpravyCommandNotifyService spravyCommandNotifyService,
        ITaskProgressService taskProgressService
    ) : base(true)
    {
        PageHeaderViewModel = pageHeaderViewModel;
        PageHeaderViewModel.Header = "Today to-do";
        PageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;

        ToDoSubItemsViewModel.List
           .WhenAnyValue(x => x.IsMulti)
           .Subscribe(x =>
            {
                PageHeaderViewModel.Commands.Clear();

                if (x)
                {
                    PageHeaderViewModel.Commands.AddRange(spravyCommandNotifyService.TodayToDoItemsMultiItems
                       .ToArray());
                }
            });

        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);
    }

    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public PageHeaderViewModel PageHeaderViewModel { get; }
    public SpravyCommand InitializedCommand { get; }

    public override string ViewId
    {
        get => TypeCache<TodayToDoItemsViewModel>.Type.Name;
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return toDoService.GetTodayToDoItemsAsync(cancellationToken)
           .IfSuccessForEachAsync(id => toDoCache.GetToDoItem(id), cancellationToken)
           .IfSuccessAsync(ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), false, cancellationToken),
                cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableSuccess;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableSuccess;
    }
}