namespace Spravy.Ui.Setting;

public class WindowSetting : IViewModelSetting<WindowSetting>
{
    public int X { get; set; }
    public int Y { get; set; }
    public double Width { get; set; } = 600;
    public double Height { get; set; } = 600;
    public static WindowSetting Default { get; } = new();
}