using Avalonia;

namespace Spravy.Tests.Extensions;

public static class PointExtension
{
    public static Point Center(this Point point, Rect rect)
    {
        return new Point(point.X + rect.Width / 2, point.Y + rect.Height / 2);
    }
    
}