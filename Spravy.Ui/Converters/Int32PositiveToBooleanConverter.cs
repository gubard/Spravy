namespace Spravy.Ui.Converters;

public class Int32PositiveToBooleanConverter : IValueConverter
{
    public bool IsReverse { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int item)
        {
            return false;
        }

        if (IsReverse)
        {
            return item <= 0;
        }

        return item > 0;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException();
    }
}
