namespace Spravy.Ui.Services;

public class SpravyCommandNotifyService
{
    public SpravyCommandNotifyService(SpravyCommandService commandService)
    {
        MultiComplete = new("mdi-check", new("Command.Complete"), commandService.MultiComplete);

        MultiAddToFavorite = new(
            "mdi-star-outline",
            new("Command.AddToFavorite"),
            commandService.MultiAddToFavorite
        );

        MultiRemoveFromFavorite = new(
            "mdi-star",
            new("Command.RemoveFromFavorite"),
            commandService.MultiRemoveFromFavorite
        );

        MultiOpenLink = new("mdi-link", new("Command.OpenLink"), commandService.MultiOpenLink);

        MultiAddChild = new(
            "mdi-plus",
            new("Command.AddChildToDoItem"),
            commandService.MultiAddChild
        );

        MultiDelete = new("mdi-delete", new("Command.Delete"), commandService.MultiDelete);

        MultiShowSetting = new("mdi-cog", new("Command.Setting"), commandService.MultiShowSetting);

        MultiOpenLeaf = new("mdi-leaf", new("Command.OpenLeaf"), commandService.MultiOpenLeaf);

        MultiChangeParent = new(
            "mdi-swap-horizontal",
            new("Command.ChangeParent"),
            commandService.MultiChangeParent
        );

        MultiMakeAsRoot = new(
            "mdi-family-tree",
            new("Command.MakeAsRootToDoItem"),
            commandService.MultiMakeAsRoot
        );

        MultiCopyToClipboard = new(
            "mdi-clipboard",
            new("Command.CopyToClipboard"),
            commandService.MultiCopyToClipboard
        );

        MultiRandomizeChildrenOrder = new(
            "mdi-dice-6",
            new("Command.RandomizeChildrenOrder"),
            commandService.MultiRandomizeChildrenOrder
        );

        MultiChangeOrder = new(
            "mdi-reorder-horizontal",
            new("Command.Reorder"),
            commandService.MultiChangeOrder
        );

        MultiReset = new("mdi-refresh", new("Command.Reset"), commandService.MultiReset);

        MultiClone = new("mdi-copyleft", new("Command.Clone"), commandService.MultiClone);

        MultiCreateReference = new(
            "mdi-link-variant",
            new("Command.CreateReference"),
            commandService.MultiCreateReference
        );

        MultiCompleteToDoItem = new(
            "mdi-check",
            new("Command.Complete"),
            commandService.MultiCompleteToDoItem
        );

        MultiAddToFavoriteToDoItem = new(
            "mdi-star-outline",
            new("Command.AddToFavorite"),
            commandService.MultiAddToFavoriteToDoItem
        );

        MultiRemoveFromFavoriteToDoItem = new(
            "mdi-star",
            new("Command.RemoveFromFavorite"),
            commandService.MultiRemoveFromFavoriteToDoItem
        );

        MultiOpenLinkToDoItem = new(
            "mdi-link",
            new("Command.OpenLink"),
            commandService.MultiOpenLinkToDoItem
        );

        MultiAddChildToDoItem = new(
            "mdi-plus",
            new("Command.AddChildToDoItem"),
            commandService.MultiAddChildToDoItem
        );

        MultiDeleteToDoItem = new(
            "mdi-delete",
            new("Command.Delete"),
            commandService.MultiDeleteToDoItem
        );

        MultiShowSettingToDoItem = new(
            "mdi-cog",
            new("Command.Setting"),
            commandService.MultiShowSettingToDoItem
        );

        MultiOpenLeafToDoItem = new(
            "mdi-leaf",
            new("Command.OpenLeaf"),
            commandService.MultiOpenLeafToDoItem
        );

        MultiChangeParentToDoItem = new(
            "mdi-swap-horizontal",
            new("Command.ChangeParent"),
            commandService.MultiChangeParentToDoItem
        );

        MultiMakeAsRootToDoItem = new(
            "mdi-family-tree",
            new("Command.MakeAsRootToDoItem"),
            commandService.MultiMakeAsRootToDoItem
        );

        MultiCopyToClipboardToDoItem = new(
            "mdi-clipboard",
            new("Command.CopyToClipboard"),
            commandService.MultiCopyToClipboardToDoItem
        );

        MultiRandomizeChildrenOrderToDoItem = new(
            "mdi-dice-6",
            new("Command.RandomizeChildrenOrder"),
            commandService.MultiRandomizeChildrenOrderToDoItem
        );

        MultiChangeOrderToDoItem = new(
            "mdi-reorder-horizontal",
            new("Command.Reorder"),
            commandService.MultiChangeOrderToDoItem
        );

        MultiResetToDoItem = new(
            "mdi-refresh",
            new("Command.Reset"),
            commandService.MultiResetToDoItem
        );

        MultiCloneToDoItem = new(
            "mdi-copyleft",
            new("Command.Clone"),
            commandService.MultiCloneToDoItem
        );

        MultiCreateReferenceToDoItem = new(
            "mdi-link-variant",
            new("Command.CreateReference"),
            commandService.MultiCreateReferenceToDoItem
        );

        Complete = new("mdi-check", new("Command.Complete"), commandService.Complete);

        AddToFavorite = new(
            "mdi-star-outline",
            new("Command.AddToFavorite"),
            commandService.AddToFavorite
        );

        RemoveFromFavorite = new(
            "mdi-star",
            new("Command.RemoveFromFavorite"),
            commandService.RemoveFromFavorite
        );

        AddToBookmark = new(
            "mdi-bookmark-outline",
            new("Command.AddToBookmark"),
            commandService.AddToBookmark
        );

        RemoveFromBookmark = new(
            "mdi-bookmark",
            new("Command.RemoveFromBookmark"),
            commandService.RemoveFromBookmark
        );

        OpenLink = new("mdi-link", new("Command.OpenLink"), commandService.OpenLink);

        AddChild = new("mdi-plus", new("Command.AddChildToDoItem"), commandService.AddChild);

        Delete = new("mdi-delete", new("Command.Delete"), commandService.Delete);

        ShowSetting = new("mdi-cog", new("Command.Setting"), commandService.ShowSetting);

        OpenLeaf = new("mdi-leaf", new("Command.OpenLeaf"), commandService.OpenLeaf);

        ChangeParent = new(
            "mdi-swap-horizontal",
            new("Command.ChangeParent"),
            commandService.ChangeParent
        );
        MakeAsRoot = new(
            "mdi-family-tree",
            new("Command.MakeAsRootToDoItem"),
            commandService.MakeAsRoot
        );

        CopyToClipboard = new(
            "mdi-clipboard",
            new("Command.CopyToClipboard"),
            commandService.ToDoItemCopyToClipboard
        );

        RandomizeChildrenOrder = new(
            "mdi-dice-6",
            new("Command.RandomizeChildrenOrder"),
            commandService.RandomizeChildrenOrder
        );

        CreateTimer = new("mdi-timer", new("Command.CreateTimer"), commandService.CreateTimer);

        ChangeOrder = new(
            "mdi-reorder-horizontal",
            new("Command.Reorder"),
            commandService.ChangeOrder
        );

        Reset = new("mdi-refresh", new("Command.Reset"), commandService.Reset);
        Clone = new("mdi-copyleft", new("Command.Clone"), commandService.Clone);

        CreateReference = new(
            "mdi-link-variant",
            new("Command.CreateReference"),
            commandService.CreateReference
        );

        NavigateToCurrentToDoItem = new(
            "mdi-arrow-right",
            new("Command.OpenCurrent"),
            commandService.NavigateToCurrentToDoItem,
            new(Key.Q, KeyModifiers.Control)
        );

        LeafToDoItemsMulti = new[]
        {
            MultiClone,
            MultiComplete,
            MultiDelete,
            MultiReset,
            MultiAddChild,
            MultiChangeOrder,
            MultiChangeParent,
            MultiOpenLeaf,
            MultiOpenLink,
            MultiShowSetting,
            MultiAddToFavorite,
            MultiCopyToClipboard,
            MultiRemoveFromFavorite,
            MultiRandomizeChildrenOrder,
            MultiMakeAsRoot,
            MultiCreateReference,
        };

        RootToDoItemsMulti = new[]
        {
            MultiClone,
            MultiComplete,
            MultiDelete,
            MultiReset,
            MultiAddChild,
            MultiChangeOrder,
            MultiChangeParent,
            MultiOpenLeaf,
            MultiOpenLink,
            MultiShowSetting,
            MultiAddToFavorite,
            MultiCopyToClipboard,
            MultiRemoveFromFavorite,
            MultiRandomizeChildrenOrder,
            MultiCreateReference,
        };

        TodayToDoItemsMulti = new[]
        {
            MultiClone,
            MultiComplete,
            MultiDelete,
            MultiReset,
            MultiAddChild,
            MultiChangeOrder,
            MultiChangeParent,
            MultiOpenLeaf,
            MultiOpenLink,
            MultiShowSetting,
            MultiAddToFavorite,
            MultiCopyToClipboard,
            MultiRemoveFromFavorite,
            MultiRandomizeChildrenOrder,
            MultiMakeAsRoot,
            MultiCreateReference,
        };

        SearchToDoItemsMulti = new[]
        {
            MultiComplete,
            MultiDelete,
            MultiReset,
            MultiAddChild,
            MultiClone,
            MultiChangeOrder,
            MultiChangeParent,
            MultiOpenLeaf,
            MultiOpenLink,
            MultiShowSetting,
            MultiAddToFavorite,
            MultiCopyToClipboard,
            MultiRemoveFromFavorite,
            MultiRandomizeChildrenOrder,
            MultiMakeAsRoot,
            MultiCreateReference,
        };
    }

