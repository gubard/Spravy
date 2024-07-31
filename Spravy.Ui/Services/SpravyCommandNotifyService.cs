using Avalonia.Input;

namespace Spravy.Ui.Services;

public class SpravyCommandNotifyService
{
    public SpravyCommandNotifyService(SpravyCommandService commandService)
    {
        MultiComplete = new(
            MaterialIconKind.Check,
            new("Command.Complete"),
            commandService.MultiComplete
        );

        MultiAddToFavorite = new(
            MaterialIconKind.StarOutline,
            new("Command.AddToFavorite"),
            commandService.MultiAddToFavorite
        );

        MultiRemoveFromFavorite = new(
            MaterialIconKind.Star,
            new("Command.RemoveFromFavorite"),
            commandService.MultiRemoveFromFavorite
        );

        MultiOpenLink = new(
            MaterialIconKind.Link,
            new("Command.OpenLink"),
            commandService.MultiOpenLink
        );

        MultiAddChild = new(
            MaterialIconKind.Plus,
            new("Command.AddChildToDoItem"),
            commandService.MultiAddChild
        );

        MultiDelete = new(
            MaterialIconKind.Delete,
            new("Command.Delete"),
            commandService.MultiDelete
        );

        MultiShowSetting = new(
            MaterialIconKind.Settings,
            new("Command.Setting"),
            commandService.MultiShowSetting
        );

        MultiOpenLeaf = new(
            MaterialIconKind.Leaf,
            new("Command.OpenLeaf"),
            commandService.MultiOpenLeaf
        );

        MultiChangeParent = new(
            MaterialIconKind.SwapHorizontal,
            new("Command.ChangeParent"),
            commandService.MultiChangeParent
        );

        MultiMakeAsRoot = new(
            MaterialIconKind.FamilyTree,
            new("Command.MakeAsRootToDoItem"),
            commandService.MultiMakeAsRoot
        );

        MultiCopyToClipboard = new(
            MaterialIconKind.Clipboard,
            new("Command.CopyToClipboard"),
            commandService.MultiCopyToClipboard
        );

        MultiRandomizeChildrenOrder = new(
            MaterialIconKind.DiceSix,
            new("Command.RandomizeChildrenOrder"),
            commandService.MultiRandomizeChildrenOrder
        );

        MultiChangeOrder = new(
            MaterialIconKind.ReorderHorizontal,
            new("Command.Reorder"),
            commandService.MultiChangeOrder
        );

        MultiReset = new(MaterialIconKind.Refresh, new("Command.Reset"), commandService.MultiReset);

        MultiClone = new(
            MaterialIconKind.Copyleft,
            new("Command.Clone"),
            commandService.MultiClone
        );

        MultiCreateReference = new(
            MaterialIconKind.LinkVariant,
            new("Command.CreateReference"),
            commandService.MultiCreateReference
        );

        MultiCompleteToDoItem = new(
            MaterialIconKind.Check,
            new("Command.Complete"),
            commandService.MultiCompleteToDoItem
        );

        MultiAddToFavoriteToDoItem = new(
            MaterialIconKind.StarOutline,
            new("Command.AddToFavorite"),
            commandService.MultiAddToFavoriteToDoItem
        );

        MultiRemoveFromFavoriteToDoItem = new(
            MaterialIconKind.Star,
            new("Command.RemoveFromFavorite"),
            commandService.MultiRemoveFromFavoriteToDoItem
        );

        MultiOpenLinkToDoItem = new(
            MaterialIconKind.Link,
            new("Command.OpenLink"),
            commandService.MultiOpenLinkToDoItem
        );

        MultiAddChildToDoItem = new(
            MaterialIconKind.Plus,
            new("Command.AddChildToDoItem"),
            commandService.MultiAddChildToDoItem
        );

        MultiDeleteToDoItem = new(
            MaterialIconKind.Delete,
            new("Command.Delete"),
            commandService.MultiDeleteToDoItem
        );

        MultiShowSettingToDoItem = new(
            MaterialIconKind.Settings,
            new("Command.Setting"),
            commandService.MultiShowSettingToDoItem
        );

        MultiOpenLeafToDoItem = new(
            MaterialIconKind.Leaf,
            new("Command.OpenLeaf"),
            commandService.MultiOpenLeafToDoItem
        );

        MultiChangeParentToDoItem = new(
            MaterialIconKind.SwapHorizontal,
            new("Command.ChangeParent"),
            commandService.MultiChangeParentToDoItem
        );

        MultiMakeAsRootToDoItem = new(
            MaterialIconKind.FamilyTree,
            new("Command.MakeAsRootToDoItem"),
            commandService.MultiMakeAsRootToDoItem
        );

        MultiCopyToClipboardToDoItem = new(
            MaterialIconKind.Clipboard,
            new("Command.CopyToClipboard"),
            commandService.MultiCopyToClipboardToDoItem
        );

        MultiRandomizeChildrenOrderToDoItem = new(
            MaterialIconKind.DiceSix,
            new("Command.RandomizeChildrenOrder"),
            commandService.MultiRandomizeChildrenOrderToDoItem
        );

        MultiChangeOrderToDoItem = new(
            MaterialIconKind.ReorderHorizontal,
            new("Command.Reorder"),
            commandService.MultiChangeOrderToDoItem
        );

        MultiResetToDoItem = new(
            MaterialIconKind.Refresh,
            new("Command.Reset"),
            commandService.MultiResetToDoItem
        );

        MultiCloneToDoItem = new(
            MaterialIconKind.Copyleft,
            new("Command.Clone"),
            commandService.MultiCloneToDoItem
        );

        MultiCreateReferenceToDoItem = new(
            MaterialIconKind.LinkVariant,
            new("Command.CreateReference"),
            commandService.MultiCreateReferenceToDoItem
        );

        Complete = new(MaterialIconKind.Check, new("Command.Complete"), commandService.Complete);

        AddToFavorite = new(
            MaterialIconKind.StarOutline,
            new("Command.AddToFavorite"),
            commandService.AddToFavorite
        );

        RemoveFromFavorite = new(
            MaterialIconKind.Star,
            new("Command.RemoveFromFavorite"),
            commandService.RemoveFromFavorite
        );

        OpenLink = new(MaterialIconKind.Link, new("Command.OpenLink"), commandService.OpenLink);

        AddChild = new(
            MaterialIconKind.Plus,
            new("Command.AddChildToDoItem"),
            commandService.AddChild
        );

        Delete = new(MaterialIconKind.Delete, new("Command.Delete"), commandService.Delete);

        ShowSetting = new(
            MaterialIconKind.Settings,
            new("Command.Setting"),
            commandService.ShowSetting
        );

        OpenLeaf = new(MaterialIconKind.Leaf, new("Command.OpenLeaf"), commandService.OpenLeaf);

        ChangeParent = new(
            MaterialIconKind.SwapHorizontal,
            new("Command.ChangeParent"),
            commandService.ChangeParent
        );
        MakeAsRoot = new(
            MaterialIconKind.FamilyTree,
            new("Command.MakeAsRootToDoItem"),
            commandService.MakeAsRoot
        );

        CopyToClipboard = new(
            MaterialIconKind.Clipboard,
            new("Command.CopyToClipboard"),
            commandService.CopyToClipboard
        );

        RandomizeChildrenOrder = new(
            MaterialIconKind.DiceSix,
            new("Command.RandomizeChildrenOrder"),
            commandService.RandomizeChildrenOrder
        );

        ChangeOrder = new(
            MaterialIconKind.ReorderHorizontal,
            new("Command.Reorder"),
            commandService.ChangeOrder
        );

        Reset = new(MaterialIconKind.Refresh, new("Command.Reset"), commandService.Reset);
        Clone = new(MaterialIconKind.Copyleft, new("Command.Clone"), commandService.Clone);

        CreateReference = new(
            MaterialIconKind.LinkVariant,
            new("Command.CreateReference"),
            commandService.CreateReference
        );

        NavigateToCurrentToDoItem = new(
            MaterialIconKind.ArrowRight,
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

    public ReadOnlyMemory<SpravyCommandNotify> LeafToDoItemsMulti { get; }
    public ReadOnlyMemory<SpravyCommandNotify> RootToDoItemsMulti { get; }
    public ReadOnlyMemory<SpravyCommandNotify> TodayToDoItemsMulti { get; }
    public ReadOnlyMemory<SpravyCommandNotify> SearchToDoItemsMulti { get; }
}
