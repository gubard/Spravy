namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemSettingsViewModel : DialogableViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly EmptyToDoItemSettings empty;
    private readonly ValueToDoItemSettingsViewModel valueSettings;
    private readonly PlannedToDoItemSettingsViewModel plannedSettings;
    private readonly PeriodicityToDoItemSettingsViewModel periodicitySettings;
    private readonly PeriodicityOffsetToDoItemSettingsViewModel periodicityOffsetSettings;
    private readonly ReferenceToDoItemSettingsViewModel referenceSettings;

    [ObservableProperty]
    private IEditToDoItems edit;

    public ToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        ToDoItemContentViewModel toDoItemContent,
        IViewFactory viewFactory,
        IToDoService toDoService
    )
    {
        Item = item;
        ToDoItemContent = toDoItemContent;
        this.toDoService = toDoService;
        ToDoItemContent.Type = Item.Type;
        ToDoItemContent.Name = Item.Name;
        ToDoItemContent.Link = Item.Link;
        ToDoItemContent.Icon = Item.Icon;
        ToDoItemContent.Color = Item.Color;
        empty = EmptyToDoItemSettings.Default;
        valueSettings = viewFactory.CreateValueToDoItemSettingsViewModel(Item);
        plannedSettings = viewFactory.CreatePlannedToDoItemSettingsViewModel(Item);
        periodicitySettings = viewFactory.CreatePeriodicityToDoItemSettingsViewModel(Item);
        periodicityOffsetSettings = viewFactory.CreatePeriodicityOffsetToDoItemSettingsViewModel(
            Item
        );
        referenceSettings = viewFactory.CreateReferenceToDoItemSettingsViewModel(Item);
        edit = CreateEdit();
        ToDoItemContent.PropertyChanged += OnPropertyChanged;
    }

    public ToDoItemEntityNotify Item { get; }
    public ToDoItemContentViewModel ToDoItemContent { get; }

    public override string ViewId
    {
        get => TypeCache<ToDoItemSettingsViewModel>.Type.Name;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return ToDoItemContent.LoadStateAsync(ct);
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return ToDoItemContent.SaveStateAsync(ct);
    }

    private IEditToDoItems CreateEdit()
    {
        return ToDoItemContent.Type switch
        {
            ToDoItemType.Value => valueSettings,
            ToDoItemType.Group => empty,
            ToDoItemType.Planned => plannedSettings,
            ToDoItemType.Periodicity => periodicitySettings,
            ToDoItemType.PeriodicityOffset => periodicityOffsetSettings,
            ToDoItemType.Circle => valueSettings,
            ToDoItemType.Step => valueSettings,
            ToDoItemType.Reference => referenceSettings,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToDoItemContent.Type))
        {
            Edit = CreateEdit();
        }
    }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.EditToDoItemsAsync(
            Edit.GetEditToDoItems()
                .SetType(new(ToDoItemContent.Type))
                .SetName(new(ToDoItemContent.Name))
                .SetLink(new(ToDoItemContent.Link.ToOptionUri()))
                .SetIcon(new(ToDoItemContent.Icon))
                .SetColor(new(ToDoItemContent.Color.ToString())),
            ct
        );
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}