    public SpravyCommandNotify MultiComplete { get; }
    public SpravyCommandNotify MultiAddToFavorite { get; }
    public SpravyCommandNotify MultiRemoveFromFavorite { get; }
    public SpravyCommandNotify MultiOpenLink { get; }
    public SpravyCommandNotify MultiAddChild { get; }
    public SpravyCommandNotify MultiDelete { get; }
    public SpravyCommandNotify MultiShowSetting { get; }
    public SpravyCommandNotify MultiOpenLeaf { get; }
    public SpravyCommandNotify MultiChangeParent { get; }
    public SpravyCommandNotify MultiMakeAsRoot { get; }
    public SpravyCommandNotify MultiCopyToClipboard { get; }
    public SpravyCommandNotify MultiRandomizeChildrenOrder { get; }
    public SpravyCommandNotify MultiChangeOrder { get; }
    public SpravyCommandNotify MultiReset { get; }
    public SpravyCommandNotify MultiClone { get; }
    public SpravyCommandNotify MultiCreateReference { get; }

    public SpravyCommandNotify MultiCompleteToDoItem { get; }
    public SpravyCommandNotify MultiAddToFavoriteToDoItem { get; }
    public SpravyCommandNotify MultiRemoveFromFavoriteToDoItem { get; }
    public SpravyCommandNotify MultiOpenLinkToDoItem { get; }
    public SpravyCommandNotify MultiAddChildToDoItem { get; }
    public SpravyCommandNotify MultiDeleteToDoItem { get; }
    public SpravyCommandNotify MultiShowSettingToDoItem { get; }
    public SpravyCommandNotify MultiOpenLeafToDoItem { get; }
    public SpravyCommandNotify MultiChangeParentToDoItem { get; }
    public SpravyCommandNotify MultiMakeAsRootToDoItem { get; }
    public SpravyCommandNotify MultiCopyToClipboardToDoItem { get; }
    public SpravyCommandNotify MultiRandomizeChildrenOrderToDoItem { get; }
    public SpravyCommandNotify MultiChangeOrderToDoItem { get; }
    public SpravyCommandNotify MultiResetToDoItem { get; }
    public SpravyCommandNotify MultiCloneToDoItem { get; }
    public SpravyCommandNotify MultiCreateReferenceToDoItem { get; }

