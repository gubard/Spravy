namespace Spravy.Ui.Converters;

public class ObjectEqualsParameterConverter : IValueConverter
{
    public bool Reverse { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            if (parameter is null)
            {
                return !Reverse;
            }

            return Reverse;
        }

        if (parameter is null)
        {
            return Reverse;
        }

        if (Reverse)
        {
            return !value.Equals(parameter);
        }

        return value.Equals(parameter);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}