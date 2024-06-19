namespace Spravy.Ui.Features.ToDo.Commands;

public class RootToDoItemsCommands
{
    public RootToDoItemsCommands(
        IToDoService toDoService,
        INavigator navigator,
        IUiApplicationService uiApplicationService,
        IDialogViewer dialogViewer,
        IClipboardService clipboardService,
        IOpenerLink openerLink,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        Commands = new[]
        {
            SpravyCommandNotify.CreateMultiCloneItem(uiApplicationService, toDoService, dialogViewer, errorHandler,
                taskProgressService),
            SpravyCommandNotify.CreateMultiCompleteItem(uiApplicationService, toDoService, errorHandler,
                taskProgressService),
            SpravyCommandNotify.CreateMultiDeleteItem(uiApplicationService, toDoService, dialogViewer, errorHandler,
                taskProgressService),
            SpravyCommandNotify.CreateMultiResetItem(uiApplicationService, toDoService, dialogViewer, errorHandler,
                taskProgressService),
            SpravyCommandNotify.CreateMultiAddChildItem(uiApplicationService, toDoService, dialogViewer, errorHandler,
                taskProgressService),
            SpravyCommandNotify.CreateMultiCloneItem(uiApplicationService, toDoService, dialogViewer, errorHandler,
                taskProgressService),
            SpravyCommandNotify.CreateMultiChangeOrderItem(uiApplicationService, toDoService, dialogViewer,
                errorHandler, taskProgressService),
            SpravyCommandNotify.CreateMultiChangeParentItem(uiApplicationService, toDoService, dialogViewer,
                errorHandler, taskProgressService),
            SpravyCommandNotify.CreateMultiOpenLeafItem(uiApplicationService, navigator, errorHandler,
                taskProgressService),
            SpravyCommandNotify.CreateMultiOpenLinkItem(uiApplicationService, openerLink, errorHandler,
                taskProgressService),
            SpravyCommandNotify.CreateMultiShowSettingItem(uiApplicationService, toDoService, dialogViewer,
                errorHandler, taskProgressService),
            SpravyCommandNotify.CreateMultiAddToFavoriteItem(uiApplicationService, toDoService, errorHandler,
                taskProgressService),
            SpravyCommandNotify.CreateMultiCopyToClipboardItem(uiApplicationService, toDoService, dialogViewer,
                clipboardService, errorHandler, taskProgressService),
            SpravyCommandNotify.CreateMultiRemoveFromFavoriteItem(uiApplicationService, toDoService, errorHandler,
                taskProgressService),
            SpravyCommandNotify.CreateMultiRandomizeChildrenOrderItem(uiApplicationService, toDoService, dialogViewer,
                errorHandler, taskProgressService),
        };
    }

    public ReadOnlyMemory<SpravyCommandNotify> Commands { get; }
}