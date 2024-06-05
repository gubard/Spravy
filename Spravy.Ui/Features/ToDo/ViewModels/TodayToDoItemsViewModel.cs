namespace Spravy.Ui.Features.ToDo.ViewModels;

public class TodayToDoItemsViewModel : NavigatableViewModelBase, IRefresh, IToDoSubItemsViewModelProperty
{
    public TodayToDoItemsViewModel(
        PageHeaderViewModel pageHeaderViewModel,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IToDoService toDoService,
        INavigator navigator,
        IUiApplicationService uiApplicationService,
        IDialogViewer dialogViewer,
        IConverter converter,
        IClipboardService clipboardService,
        IOpenerLink openerLink,
        IErrorHandler errorHandler
    ) : base(true)
    {
        PageHeaderViewModel = pageHeaderViewModel;
        PageHeaderViewModel.Header = "Today to-do";
        PageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        
        ToDoSubItemsViewModel.List
           .WhenAnyValue(x => x.IsMulti)
           .Subscribe(x =>
            {
                PageHeaderViewModel.Commands.Clear();
                
                if (x)
                {
                    PageHeaderViewModel.Commands.AddRange([
                        SpravyCommandNotify.CreateMultiCloneItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiCompleteItem(uiApplicationService, toDoService, errorHandler),
                        SpravyCommandNotify.CreateMultiDeleteItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiResetItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiAddChildItem(uiApplicationService, toDoService, dialogViewer,
                            converter, errorHandler),
                        SpravyCommandNotify.CreateMultiCloneItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiChangeOrderItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiChangeParentItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiOpenLeafItem(uiApplicationService, navigator, errorHandler),
                        SpravyCommandNotify.CreateMultiOpenLinkItem(uiApplicationService, openerLink, errorHandler),
                        SpravyCommandNotify.CreateMultiShowSettingItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiAddToFavoriteItem(uiApplicationService, toDoService,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiCopyToClipboardItem(uiApplicationService, toDoService,
                            dialogViewer,
                            clipboardService, errorHandler),
                        SpravyCommandNotify.CreateMultiRemoveFromFavoriteItem(uiApplicationService, toDoService,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiRandomizeChildrenOrderItem(uiApplicationService, toDoService,
                            dialogViewer, errorHandler),
                        SpravyCommandNotify.CreateMultiMakeAsRootItem(uiApplicationService, toDoService, errorHandler),
                    ]);
                }
            });
        
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required IToDoCache ToDoCache { get; init; }
    
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public PageHeaderViewModel PageHeaderViewModel { get; }
    public ICommand InitializedCommand { get; }
    
    public override string ViewId
    {
        get => TypeCache<TodayToDoItemsViewModel>.Type.Name;
    }
    
    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetTodayToDoItemsAsync(cancellationToken)
           .IfSuccessForEachAsync(id => ToDoCache.GetToDoItem(id), cancellationToken)
           .IfSuccessAsync(ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, false, cancellationToken),
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