namespace Spravy.Ui.Interfaces;

public interface IToDoWeeksOffsetProperty : IRefresh, IIdProperty
{
    ushort WeeksOffset { get; set; }
}