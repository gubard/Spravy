namespace Spravy.Ui.Interfaces;

public interface IToDoMonthsOffsetProperty : IRefresh, IIdProperty
{
    ushort MonthsOffset { get; set; }
}