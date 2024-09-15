namespace Spravy.Ui.Converters;

public class ToDoItemTypeLocalizationValueConverter : IValueConverter
{
    private readonly Application application = Application.Current.ThrowIfNull();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not ToDoItemType type)
        {
            return null;
        }

        return type switch
        {
            ToDoItemType.Value => application.GetResource("ToDoItemType.Value"),
            ToDoItemType.Group => application.GetResource("ToDoItemType.Group"),
            ToDoItemType.Planned => application.GetResource("ToDoItemType.Planned"),
            ToDoItemType.Periodicity => application.GetResource("ToDoItemType.Periodicity"),
            ToDoItemType.PeriodicityOffset => application.GetResource(
                "ToDoItemType.PeriodicityOffset"
            ),
            ToDoItemType.Circle => application.GetResource("ToDoItemType.Circle"),
            ToDoItemType.Step => application.GetResource("ToDoItemType.Step"),
            ToDoItemType.Reference => application.GetResource("ToDoItemType.Reference"),
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
        throw new NotSupportedException();
    }
}
