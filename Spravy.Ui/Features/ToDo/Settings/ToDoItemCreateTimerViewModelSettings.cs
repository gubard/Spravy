namespace Spravy.Ui.Features.ToDo.Settings;

public class ToDoItemCreateTimerViewModelSettings : IViewModelSetting<ToDoItemCreateTimerViewModelSettings>
{
    public ToDoItemCreateTimerViewModelSettings()
    {
    }

    public ToDoItemCreateTimerViewModelSettings(ToDoItemCreateTimerViewModel viewModel)
    {
        Name = viewModel.Name;
        var names = viewModel.Names.ToArray(viewModel.Names.Count + 1).AsSpan();
        names[^1] = Name;
        Names.AddRange(names.DistinctIgnoreNullOrWhiteSpace());
        Times.AddRange(viewModel.Times);
    }

    public string Name { get; set; } = string.Empty;
    public List<string> Names { get; set; } = new();
    public List<TimeSpan> Times { get; set; } = new();
    public static ToDoItemCreateTimerViewModelSettings Default { get; } = new();
}