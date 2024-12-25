namespace Spravy.Ui.Setting;

public class AppSetting : IViewModelSetting<AppSetting>
{
    public static AppSetting Default { get; } = new();

    public AppSetting()
    {
    }

    public AppSetting(SettingViewModel viewModel)
    {
        Theme = viewModel.SelectedTheme;
        Language = viewModel.Language;
        FavoriteIcons = viewModel.FavoriteIcons;
        IsMute = viewModel.SoundSettingsNotify.IsMute;
    }

    public string[] FavoriteIcons { get; set; } = [];
    public ThemeType Theme { get; set; }
    public string Language { get; set; } = LanguageHelper.DefaultLanguage;
    public bool IsMute { get; set; }
}