namespace Spravy.Ui.Converters;

public class BooleanToCompletedStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool b)
        {
            return value;
        }

        return b ? "Completed" : "To do";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}