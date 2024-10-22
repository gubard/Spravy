using Spravy.Ui.Setting;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class MultiToDoItemSettingViewModel : DialogableViewModelBase, IApplySettings
{
    private readonly IObjectStorage objectStorage;
    private readonly ReadOnlyMemory<ToDoItemEntityNotify> items;
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private bool isName;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private bool isLink;

    [ObservableProperty]
    private string link = string.Empty;

    [ObservableProperty]
    private bool isType;

    [ObservableProperty]
    private ToDoItemType type;

    [ObservableProperty]
    private bool isDueDate;

    [ObservableProperty]
    private DateOnly dueDate = DateTime.Now.ToDateOnly();

    [ObservableProperty]
    private bool isRemindDaysBefore;

    [ObservableProperty]
    private uint remindDaysBefore;

    [ObservableProperty]
    private bool isIcon;

    [ObservableProperty]
    private string icon = string.Empty;

    [ObservableProperty]
    private bool isColor;

    [ObservableProperty]
    private string color = string.Empty;

    private bool firstIconChanged = true;

    public MultiToDoItemSettingViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        IToDoService toDoService,
        IObjectStorage objectStorage
    )
    {
        this.items = items;
        this.toDoService = toDoService;
        this.objectStorage = objectStorage;
        ToDoItemTypes = new(UiHelper.ToDoItemTypes.ToArray());
    }

    public AvaloniaList<string> FavoriteIcons { get; } = new();
    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }

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
            new AppSetting { FavoriteIcons = FavoriteIcons.ToArray(), },
            ct
        );
    }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.EditToDoItemsAsync(GetEditToDoItems(), ct);
    }

    private EditToDoItems GetEditToDoItems()
    {
        var result = new EditToDoItems().SetIds(items.Select(x => x.Id));

        if (IsName)
        {
            result = result.SetName(new(Name));
        }

        if (IsType)
        {
            result = result.SetType(new(Type));
        }

        if (IsLink)
        {
            result = result.SetLink(new(Link.ToOptionUri()));
        }

        if (IsDueDate)
        {
            result = result.SetDueDate(new(DueDate));
        }

        if (IsRemindDaysBefore)
        {
            result = result.SetRemindDaysBefore(new(RemindDaysBefore));
        }

        if (IsColor)
        {
            result = result.SetColor(new(Color));
        }

        if (IsIcon)
        {
            result = result.SetIcon(new(Icon));
        }

        return result;
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Icon))
        {
            if (firstIconChanged)
            {
                firstIconChanged = false;

                return;
            }

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
        }
    }
}
