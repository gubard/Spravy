namespace Spravy.Ui.Models;

public class SpravyCommandNotify : NotifyBase
{
    private static SpravyCommandNotify? _multiCompleteItem;
    private static SpravyCommandNotify? _multiAddToFavoriteItem;
    private static SpravyCommandNotify? _multiRemoveFromFavoriteItem;
    private static SpravyCommandNotify? _multiOpenLinkItem;
    private static SpravyCommandNotify? _multiAddChildItem;
    private static SpravyCommandNotify? _multiDeleteItem;
    private static SpravyCommandNotify? _multiShowSettingItem;
    private static SpravyCommandNotify? _multiOpenLeafItem;
    private static SpravyCommandNotify? _multiChangeParentItem;
    private static SpravyCommandNotify? _multiMakeAsRootItem;
    private static SpravyCommandNotify? _multiCopyToClipboardItem;
    private static SpravyCommandNotify? _multiRandomizeChildrenOrderItem;
    private static SpravyCommandNotify? _multiChangeOrderItem;
    private static SpravyCommandNotify? _multiResetItem;
    private static SpravyCommandNotify? _multiCloneItem;
    
    public SpravyCommandNotify(MaterialIconKind kind, TextLocalization text, SpravyCommand item)
    {
        Item = item;
        Text = text;
        Kind = kind;
    }
    
    public SpravyCommand Item { get; }
    public MaterialIconKind Kind { get; }
    public TextLocalization Text { get; }
    