    public SpravyCommandNotify NavigateToCurrentToDoItem { get; }
    public SpravyCommandNotify Complete { get; }
    public SpravyCommandNotify CreateTimer { get; }
    public SpravyCommandNotify AddToFavorite { get; }
    public SpravyCommandNotify RemoveFromFavorite { get; }
    public SpravyCommandNotify OpenLink { get; }
    public SpravyCommandNotify AddChild { get; }
    public SpravyCommandNotify Delete { get; }
    public SpravyCommandNotify ShowSetting { get; }
    public SpravyCommandNotify OpenLeaf { get; }
    public SpravyCommandNotify ChangeParent { get; }
    public SpravyCommandNotify MakeAsRoot { get; }
    public SpravyCommandNotify CopyToClipboard { get; }
    public SpravyCommandNotify RandomizeChildrenOrder { get; }
    public SpravyCommandNotify ChangeOrder { get; }
    public SpravyCommandNotify Reset { get; }
    public SpravyCommandNotify Clone { get; }
    public SpravyCommandNotify CreateReference { get; }
    public SpravyCommandNotify AddToBookmark { get; }
    public SpravyCommandNotify RemoveFromBookmark { get; }

    public ReadOnlyMemory<SpravyCommandNotify> LeafToDoItemsMulti { get; }
    public ReadOnlyMemory<SpravyCommandNotify> RootToDoItemsMulti { get; }
    public ReadOnlyMemory<SpravyCommandNotify> TodayToDoItemsMulti { get; }
    public ReadOnlyMemory<SpravyCommandNotify> SearchToDoItemsMulti { get; }
}
