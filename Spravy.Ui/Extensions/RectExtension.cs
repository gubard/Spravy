using Avalonia;

namespace Spravy.Ui.Extensions;

public static class RectExtension
{
    public static bool IsTop(this Rect rect, Point point)
    {
        if (rect.Height / 2 >= point.Y)
        {
            return true;
        }

        return false;
    }
    
    public static bool IsBottom(this Rect rect, Point point)
    {
        if (rect.Height / 2 <= point.Y)
        {
            return true;
        }

        return false;
    }
}