namespace Spravy.Ui.Converters;

public class Int32MoreThenConverter : IValueConverter
{
    public bool IsReverse { get; set; }
    public bool IsEquals { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int v)
        {
            return false;
        }

        if (parameter is not int p)
        {
            if (parameter is not string str)
            {
                return false;
            }

            if (!int.TryParse(str, out p))
            {
                return false;
            }
        }

        if (IsEquals && v == p)
        {
            return true;
        }

        if (IsReverse)
        {
            return v < p;
        }

        return v > p;
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
