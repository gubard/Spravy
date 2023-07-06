using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Spravy.Core.Enums;

namespace Spravy.Converters;

public class ToDoItemStatusToColorValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ToDoItemStatus status)
        {
            switch (status)
            {
                case ToDoItemStatus.Waiting:
                    return Colors.YellowGreen;
                case ToDoItemStatus.Today:
                    return Colors.DarkGreen;
                case ToDoItemStatus.Miss:
                    return Colors.DarkRed;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return Colors.DarkRed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            if (color == Colors.DarkGreen)
            {
                return ToDoItemStatus.Today;
            }

            if (color == Colors.DarkRed)
            {
                return ToDoItemStatus.Miss;
            }

            if (color == Colors.YellowGreen)
            {
                return ToDoItemStatus.Waiting;
            }

            throw new ArgumentOutOfRangeException();
        }

        return ToDoItemStatus.Miss;
    }
}