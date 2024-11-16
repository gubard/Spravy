using Spravy.Ui.Setting;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class EditToDoItemViewModel : DialogableViewModelBase
{
    private readonly IObjectStorage objectStorage;
    private readonly IToDoCache toDoCache;

    [ObservableProperty]
    private bool isEditName;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private bool isEditLink;

    [ObservableProperty]
    private string link = string.Empty;

    [ObservableProperty]
    private bool isEditType;

    [ObservableProperty]
    private ToDoItemType type;

    [ObservableProperty]
    private bool isEditDueDate;

    [ObservableProperty]
    private DateOnly dueDate = DateTime.Now.ToDateOnly();

    [ObservableProperty]
    private bool isEditRemindDaysBefore;

    [ObservableProperty]
    private uint remindDaysBefore;

    [ObservableProperty]
    private bool isEditIcon;

    [ObservableProperty]
    private string icon = string.Empty;

    [ObservableProperty]
    private bool isEditColor;

    [ObservableProperty]
    private Color color = Colors.Transparent;

    [ObservableProperty]
    private bool isEditDescriptionType;

    [ObservableProperty]
    private DescriptionType descriptionType;

    [ObservableProperty]
    private bool isEditDescription;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private bool isEditChildrenType;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    [ObservableProperty]
    private bool isEditIsRequiredCompleteInDueDate;

    [ObservableProperty]
    private bool isRequiredCompleteInDueDate = true;

    [ObservableProperty]
    private bool isEditTypeOfPeriodicity;

    [ObservableProperty]
    private TypeOfPeriodicity typeOfPeriodicity;

    [ObservableProperty]
    private bool isEditDaysOffset;

    [ObservableProperty]
    private ushort daysOffset;

    [ObservableProperty]
    private bool isEditMonthsOffset;

    [ObservableProperty]
    private ushort monthsOffset;

    [ObservableProperty]
    private bool isEditWeeksOffset;

    [ObservableProperty]
    private ushort weeksOffset;

    [ObservableProperty]
    private bool isEditYearsOffset;

    [ObservableProperty]
    private ushort yearsOffset;

    [ObservableProperty]
    private bool isEditReference;

    [ObservableProperty]
    private bool isEditAnnuallyDays;

    [ObservableProperty]
    private bool isEditMonthlyDays;

    [ObservableProperty]
    private bool isEditWeeklyDays;

    public EditToDoItemViewModel(
        IObjectStorage objectStorage,
        ToDoItemSelectorViewModel toDoItemSelector,
        IToDoCache toDoCache,
        bool isEditShow,
        bool isEditDescriptionShow
    )
    {
        this.objectStorage = objectStorage;
        ToDoItemSelector = toDoItemSelector;
        this.toDoCache = toDoCache;
        IsEditShow = isEditShow;
        IsEditDescriptionShow = isEditDescriptionShow;
        ToDoItemTypes = new(UiHelper.ToDoItemTypes.ToArray());
        WeeklyDays = new();
        MonthlyDays = new();
        FavoriteIcons = new();

        AnnuallyDays = new(
            Enumerable.Range(1, 12).Select(x => new DayOfYearSelectItem { Month = (byte)x })
        );

        PropertyChanged += OnPropertyChanged;
        WeeklyDays.CollectionChanged += (_, _) => IsEditWeeklyDays = true;
        MonthlyDays.CollectionChanged += (_, _) => IsEditMonthlyDays = true;

        ToDoItemSelector.PropertyChanged += (_, e) =>
        {
            if (ToDoItemSelector.IsBusy)
            {
                return;
            }

            IsEditReference = e.PropertyName switch
            {
                nameof(ToDoItemSelector.SelectedItem) => true,
                _ => IsEditReference
            };
        };

        foreach (var annuallyDay in AnnuallyDays)
        {
            foreach (var day in annuallyDay.Days)
            {
                day.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(DayOfMonthSelectItem.IsSelected))
                    {
                        IsEditAnnuallyDays = true;
                    }
                };
            }
        }
    }

    public bool IsEditShow { get; }
    public bool IsEditDescriptionShow { get; }
    public AvaloniaList<string> FavoriteIcons { get; }
    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }
    public AvaloniaList<DayOfWeek> WeeklyDays { get; }
    public AvaloniaList<int> MonthlyDays { get; }
    public AvaloniaList<DayOfYearSelectItem> AnnuallyDays { get; }
    public ToDoItemSelectorViewModel ToDoItemSelector { get; }

    public override string ViewId
    {
        get => $"{TypeCache<ToDoItemToStringSettingsViewModel>.Type}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<AppSetting>(App.ViewId, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
                        () =>
                        {
                            FavoriteIcons.UpdateUi(setting.FavoriteIcons);

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(
            App.ViewId,
            new AppSetting { FavoriteIcons = FavoriteIcons.ToArray() },
            ct
        );
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public AddToDoItemOptions GetAddToDoItemOptions(OptionStruct<Guid> parentId)
    {
        return new(
            Name,
            Description,
            Type,
            false,
            false,
            DueDate,
            TypeOfPeriodicity,
            AnnuallyDays
                .SelectMany(x =>
                    x.Days.Where(y => y.IsSelected).Select(y => new DayOfYear(y.Day, x.Month))
                )
                .ToArray(),
            MonthlyDays.Select(x => (byte)x).ToArray(),
            WeeklyDays.ToArray(),
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            ChildrenType,
            IsRequiredCompleteInDueDate,
            DescriptionType,
            Icon,
            Color.ToString(),
            (ToDoItemSelector.SelectedItem?.Id).ToOptionGuid(),
            parentId,
            Link.ToOptionUri(),
            RemindDaysBefore
        );
    }

    public EditToDoItems GetEditToDoItems()
    {
        var result = new EditToDoItems();

        if (IsEditName)
        {
            result = result.SetName(new(Name));
        }

        if (IsEditType)
        {
            result = result.SetType(new(Type));
        }

        if (IsEditLink)
        {
            result = result.SetLink(new(Link.ToOptionUri()));
        }

        if (IsEditDueDate)
        {
            result = result.SetDueDate(new(DueDate));
        }

        if (IsEditRemindDaysBefore)
        {
            result = result.SetRemindDaysBefore(new(RemindDaysBefore));
        }

        if (IsEditColor)
        {
            result = result.SetColor(new(Color.ToString()));
        }

        if (IsEditIcon)
        {
            result = result.SetIcon(new(Icon));
        }

        if (IsEditDescription)
        {
            result = result.SetDescription(new(Description));
        }

        if (IsEditDescriptionType)
        {
            result = result.SetDescriptionType(new(DescriptionType));
        }

        if (IsEditLink)
        {
            result = result.SetLink(new(Link.ToOptionUri()));
        }

        if (IsEditReference)
        {
            result = result.SetReferenceId(new((ToDoItemSelector.SelectedItem?.Id).ToOptionGuid()));
        }

        if (IsEditMonthlyDays)
        {
            result = result.SetMonthlyDays(new(MonthlyDays.Select(x => (byte)x).ToArray()));
        }

        if (IsEditAnnuallyDays)
        {
            result = result.SetAnnuallyDays(
                new(
                    AnnuallyDays
                        .SelectMany(x =>
                            x.Days.Where(y => y.IsSelected)
                                .Select(y => new DayOfYear(y.Day, x.Month))
                        )
                        .ToArray()
                )
            );
        }

        if (IsEditChildrenType)
        {
            result = result.SetChildrenType(new(ChildrenType));
        }

        if (IsEditDaysOffset)
        {
            result = result.SetDaysOffset(new(DaysOffset));
        }

        if (IsEditMonthsOffset)
        {
            result = result.SetMonthsOffset(new(MonthsOffset));
        }

        if (IsEditWeeksOffset)
        {
            result = result.SetWeeksOffset(new(WeeksOffset));
        }

        if (IsEditYearsOffset)
        {
            result = result.SetYearsOffset(new(YearsOffset));
        }

        if (IsEditIsRequiredCompleteInDueDate)
        {
            result = result.SetIsRequiredCompleteInDueDate(new(IsRequiredCompleteInDueDate));
        }

        if (IsEditTypeOfPeriodicity)
        {
            result = result.SetTypeOfPeriodicity(new(TypeOfPeriodicity));
        }

        if (IsEditWeeklyDays)
        {
            result = result.SetWeeklyDays(new(WeeklyDays.ToArray()));
        }

        return result;
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Icon):
            {
                IsEditIcon = true;

                if (Icon.IsNullOrWhiteSpace())
                {
                    return;
                }

                if (FavoriteIcons.Contains(Icon))
                {
                    return;
                }

                FavoriteIcons.Insert(0, Icon);

                if (FavoriteIcons.Count > 5)
                {
                    FavoriteIcons.RemoveAt(5);
                }

                break;
            }
            case nameof(Name):
            {
                IsEditName = true;

                break;
            }
            case nameof(Type):
            {
                IsEditType = true;

                break;
            }
            case nameof(Description):
            {
                IsEditDescription = true;

                break;
            }
            case nameof(Link):
            {
                IsEditLink = true;

                break;
            }
            case nameof(DescriptionType):
            {
                IsEditDescriptionType = true;

                break;
            }
            case nameof(DueDate):
            {
                IsEditDueDate = true;

                break;
            }
            case nameof(DaysOffset):
            {
                IsEditDaysOffset = true;

                break;
            }
            case nameof(MonthsOffset):
            {
                IsEditMonthsOffset = true;

                break;
            }
            case nameof(WeeksOffset):
            {
                IsEditWeeksOffset = true;

                break;
            }
            case nameof(YearsOffset):
            {
                IsEditYearsOffset = true;

                break;
            }
            case nameof(IsRequiredCompleteInDueDate):
            {
                IsEditIsRequiredCompleteInDueDate = true;

                break;
            }
            case nameof(TypeOfPeriodicity):
            {
                IsEditTypeOfPeriodicity = true;

                break;
            }
            case nameof(Color):
            {
                IsEditColor = true;

                break;
            }
            case nameof(RemindDaysBefore):
            {
                IsEditRemindDaysBefore = true;

                break;
            }
        }
    }

    public Result SetItem(EditToDoItemViewModelSettings settings)
    {
        Name = settings.Name;
        Type = settings.Type;
        Description = settings.Description;
        Link = settings.Link;
        DescriptionType = settings.DescriptionType;
        DueDate = settings.DueDate;
        DaysOffset = settings.DaysOffset;
        MonthsOffset = settings.MonthsOffset;
        WeeksOffset = settings.WeeksOffset;
        YearsOffset = settings.YearsOffset;
        IsRequiredCompleteInDueDate = settings.IsRequiredCompleteInDueDate;
        TypeOfPeriodicity = settings.TypeOfPeriodicity;
        Icon = settings.Icon;
        Color = Color.Parse(settings.Color);
        RemindDaysBefore = settings.RemindDaysBefore;
        MonthlyDays.Clear();
        MonthlyDays.AddRange(settings.MonthlyDays.Select(x => (int)x));
        WeeklyDays.Clear();
        WeeklyDays.AddRange(settings.WeeklyDays);

        foreach (var daysOfYear in AnnuallyDays)
        {
            foreach (var day in daysOfYear.Days)
            {
                day.IsSelected = false;
            }
        }

        var months = settings.AnnuallyDays.Select(x => x.Month).Distinct().ToArray();

        foreach (var daysOfYear in AnnuallyDays)
        {
            if (months.Contains(daysOfYear.Month))
            {
                var days = settings
                    .AnnuallyDays.Where(x => x.Month == daysOfYear.Month)
                    .Select(x => x.Day)
                    .ToArray();

                foreach (var day in daysOfYear.Days.Where(x => days.Contains(x.Day)))
                {
                    day.IsSelected = true;
                }
            }
        }

        if (settings.ReferenceId is not null)
        {
            return toDoCache
                .GetToDoItem(settings.ReferenceId.Value)
                .IfSuccess(x =>
                {
                    ToDoItemSelector.SelectedItem = x;

                    return Result.Success;
                });
        }

        return Result.Success;
    }

    public Result SetItem(ToDoItemEntityNotify notify)
    {
        Name = notify.Name;
        Type = notify.Type;
        Description = notify.Description;
        Link = notify.Link;
        DescriptionType = notify.DescriptionType;
        DueDate = notify.DueDate;
        DaysOffset = notify.DaysOffset;
        MonthsOffset = notify.MonthsOffset;
        WeeksOffset = notify.WeeksOffset;
        YearsOffset = notify.YearsOffset;
        IsRequiredCompleteInDueDate = notify.IsRequiredCompleteInDueDate;
        TypeOfPeriodicity = notify.TypeOfPeriodicity;
        Icon = notify.Icon;
        Color = notify.Color;
        RemindDaysBefore = notify.RemindDaysBefore;
        MonthlyDays.Clear();
        MonthlyDays.AddRange(notify.MonthlyDays);
        WeeklyDays.Clear();
        WeeklyDays.AddRange(notify.WeeklyDays);

        foreach (var daysOfYear in AnnuallyDays)
        {
            foreach (var day in daysOfYear.Days)
            {
                day.IsSelected = false;
            }
        }

        var months = notify.AnnuallyDays.Select(x => x.Month).Distinct().ToArray();

        foreach (var daysOfYear in AnnuallyDays)
        {
            if (months.Contains(daysOfYear.Month))
            {
                var days = notify
                    .AnnuallyDays.Where(x => x.Month == daysOfYear.Month)
                    .Select(x => x.Day)
                    .ToArray();

                foreach (var day in daysOfYear.Days.Where(x => days.Contains(x.Day)))
                {
                    day.IsSelected = true;
                }
            }
        }

        if (notify.Reference is not null)
        {
            ToDoItemSelector.SelectedItem = notify.Reference;
        }

        return Result.Success;
    }

    public Result UndoAllUi()
    {
        IsEditName = false;
        IsEditType = false;
        IsEditLink = false;
        IsEditDueDate = false;
        IsEditRemindDaysBefore = false;
        IsEditColor = false;
        IsEditIcon = false;
        IsEditDescription = false;
        IsEditDescriptionType = false;
        IsEditLink = false;
        IsEditReference = false;
        IsEditMonthlyDays = false;
        IsEditAnnuallyDays = false;
        IsEditChildrenType = false;
        IsEditDaysOffset = false;
        IsEditMonthsOffset = false;
        IsEditWeeksOffset = false;
        IsEditYearsOffset = false;
        IsEditIsRequiredCompleteInDueDate = false;
        IsEditTypeOfPeriodicity = false;
        IsEditWeeklyDays = false;

        return Result.Success;
    }
}