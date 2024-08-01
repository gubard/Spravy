namespace Spravy.Ui.Setting;

public class Setting
{
    public Setting() { }

    public Setting(SettingViewModel viewModel)
    {
        Theme = viewModel.SelectedTheme;
    }

    public ThemeType Theme { get; set; }
}
