namespace Spravy.Ui.Features.ToDo.Settings;

public class EditToDoItemViewModelSettings : IViewModelSetting<EditToDoItemViewModelSettings>
{
    public static EditToDoItemViewModelSettings Default { get; } = new();

    public EditToDoItemViewModelSettings() { }

    public EditToDoItemViewModelSettings(EditToDoItemViewModel viewModel)
    {
        Name = viewModel.Name;
        Type = viewModel.Type;
        Description = viewModel.Description;
        Link = viewModel.Link;
        DescriptionType = viewModel.DescriptionType;
        DueDate = viewModel.DueDate;
        DaysOffset = viewModel.DaysOffset;
        MonthsOffset = viewModel.MonthsOffset;
        WeeksOffset = viewModel.WeeksOffset;
        YearsOffset = viewModel.YearsOffset;
        IsRequiredCompleteInDueDate = viewModel.IsRequiredCompleteInDueDate;
        TypeOfPeriodicity = viewModel.TypeOfPeriodicity;
        Icon = viewModel.Icon;
        Color = viewModel.Color.ToString();
        RemindDaysBefore = viewModel.RemindDaysBefore;
        MonthlyDays = viewModel.MonthlyDays.Select(x => (byte)x).ToArray();
        WeeklyDays = viewModel.WeeklyDays.ToArray();

        AnnuallyDays = viewModel
            .AnnuallyDays.SelectMany(x =>
                x.Days.Where(y => y.IsSelected)
                    .Select(y => new DayOfYearSettings { Day = y.Day, Month = x.Month })
            )
            .ToArray();

        if (viewModel.ToDoItemSelector.SelectedItem is not null)
        {
            ReferenceId = viewModel.ToDoItemSelector.SelectedItem.Id;
        }
    }

    public string Name { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public ToDoItemType Type { get; set; }
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public uint RemindDaysBefore { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = Colors.Transparent.ToString();
    public DescriptionType DescriptionType { get; set; }
    public string Description { get; set; } = string.Empty;
    public ToDoItemChildrenType ChildrenType { get; set; }
    public bool IsRequiredCompleteInDueDate { get; set; } = true;
    public TypeOfPeriodicity TypeOfPeriodicity { get; set; }
    public ushort DaysOffset { get; set; }
    public ushort MonthsOffset { get; set; }
    public ushort WeeksOffset { get; set; }
    public ushort YearsOffset { get; set; }
    public DayOfWeek[] WeeklyDays { get; } = [DayOfWeek.Monday];
    public byte[] MonthlyDays { get; } = [1];
    public DayOfYearSettings[] AnnuallyDays { get; } = [new() { Day = 1, Month = 1, }];
    public Guid? ReferenceId { get; set; }
}
