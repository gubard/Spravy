namespace Spravy.Ui.Models;

public readonly struct SelectedDaysOfMonthChangedEventArgs
{
    public SelectedDaysOfMonthChangedEventArgs(byte[] newDaysOfMonth)
    {
        NewDaysOfMonth = newDaysOfMonth;
    }

    public byte[] NewDaysOfMonth { get; }
}