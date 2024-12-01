namespace Spravy.Ui.Converters;

public class ColorToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Color color)
        {
            return Brushes.Transparent;
        }

        if (color.A == 0)
        {
            return Brushes.Transparent;
        }

        if (parameter is int a)
        {
            return new SolidColorBrush(new Color((byte)a, color.R, color.G, color.B));
        }

        return new SolidColorBrush(color);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}