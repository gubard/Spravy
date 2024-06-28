namespace Spravy.Ui.Interfaces;

public interface IToDoDaysOffsetProperty : IRefresh, IIdProperty
{
    ushort DaysOffset { get; set; }
}
