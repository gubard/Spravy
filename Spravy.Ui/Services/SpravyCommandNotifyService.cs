namespace Spravy.Ui.Services;

public class SpravyCommandNotifyService
{
    public SpravyCommandNotify MultiCompleteItem { get; }
    public SpravyCommandNotify MultiAddToFavoriteItem { get; }
    public SpravyCommandNotify MultiRemoveFromFavoriteItem { get; }
    public SpravyCommandNotify MultiOpenLinkItem { get; }
    public SpravyCommandNotify MultiAddChildItem { get; }
    public SpravyCommandNotify MultiDeleteItem { get; }
    public SpravyCommandNotify MultiShowSettingItem { get; }
    public SpravyCommandNotify MultiOpenLeafItem { get; }
    public SpravyCommandNotify MultiChangeParentItem { get; }
    public SpravyCommandNotify MultiMakeAsRootItem { get; }
    public SpravyCommandNotify MultiCopyToClipboardItem { get; }
    public SpravyCommandNotify MultiRandomizeChildrenOrderItem { get; }
    public SpravyCommandNotify MultiChangeOrderItem { get; }
    public SpravyCommandNotify MultiResetItem { get; }
    public SpravyCommandNotify MultiCloneItem { get; }
    public ReadOnlyMemory<SpravyCommandNotify> LeafToDoItemsMultiItems { get; }
    public ReadOnlyMemory<SpravyCommandNotify> RootToDoItemsMultiItems { get; }
    public ReadOnlyMemory<SpravyCommandNotify> TodayToDoItemsMultiItems { get; }
    public ReadOnlyMemory<SpravyCommandNotify> SearchToDoItemsMultiItems { get; }

    public SpravyCommandNotifyService(SpravyCommandService commandService)
    {
        MultiCompleteItem = new(MaterialIconKind.Check, new("Command.Complete"), commandService.MultiComplete);

        MultiAddToFavoriteItem = new(MaterialIconKind.StarOutline, new("Command.AddToFavorite"),
            commandService.MultiAddToFavorite);

        MultiRemoveFromFavoriteItem = new(MaterialIconKind.Star, new("Command.RemoveFromFavorite"),
            commandService.MultiRemoveFromFavorite);

        MultiOpenLinkItem = new(MaterialIconKind.Link, new("Command.OpenLink"), commandService.MultiOpenLink);
        MultiAddChildItem = new(MaterialIconKind.Plus, new("Command.AddChildToDoItem"), commandService.MultiAddChild);
        MultiDeleteItem = new(MaterialIconKind.Delete, new("Command.Delete"), commandService.MultiDelete);
        MultiShowSettingItem = new(MaterialIconKind.Settings, new("Command.Settings"), commandService.MultiShowSetting);
        MultiOpenLeafItem = new(MaterialIconKind.Leaf, new("Command.OpenLeaf"), commandService.MultiOpenLeaf);

        MultiChangeParentItem = new(MaterialIconKind.SwapHorizontal, new("Command.ChangeParent"),
            commandService.MultiChangeParent);

        MultiMakeAsRootItem = new(MaterialIconKind.FamilyTree, new("Command.MakeAsRootToDoItem"),
            commandService.MultiMakeAsRoot);

        MultiCopyToClipboardItem = new(MaterialIconKind.Clipboard, new("Command.CopyToClipboard"),
            commandService.MultiCopyToClipboard);

        MultiRandomizeChildrenOrderItem = new(MaterialIconKind.DiceSix, new("Command.RandomizeChildrenOrder"),
            commandService.MultiRandomizeChildrenOrder);

        MultiChangeOrderItem = new(MaterialIconKind.ReorderHorizontal, new("Command.Reorder"),
            commandService.MultiChangeOrder);

        MultiResetItem = new(MaterialIconKind.Refresh, new("Command.Reset"), commandService.MultiReset);
        MultiCloneItem = new(MaterialIconKind.Copyleft, new("Command.Clone"), commandService.MultiClone);

        LeafToDoItemsMultiItems = new[]
        {
            MultiCloneItem,
            MultiCompleteItem,
            MultiDeleteItem,
            MultiResetItem,
            MultiAddChildItem,
            MultiCloneItem,
            MultiChangeOrderItem,
            MultiChangeParentItem,
            MultiOpenLeafItem,
            MultiOpenLinkItem,
            MultiShowSettingItem,
            MultiAddToFavoriteItem,
            MultiCopyToClipboardItem,
            MultiRemoveFromFavoriteItem,
            MultiRandomizeChildrenOrderItem,
            MultiMakeAsRootItem,
        };

        RootToDoItemsMultiItems = new[]
        {
            MultiCloneItem,
            MultiCompleteItem,
            MultiDeleteItem,
            MultiResetItem,
            MultiAddChildItem,
            MultiCloneItem,
            MultiChangeOrderItem,
            MultiChangeParentItem,
            MultiOpenLeafItem,
            MultiOpenLinkItem,
            MultiShowSettingItem,
            MultiAddToFavoriteItem,
            MultiCopyToClipboardItem,
            MultiRemoveFromFavoriteItem,
            MultiRandomizeChildrenOrderItem,
        };

        TodayToDoItemsMultiItems = new[]
        {
            MultiCloneItem,
            MultiCompleteItem,
            MultiDeleteItem,
            MultiResetItem,
            MultiAddChildItem,
            MultiCloneItem,
            MultiChangeOrderItem,
            MultiChangeParentItem,
            MultiOpenLeafItem,
            MultiOpenLinkItem,
            MultiShowSettingItem,
            MultiAddToFavoriteItem,
            MultiCopyToClipboardItem,
            MultiRemoveFromFavoriteItem,
            MultiRandomizeChildrenOrderItem,
            MultiMakeAsRootItem,
        };

        SearchToDoItemsMultiItems = new[]
        {
            MultiCloneItem,
            MultiCompleteItem,
            MultiDeleteItem,
            MultiResetItem,
            MultiAddChildItem,
            MultiCloneItem,
            MultiChangeOrderItem,
            MultiChangeParentItem,
            MultiOpenLeafItem,
            MultiOpenLinkItem,
            MultiShowSettingItem,
            MultiAddToFavoriteItem,
            MultiCopyToClipboardItem,
            MultiRemoveFromFavoriteItem,
            MultiRandomizeChildrenOrderItem,
            MultiMakeAsRootItem,
        };
    }
}