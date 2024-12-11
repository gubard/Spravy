namespace Spravy.Ui.Features.ToDo.Models;

public partial class ToDoItemEntityNotify : NotifyBase,
    IEquatable<ToDoItemEntityNotify>,
    IObjectParameters,
    IToDoItemEditId
{
    private static readonly ReadOnlyMemory<char> idParameterName = nameof(Id).AsMemory();
    private static readonly ReadOnlyMemory<char> nameParameterName = nameof(Name).AsMemory();

    private readonly SpravyCommandNotifyService spravyCommandNotifyService;

    [ObservableProperty]
    private ToDoItemEntityNotify? active;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    [ObservableProperty]
    private Color color;

    [ObservableProperty]
    private ushort daysOffset;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private DescriptionType descriptionType;

    [ObservableProperty]
    private DateOnly dueDate;

    [ObservableProperty]
    private string icon;

    [ObservableProperty]
    private bool isBookmark;

    [ObservableProperty]
    private ToDoItemIsCan isCan;

    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private bool isFavorite;

    [ObservableProperty]
    private bool isIgnore;

    [ObservableProperty]
    private bool isRequiredCompleteInDueDate;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private string link;

    [ObservableProperty]
    private ushort loadedIndex;

    [ObservableProperty]
    private ushort monthsOffset;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private uint orderIndex;

    [ObservableProperty]
    private ToDoItemEntityNotify? parent;

    [ObservableProperty]
    private object[] path;

    [ObservableProperty]
    private ToDoItemEntityNotify? reference;

    [ObservableProperty]
    private uint remindDaysBefore;

    [ObservableProperty]
    private ToDoItemStatus status;

    [ObservableProperty]
    private ToDoItemType type;

    [ObservableProperty]
    private TypeOfPeriodicity typeOfPeriodicity;

    [ObservableProperty]
    private ushort weeksOffset;

    [ObservableProperty]
    private ushort yearsOffset;
    
    [ObservableProperty]
    private bool isUpdated;

    public ToDoItemEntityNotify(Guid id, SpravyCommandNotifyService spravyCommandNotifyService)
    {
        this.spravyCommandNotifyService = spravyCommandNotifyService;
        path = [RootItem.Default, this,];
        Id = id;
        description = "Loading...";
        name = "Loading...";
        link = string.Empty;
        status = ToDoItemStatus.ComingSoon;
        orderIndex = uint.MaxValue;
        isCan = ToDoItemIsCan.CanComplete;
        isRequiredCompleteInDueDate = true;
        Commands = new();
        Children = new();
        WeeklyDays = new();
        MonthlyDays = new();
        AnnuallyDays = new();
        color = Colors.Transparent;
        icon = string.Empty;
        PropertyChanged += OnPropertyChanged;
    }

    public Guid Id { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }
    public AvaloniaList<ToDoItemEntityNotify> Children { get; }
    public AvaloniaList<DayOfWeek> WeeklyDays { get; }
    public AvaloniaList<int> MonthlyDays { get; }
    public AvaloniaList<DayOfYear> AnnuallyDays { get; }
    public Guid CurrentId => Reference?.Id ?? Id;

    public ToDoItemEntityNotifyIconType IconType
    {
        get
        {
            return Type switch
            {
                ToDoItemType.Value => IsCan switch
                {
                    ToDoItemIsCan.None => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Empty : ToDoItemEntityNotifyIconType.Icon,
                    ToDoItemIsCan.CanComplete => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Unchecked : ToDoItemEntityNotifyIconType.UncheckedIcon,
                    ToDoItemIsCan.CanIncomplete => ToDoItemEntityNotifyIconType.Checked,
                    _ => throw new ArgumentOutOfRangeException(),
                },
                ToDoItemType.Group => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Empty : ToDoItemEntityNotifyIconType.Icon,
                ToDoItemType.Planned => IsCan switch
                {
                    ToDoItemIsCan.None => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Empty : ToDoItemEntityNotifyIconType.Icon,
                    ToDoItemIsCan.CanComplete => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Unchecked : ToDoItemEntityNotifyIconType.UncheckedIcon,
                    ToDoItemIsCan.CanIncomplete => ToDoItemEntityNotifyIconType.Checked,
                    _ => throw new ArgumentOutOfRangeException(),
                },
                ToDoItemType.Periodicity => IsCan switch
                {
                    ToDoItemIsCan.None => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Empty : ToDoItemEntityNotifyIconType.Icon,
                    ToDoItemIsCan.CanComplete => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Unchecked : ToDoItemEntityNotifyIconType.UncheckedIcon,
                    ToDoItemIsCan.CanIncomplete => ToDoItemEntityNotifyIconType.Checked,
                    _ => throw new ArgumentOutOfRangeException(),
                },
                ToDoItemType.PeriodicityOffset => IsCan switch
                {
                    ToDoItemIsCan.None => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Empty : ToDoItemEntityNotifyIconType.Icon,
                    ToDoItemIsCan.CanComplete => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Unchecked : ToDoItemEntityNotifyIconType.UncheckedIcon,
                    ToDoItemIsCan.CanIncomplete => ToDoItemEntityNotifyIconType.Checked,
                    _ => throw new ArgumentOutOfRangeException(),
                },
                ToDoItemType.Circle => IsCan switch
                {
                    ToDoItemIsCan.None => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Empty : ToDoItemEntityNotifyIconType.Icon,
                    ToDoItemIsCan.CanComplete => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Unchecked : ToDoItemEntityNotifyIconType.UncheckedIcon,
                    ToDoItemIsCan.CanIncomplete => ToDoItemEntityNotifyIconType.Checked,
                    _ => throw new ArgumentOutOfRangeException(),
                },
                ToDoItemType.Step => IsCan switch
                {
                    ToDoItemIsCan.None => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Empty : ToDoItemEntityNotifyIconType.Icon,
                    ToDoItemIsCan.CanComplete => Icon.IsNullOrWhiteSpace() ? ToDoItemEntityNotifyIconType.Unchecked : ToDoItemEntityNotifyIconType.UncheckedIcon,
                    ToDoItemIsCan.CanIncomplete => ToDoItemEntityNotifyIconType.Checked,
                    _ => throw new ArgumentOutOfRangeException(),
                },
                ToDoItemType.Reference => Reference?.IconType ?? ToDoItemEntityNotifyIconType.Empty,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }

    public DateOnly? ActualDueDate =>
        Type switch
        {
            ToDoItemType.Value => null,
            ToDoItemType.Group => null,
            ToDoItemType.Planned => DueDate,
            ToDoItemType.Periodicity => DueDate,
            ToDoItemType.PeriodicityOffset => DueDate,
            ToDoItemType.Circle => null,
            ToDoItemType.Step => null,
            ToDoItemType.Reference => null,
            _ => throw new ArgumentOutOfRangeException(),
        };

    public bool Equals(ToDoItemEntityNotify? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id);
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

    public Result SetParameter(ReadOnlySpan<char> parameterName, ReadOnlySpan<char> parameterValue)
    {
        return new(new NotImplementedError(nameof(SetParameter)));
    }

    public Result<ToDoItemEditId> GetToDoItemEditId()
    {
        return new ToDoItemEditId(this.ToOption(), ReadOnlyMemory<ToDoItemEntityNotify>.Empty).ToResult();
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

    public Result<ToDoItemEntityNotify> UpdateCommandsUi()
    {
        Commands.Clear();

        Commands.AddRange(
            [
                spravyCommandNotifyService.AddChild,
                spravyCommandNotifyService.ShowSetting,
                spravyCommandNotifyService.Delete,
                spravyCommandNotifyService.OpenLeaf,
                spravyCommandNotifyService.ChangeParent,
                spravyCommandNotifyService.CopyToClipboard,
                spravyCommandNotifyService.RandomizeChildrenOrder,
                spravyCommandNotifyService.ChangeOrder,
                spravyCommandNotifyService.Reset,
                spravyCommandNotifyService.Clone,
                spravyCommandNotifyService.CreateReference,
                spravyCommandNotifyService.CreateTimer,
            ]
        );

        Commands.Add(
            IsBookmark ? spravyCommandNotifyService.RemoveFromBookmark : spravyCommandNotifyService.AddToBookmark
        );

        if (!Link.IsNullOrWhiteSpace())
        {
            Commands.Add(spravyCommandNotifyService.OpenLink);
        }

        Commands.Add(
            IsFavorite ? spravyCommandNotifyService.RemoveFromFavorite : spravyCommandNotifyService.AddToFavorite
        );

        if (IsCan != ToDoItemIsCan.None)
        {
            Commands.Add(spravyCommandNotifyService.Complete);
        }

        return this.ToResult();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Reference):
                OnPropertyChanged(nameof(CurrentId));

                break;
            case nameof(DueDate):
                OnPropertyChanged(nameof(ActualDueDate));

                break;
            case nameof(Type):
            case nameof(IsCan):
            case nameof(Icon):
                OnPropertyChanged(nameof(IconType));

                break;
        }
    }
}