namespace Spravy.Ui.Models;

[ProtoContract]
public class SettingModel
{
    [ProtoMember(1)]
    public string BaseTheme { get; set; } = string.Empty;

    [ProtoMember(2)]
    public string ColorTheme { get; set; } = string.Empty;
}
