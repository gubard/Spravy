namespace Spravy.Ui.Services;

public class SpravyCommandNotifyService
{
    public SpravyCommandNotifyService(SpravyCommandService commandService)
    {
        Complete = new("mdi-check", new("Lang.Complete"), commandService.Complete);
        OpenLink = new("mdi-link", new("Lang.OpenLink"), commandService.OpenLink);
        AddChild = new("mdi-plus", new("Lang.AddChildToDoItem"), commandService.AddChild);
        Delete = new("mdi-delete", new("Lang.Delete"), commandService.Delete);
        ShowSetting = new("mdi-cog", new("Lang.Setting"), commandService.ShowSetting);
        OpenLeaf = new("mdi-leaf", new("Lang.OpenLeaf"), commandService.OpenLeaf);
        CreateTimer = new("mdi-timer", new("Lang.CreateTimer"), commandService.CreateTimer);
        Reset = new("mdi-refresh", new("Lang.Reset"), commandService.Reset);
        Clone = new("mdi-copyleft", new("Lang.Clone"), commandService.Clone);

        AddToFavorite = new(
            "mdi-star-outline",
            new("Lang.AddToFavorite"),
            commandService.AddToFavorite
        );

        RemoveFromFavorite = new(
            "mdi-star",
            new("Lang.RemoveFromFavorite"),
            commandService.RemoveFromFavorite
        );

        AddToBookmark = new(
            "mdi-bookmark-outline",
            new("Lang.AddToBookmark"),
            commandService.AddToBookmark
        );

        RemoveFromBookmark = new(
            "mdi-bookmark",
            new("Lang.RemoveFromBookmark"),
            commandService.RemoveFromBookmark
        );

        ChangeParent = new(
            "mdi-swap-horizontal",
            new("Lang.ChangeParent"),
            commandService.ChangeParent
        );

        CopyToClipboard = new(
            "mdi-clipboard",
            new("Lang.CopyToClipboard"),
            commandService.ToDoItemCopyToClipboard
        );

        RandomizeChildrenOrder = new(
            "mdi-dice-6",
            new("Lang.RandomizeChildrenOrder"),
            commandService.RandomizeChildrenOrder
        );

        ChangeOrder = new(
            "mdi-reorder-horizontal",
            new("Lang.Reorder"),
            commandService.ChangeOrder
        );

        CreateReference = new(
            "mdi-link-variant",
            new("Lang.CreateReference"),
            commandService.CreateReference
        );

        NavigateToCurrentToDoItem = new(
            "mdi-arrow-right",
            new("Command.OpenCurrent"),
            commandService.NavigateToCurrentToDoItem,
            new(Key.Q, KeyModifiers.Control)
        );

        ToDoItemCommands = new[]
        {
            Clone,
            Complete,
            Delete,
            Reset,
            AddChild,
            ChangeOrder,
            ChangeParent,
            OpenLeaf,
            OpenLink,
            ShowSetting,
            AddToFavorite,
            CopyToClipboard,
            RemoveFromFavorite,
            RandomizeChildrenOrder,
            CreateReference,
        };
    }

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
    public SpravyCommandNotify CopyToClipboard { get; }
    public SpravyCommandNotify RandomizeChildrenOrder { get; }
    public SpravyCommandNotify ChangeOrder { get; }
    public SpravyCommandNotify Reset { get; }
    public SpravyCommandNotify Clone { get; }
    public SpravyCommandNotify CreateReference { get; }
    public SpravyCommandNotify AddToBookmark { get; }
    public SpravyCommandNotify RemoveFromBookmark { get; }

    public ReadOnlyMemory<SpravyCommandNotify> ToDoItemCommands { get; }
}
