using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Spravy.Domain.Enums;

namespace Spravy.Ui.Converters;

public class ToDoItemStatusToBrushValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ToDoItemStatus status)
        {
            return new SolidColorBrush(Colors.DarkRed);
        }

        switch (status)
        {
            case ToDoItemStatus.Miss:
                return new SolidColorBrush(Colors.DarkRed);
            case ToDoItemStatus.Completed:
                return new SolidColorBrush(Colors.DarkGreen);
            case ToDoItemStatus.ReadyForComplete:
                return new SolidColorBrush(Colors.GreenYellow);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush brush)
        {
            return ToDoItemStatus.Miss;
        }

        if (brush.Color == Colors.DarkRed)
        {
            return ToDoItemStatus.Miss;
        }

        if (brush.Color == Colors.DarkGreen)
        {
            return ToDoItemStatus.Completed;
        }

        if (brush.Color == Colors.GreenYellow)
        {
            return ToDoItemStatus.ReadyForComplete;
        }

        throw new ArgumentOutOfRangeException();
    }
}