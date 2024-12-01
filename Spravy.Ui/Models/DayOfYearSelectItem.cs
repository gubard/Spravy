namespace Spravy.Ui.Models;

public partial class DayOfYearSelectItem : NotifyBase
{
    [ObservableProperty]
    private Month month;

    public DayOfYearSelectItem()
    {
        Days = new(
            Enumerable.Range(1, 31)
               .Select(
                    x => new DayOfMonthSelectItem
                    {
                        Day = (byte)x,
                    }
                )
        );
    }

    public AvaloniaList<DayOfMonthSelectItem> Days { get; }
}