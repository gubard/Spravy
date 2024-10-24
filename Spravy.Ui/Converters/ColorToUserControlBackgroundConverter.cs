namespace Spravy.Ui.Converters;

public class ColorToUserControlBackgroundConverter : IValueConverter
{
    private static Application application;

    static ColorToUserControlBackgroundConverter()
    {
        application = Application.Current.ThrowIfNull();
    }

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

        return new SolidColorBrush(new Color(51, color.R, color.G, color.B));
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException();
    }
}
