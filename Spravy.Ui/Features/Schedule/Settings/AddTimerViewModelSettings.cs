namespace Spravy.Ui.Features.Schedule.Settings;

public class AddTimerViewModelSettings : IViewModelSetting<AddTimerViewModelSettings>
{
    public AddTimerViewModelSettings()
    {
    }

    public AddTimerViewModelSettings(AddTimerViewModel viewModel)
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
    public static AddTimerViewModelSettings Default { get; } = new();
}