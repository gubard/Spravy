using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Spravy.Core.Enums;

namespace Spravy.Converters;

public class ToDoItemStatusToBrushValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ToDoItemStatus status)
        {
            switch (status)
            {
                case ToDoItemStatus.Waiting:
                    return new SolidColorBrush(Colors.YellowGreen);
                case ToDoItemStatus.Today:
                    return new SolidColorBrush(Colors.DarkGreen);
                case ToDoItemStatus.Miss:
                    return new SolidColorBrush(Colors.DarkRed);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return new SolidColorBrush(Colors.DarkRed);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            if (brush.Color == Colors.DarkGreen)
            {
                return ToDoItemStatus.Today;
            }

            if (brush.Color == Colors.DarkRed)
            {
                return ToDoItemStatus.Miss;
            }

            if (brush.Color == Colors.YellowGreen)
            {
                return ToDoItemStatus.Waiting;
            }

            throw new ArgumentOutOfRangeException();
        }

        return ToDoItemStatus.Miss;
    }
}