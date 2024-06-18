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
        IErrorHandler errorHandler
    )
    {
        Commands = new[]
        {
            SpravyCommandNotify.CreateMultiCloneItem(uiApplicationService, toDoService, dialogViewer, errorHandler),
            SpravyCommandNotify.CreateMultiCompleteItem(uiApplicationService, toDoService, errorHandler),
            SpravyCommandNotify.CreateMultiDeleteItem(uiApplicationService, toDoService, dialogViewer, errorHandler),
            SpravyCommandNotify.CreateMultiResetItem(uiApplicationService, toDoService, dialogViewer, errorHandler),
            SpravyCommandNotify.CreateMultiAddChildItem(uiApplicationService, toDoService, dialogViewer, errorHandler),
            SpravyCommandNotify.CreateMultiCloneItem(uiApplicationService, toDoService, dialogViewer, errorHandler),
            SpravyCommandNotify.CreateMultiChangeOrderItem(uiApplicationService, toDoService, dialogViewer,
                errorHandler),
            SpravyCommandNotify.CreateMultiChangeParentItem(uiApplicationService, toDoService, dialogViewer,
                errorHandler),
            SpravyCommandNotify.CreateMultiOpenLeafItem(uiApplicationService, navigator, errorHandler),
            SpravyCommandNotify.CreateMultiOpenLinkItem(uiApplicationService, openerLink, errorHandler),
            SpravyCommandNotify.CreateMultiShowSettingItem(uiApplicationService, toDoService, dialogViewer, errorHandler),
            SpravyCommandNotify.CreateMultiAddToFavoriteItem(uiApplicationService, toDoService, errorHandler),
            SpravyCommandNotify.CreateMultiCopyToClipboardItem(uiApplicationService, toDoService, dialogViewer,
                clipboardService, errorHandler),
            SpravyCommandNotify.CreateMultiRemoveFromFavoriteItem(uiApplicationService, toDoService, errorHandler),
            SpravyCommandNotify.CreateMultiRandomizeChildrenOrderItem(uiApplicationService, toDoService, dialogViewer,
                errorHandler),
        };
    }
    
    public ReadOnlyMemory<SpravyCommandNotify> Commands { get; }
}