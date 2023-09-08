using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Spravy.Ui.Enums;

namespace Spravy.Ui.Converters;

public class CompleteStatusToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CompleteStatus status)
        {
            throw new ArgumentException();
        }

        return status switch
        {
            CompleteStatus.Complete => Brushes.Green,
            CompleteStatus.Incomplete => Brushes.Aqua,
            CompleteStatus.Skip => Brushes.Yellow,
            CompleteStatus.Fail => Brushes.Red,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
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

        if (brush.Equals(Brushes.Yellow))
        {
            return CompleteStatus.Skip;
        }

        if (brush.Equals(Brushes.Red))
        {
            return CompleteStatus.Fail;
        }

        throw new ArgumentOutOfRangeException();
    }
}