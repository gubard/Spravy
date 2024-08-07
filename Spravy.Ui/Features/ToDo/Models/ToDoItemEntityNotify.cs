namespace Spravy.Ui.Features.ToDo.Models;

public partial class ToDoItemEntityNotify
    : NotifyBase,
        IEquatable<ToDoItemEntityNotify>,
        IObjectParameters
{
    private static readonly ReadOnlyMemory<char> idParameterName = nameof(Id).AsMemory();
    private static readonly ReadOnlyMemory<char> nameParameterName = nameof(Name).AsMemory();

    private readonly SpravyCommandNotifyService spravyCommandNotifyService;

    [ObservableProperty]
    private object[] path;

    [ObservableProperty]
    private ToDoItemEntityNotify? active;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private bool isIgnore;

    [ObservableProperty]
    private bool isFavorite;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private uint orderIndex;

    [ObservableProperty]
    private ToDoItemStatus status;

    [ObservableProperty]
    private ToDoItemIsCan isCan;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private ToDoItemEntityNotify? parent;

    [ObservableProperty]
    private Guid? referenceId;

    [ObservableProperty]
    private DescriptionType descriptionType;

    [ObservableProperty]
    private string link;

    [ObservableProperty]
    private ToDoItemType type;

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

        PropertyChanged += OnPropertyChanged;
    }

    public Guid Id { get; }

    public AvaloniaList<SpravyCommandNotify> CompactCommands { get; }
    public AvaloniaList<SpravyCommandNotify> SingleCommands { get; }
    public AvaloniaList<SpravyCommandNotify> MultiCommands { get; }
    public AvaloniaList<ToDoItemEntityNotify> Children { get; }

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

    public Result SetParameter(ReadOnlySpan<char> parameterName, ReadOnlySpan<char> parameterValue)
    {
        return new(new NotImplementedError(nameof(SetParameter)));
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ReferenceId))
        {
            OnPropertyChanged(nameof(CurrentId));
        }
        else if (e.PropertyName == nameof(DescriptionType))
        {
            OnPropertyChanged(nameof(IsDescriptionPlainText));
            OnPropertyChanged(nameof(IsDescriptionMarkdownText));
        }
    }
}
