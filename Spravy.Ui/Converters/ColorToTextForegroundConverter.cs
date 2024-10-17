namespace Spravy.Ui.Converters;

public class ColorToTextForegroundConverter : IValueConverter
{
    private static Application application;

    static ColorToTextForegroundConverter()
    {
        application = Application.Current.ThrowIfNull();
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Color color)
        {
            if (
                application.TryFindResource(
                    "TextControlForeground",
                    application.ActualThemeVariant,
                    out var resource
                )
            )
            {
                return resource;
            }

            return Brushes.Red;
        }

        if (color.A == 0)
        {
            if (
                application.TryFindResource(
                    "TextControlForeground",
                    application.ActualThemeVariant,
                    out var resource
                )
            )
            {
                return resource;
            }

            return Brushes.Red;
        }

        return new SolidColorBrush(color);
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