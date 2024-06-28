namespace Spravy.Ui.Converters;

public class CompleteStatusToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CompleteStatus status)
        {
            throw new ArgumentException();
        }

        return status switch
        {
            CompleteStatus.Complete => Brushes.Green,
            CompleteStatus.Incomplete => Brushes.Aqua,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not IBrush brush)
        {
            throw new ArgumentException();
        }

        if (brush.Equals(Brushes.Green))
        {
            return CompleteStatus.Complete;
        }

        if (brush.Equals(Brushes.Aqua))
        {
            return CompleteStatus.Incomplete;
        }

        throw new ArgumentOutOfRangeException();
    }
}