    public static SpravyCommandNotify CreateMultiCompleteItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IErrorHandler errorHandler
    )
    {
        if (_multiCompleteItem is not null)
        {
            return _multiCompleteItem;
        }
        
        _multiCompleteItem = new(MaterialIconKind.Check, new("Command.Complete"),
            SpravyCommand.CreateMultiComplete(uiApplicationService, toDoService, errorHandler));
        
        return _multiCompleteItem;
    }
    
    public static SpravyCommandNotify CreateMultiAddToFavoriteItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IErrorHandler errorHandler
    )
    {
        if (_multiAddToFavoriteItem is not null)
        {
            return _multiAddToFavoriteItem;
        }
        
        _multiAddToFavoriteItem = new(MaterialIconKind.StarOutline, new("Command.AddToFavorite"),
            SpravyCommand.CreateMultiAddToFavorite(uiApplicationService, toDoService, errorHandler));
        
        return _multiAddToFavoriteItem;
    }
    
    public static SpravyCommandNotify CreateMultiRemoveFromFavoriteItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IErrorHandler errorHandler
    )
    {
        if (_multiRemoveFromFavoriteItem is not null)
        {
            return _multiRemoveFromFavoriteItem;
        }
        
        _multiRemoveFromFavoriteItem = new(MaterialIconKind.Star, new("Command.RemoveFromFavorite"),
            SpravyCommand.CreateMultiRemoveFromFavorite(uiApplicationService, toDoService, errorHandler));
        
        return _multiRemoveFromFavoriteItem;
    }
    
    public static SpravyCommandNotify CreateMultiOpenLinkItem(
        IUiApplicationService uiApplicationService,
        IOpenerLink openerLink,
        IErrorHandler errorHandler
    )
    {
        if (_multiOpenLinkItem is not null)
        {
            return _multiOpenLinkItem;
        }
        
        _multiOpenLinkItem = new(MaterialIconKind.Link, new("Command.OpenLink"),
            SpravyCommand.CreateMultiOpenLink(uiApplicationService, openerLink, errorHandler));
        
        return _multiOpenLinkItem;
    }
    
    public static SpravyCommandNotify CreateMultiAddChildItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IConverter converter,
        IErrorHandler errorHandler
    )
    {
        if (_multiAddChildItem is not null)
        {
            return _multiAddChildItem;
        }
        
        _multiAddChildItem = new(MaterialIconKind.Plus, new("Command.AddChildToDoItem"),
            SpravyCommand.CreateMultiAddChild(uiApplicationService, toDoService, dialogViewer, converter,
                errorHandler));
        
        return _multiAddChildItem;
    }
    
    public static SpravyCommandNotify CreateMultiDeleteItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler
    )
    {
        if (_multiDeleteItem is not null)
        {
            return _multiDeleteItem;
        }
        
        _multiDeleteItem = new(MaterialIconKind.Delete, new("Command.Delete"),
            SpravyCommand.CreateMultiDelete(uiApplicationService, toDoService, dialogViewer, errorHandler));
        
        return _multiDeleteItem;
    }
    
    public static SpravyCommandNotify CreateMultiShowSettingItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler
    )
    {
        if (_multiShowSettingItem is not null)
        {
            return _multiShowSettingItem;
        }
        
        _multiShowSettingItem = new(MaterialIconKind.Settings, new("Command.Settings"),
            SpravyCommand.CreateMultiShowSetting(uiApplicationService, toDoService, dialogViewer, errorHandler));
        
        return _multiShowSettingItem;
    }
    
    public static SpravyCommandNotify CreateMultiOpenLeafItem(
        IUiApplicationService uiApplicationService,
        INavigator navigator,
        IErrorHandler errorHandler
    )
    {
        if (_multiOpenLeafItem is not null)
        {
            return _multiOpenLeafItem;
        }
        
        _multiOpenLeafItem = new(MaterialIconKind.Leaf, new("Command.OpenLeaf"),
            SpravyCommand.CreateMultiOpenLeaf(uiApplicationService, navigator, errorHandler));
        
        return _multiOpenLeafItem;
    }
    
    public static SpravyCommandNotify CreateMultiChangeParentItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler
    )
    {
        if (_multiChangeParentItem is not null)
        {
            return _multiChangeParentItem;
        }
        
        _multiChangeParentItem = new(MaterialIconKind.SwapHorizontal, new("Command.ChangeParent"),
            SpravyCommand.CreateMultiChangeParent(uiApplicationService, toDoService, dialogViewer, errorHandler));
        
        return _multiChangeParentItem;
    }
    
    public static SpravyCommandNotify CreateMultiMakeAsRootItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IErrorHandler errorHandler
    )
    {
        if (_multiMakeAsRootItem is not null)
        {
            return _multiMakeAsRootItem;
        }
        
        _multiMakeAsRootItem = new(MaterialIconKind.FamilyTree, new("Command.MakeAsRootToDoItem"),
            SpravyCommand.CreateMultiMakeAsRoot(uiApplicationService, toDoService, errorHandler));
        
        return _multiMakeAsRootItem;
    }
    
    public static SpravyCommandNotify CreateMultiCopyToClipboardItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IClipboardService clipboardService,
        IErrorHandler errorHandler
    )
    {
        if (_multiCopyToClipboardItem is not null)
        {
            return _multiCopyToClipboardItem;
        }
        
        _multiCopyToClipboardItem = new(MaterialIconKind.Clipboard, new("Command.CopyToClipboard"),
            SpravyCommand.CreateMultiCopyToClipboard(uiApplicationService, toDoService, dialogViewer, clipboardService,
                errorHandler));
        
        return _multiCopyToClipboardItem;
    }
    
    public static SpravyCommandNotify CreateMultiRandomizeChildrenOrderItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler
    )
    {
        if (_multiRandomizeChildrenOrderItem is not null)
        {
            return _multiRandomizeChildrenOrderItem;
        }
        
        _multiRandomizeChildrenOrderItem = new(MaterialIconKind.DiceSix, new("Command.RandomizeChildrenOrder"),
            SpravyCommand.CreateMultiRandomizeChildrenOrder(uiApplicationService, toDoService, dialogViewer,
                errorHandler));
        
        return _multiRandomizeChildrenOrderItem;
    }
    
    public static SpravyCommandNotify CreateMultiChangeOrderItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler
    )
    {
        if (_multiChangeOrderItem is not null)
        {
            return _multiChangeOrderItem;
        }
        
        _multiChangeOrderItem = new(MaterialIconKind.ReorderHorizontal, new("Command.Reorder"),
            SpravyCommand.CreateMultiChangeOrder(uiApplicationService, toDoService, dialogViewer, errorHandler));
        
        return _multiChangeOrderItem;
    }
    
    public static SpravyCommandNotify CreateMultiResetItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler
    )
    {
        if (_multiResetItem is not null)
        {
            return _multiResetItem;
        }
        
        _multiResetItem = new(MaterialIconKind.Refresh, new("Command.Reset"),
            SpravyCommand.CreateMultiReset(uiApplicationService, toDoService, dialogViewer, errorHandler));
        
        return _multiResetItem;
    }
    
    public static SpravyCommandNotify CreateMultiCloneItem(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler
    )
    {
        if (_multiCloneItem is not null)
        {
            return _multiCloneItem;
        }
        
        _multiCloneItem = new(MaterialIconKind.Copyleft, new("Command.Clone"),
            SpravyCommand.CreateMultiClone(uiApplicationService, toDoService, dialogViewer, errorHandler));
        
        return _multiCloneItem;
    }
}