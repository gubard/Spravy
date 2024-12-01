namespace Spravy.Ui.Converters;

public class EnumLocalizationValueConverter : IValueConverter
{
    public static EnumLocalizationValueConverter Default = new();

    private readonly Application application = Application.Current.ThrowIfNull();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not Enum e)
        {
            return null;
        }

        var typeName = e.GetType().Name;
        var resource = application.GetResource($"Lang.{typeName}.{e}");

        return resource ?? value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}