using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Spravy.Core.Enums;

namespace Spravy.Ui.Converters;

public class ToDoItemStatusToBrushValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ToDoItemStatus status)
        {
            switch (status)
            {
                case ToDoItemStatus.Waiting:
                    return new SolidColorBrush(Colors.Yellow);
                case ToDoItemStatus.Today:
                    return new SolidColorBrush(Colors.CadetBlue);
                case ToDoItemStatus.Miss:
                    return new SolidColorBrush(Colors.DarkRed);
                case ToDoItemStatus.Complete:
                    return new SolidColorBrush(Colors.DarkGreen);
                case ToDoItemStatus.ReadyForComplete:
                    return new SolidColorBrush(Colors.GreenYellow);
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
            if (brush.Color == Colors.CadetBlue)
            {
                return ToDoItemStatus.Today;
            }

            if (brush.Color == Colors.DarkRed)
            {
                return ToDoItemStatus.Miss;
            }

            if (brush.Color == Colors.Yellow)
            {
                return ToDoItemStatus.Waiting;
            }

            if (brush.Color == Colors.DarkGreen)
            {
                return ToDoItemStatus.Complete;
            }

            if (brush.Color == Colors.GreenYellow)
            {
                return ToDoItemStatus.ReadyForComplete;
            }

            throw new ArgumentOutOfRangeException();
        }

        return ToDoItemStatus.Miss;
    }
}