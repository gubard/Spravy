namespace Spravy.Ui.Setting;

public class Setting : IViewModelSetting<Setting>
{
    public static Setting Default { get; } = new();

    public Setting() { }

    public Setting(SettingViewModel viewModel)
    {
        Theme = viewModel.SelectedTheme;
    }

    public ThemeType Theme { get; set; }
}
