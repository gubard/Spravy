namespace Spravy.Ui.Features.ToDo.Models;

public class ToDoItemEntityNotify : NotifyBase, IEquatable<ToDoItemEntityNotify>, IParameters
{
    private static readonly ReadOnlyMemory<char> idParameterName = nameof(Id).AsMemory();
    private static readonly ReadOnlyMemory<char> nameParameterName = nameof(Name).AsMemory();

    private readonly SpravyCommandNotifyService spravyCommandNotifyService;

    public ToDoItemEntityNotify(Guid id, SpravyCommandNotifyService spravyCommandNotifyService)
    {
        this.spravyCommandNotifyService = spravyCommandNotifyService;
        Path = [RootItem.Default, this,];
        Id = id;
        Description = "Loading...";
        Name = "Loading...";
        Link = string.Empty;
        Status = ToDoItemStatus.ReadyForComplete;
        OrderIndex = uint.MaxValue;
        CompactCommands = new();
        SingleCommands = new();
        Children = new();
        MultiCommands = new();

        this.WhenAnyValue(x => x.ReferenceId)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentId)));

        this.WhenAnyValue(x => x.DescriptionType)
            .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(IsDescriptionPlainText));
                this.RaisePropertyChanged(nameof(IsDescriptionMarkdownText));
            });

        MultiCommands.AddRange(
            [
                spravyCommandNotifyService.MultiAddChildToDoItem,
                spravyCommandNotifyService.MultiShowSettingToDoItem,
                spravyCommandNotifyService.MultiDeleteToDoItem,
                spravyCommandNotifyService.MultiOpenLeafToDoItem,
                spravyCommandNotifyService.MultiChangeParentToDoItem,
                spravyCommandNotifyService.MultiMakeAsRootToDoItem,
                spravyCommandNotifyService.MultiCopyToClipboardToDoItem,
                spravyCommandNotifyService.MultiRandomizeChildrenOrderToDoItem,
                spravyCommandNotifyService.MultiChangeOrderToDoItem,
                spravyCommandNotifyService.MultiResetToDoItem,
                spravyCommandNotifyService.MultiCloneToDoItem,
                spravyCommandNotifyService.MultiOpenLinkToDoItem,
                spravyCommandNotifyService.MultiAddToFavoriteToDoItem,
                spravyCommandNotifyService.MultiRemoveFromFavoriteToDoItem,
                spravyCommandNotifyService.MultiCompleteToDoItem,
                spravyCommandNotifyService.MultiCreateReferenceToDoItem,
            ]
        );
    }

    public Guid Id { get; }

    public AvaloniaList<SpravyCommandNotify> CompactCommands { get; }
    public AvaloniaList<SpravyCommandNotify> SingleCommands { get; }
    public AvaloniaList<SpravyCommandNotify> MultiCommands { get; }
    public AvaloniaList<ToDoItemEntityNotify> Children { get; }

    [Reactive]
    public object[] Path { get; set; }

    [Reactive]
    public ToDoItemEntityNotify? Active { get; set; }

    [Reactive]
    public bool IsSelected { get; set; }

    [Reactive]
    public bool IsExpanded { get; set; }

    [Reactive]
    public bool IsIgnore { get; set; }

    [Reactive]
    public bool IsFavorite { get; set; }

    [Reactive]
    public string Description { get; set; }

    [Reactive]
    public uint OrderIndex { get; set; }

    [Reactive]
    public ToDoItemStatus Status { get; set; }

    [Reactive]
    public ToDoItemIsCan IsCan { get; set; }

    [Reactive]
    public string Name { get; set; }

    [Reactive]
    public ToDoItemEntityNotify? Parent { get; set; }

    [Reactive]
    public Guid? ReferenceId { get; set; }

    [Reactive]
    public DescriptionType DescriptionType { get; set; }

    [Reactive]
    public string Link { get; set; }

    [Reactive]
    public ToDoItemType Type { get; set; }

    public bool IsDescriptionPlainText
    {
        get => DescriptionType == DescriptionType.PlainText;
    }

    public bool IsDescriptionMarkdownText
    {
        get => DescriptionType == DescriptionType.Markdown;
    }

    public Guid CurrentId
    {
        get => ReferenceId ?? Id;
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetSelectedItems()
    {
        ReadOnlyMemory<ToDoItemEntityNotify> selected = Children.Where(x => x.IsSelected).ToArray();

        if (selected.IsEmpty)
        {
            return new(new NonItemSelectedError());
        }

        return new(selected);
    }

    public bool Equals(ToDoItemEntityNotify? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((ToDoItemEntityNotify)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public Result<string> GetParameter(ReadOnlySpan<char> parameterName)
    {
        if (idParameterName.Span.AreEquals(parameterName))
        {
            return Id.ToString().ToResult();
        }

        if (nameParameterName.Span.AreEquals(parameterName))
        {
            return Name.ToResult();
        }

        return new(new NotFoundNamedError(parameterName.ToString()));
    }

    public Result<ToDoItemEntityNotify> UpdateCommandsUi()
    {
        CompactCommands.Clear();
        SingleCommands.Clear();

        CompactCommands.AddRange(
            [
                spravyCommandNotifyService.AddChild,
                spravyCommandNotifyService.ShowSetting,
                spravyCommandNotifyService.Delete,
                spravyCommandNotifyService.OpenLeaf,
                spravyCommandNotifyService.ChangeParent,
                spravyCommandNotifyService.MakeAsRoot,
                spravyCommandNotifyService.CopyToClipboard,
                spravyCommandNotifyService.RandomizeChildrenOrder,
                spravyCommandNotifyService.ChangeOrder,
                spravyCommandNotifyService.Reset,
                spravyCommandNotifyService.Clone,
                spravyCommandNotifyService.CreateReference,
            ]
        );

        var singleCommands = new List<SpravyCommandNotify>(CompactCommands);

        if (!Link.IsNullOrWhiteSpace())
        {
            singleCommands.Add(spravyCommandNotifyService.OpenLink);
        }

        singleCommands.Add(
            IsFavorite
                ? spravyCommandNotifyService.RemoveFromFavorite
                : spravyCommandNotifyService.AddToFavorite
        );

        if (IsCan != ToDoItemIsCan.None)
        {
            singleCommands.Add(spravyCommandNotifyService.Complete);
        }

        SingleCommands.AddRange(singleCommands);

        return this.ToResult();
    }
}
