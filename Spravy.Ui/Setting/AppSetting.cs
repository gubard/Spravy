namespace Spravy.Ui.Setting;

public class AppSetting : IViewModelSetting<AppSetting>
{
    public static AppSetting Default { get; } = new();

    public string[] FavoriteIcons { get; set; } = [];
}
