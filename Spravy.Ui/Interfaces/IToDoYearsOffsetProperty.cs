namespace Spravy.Ui.Interfaces;

public interface IToDoYearsOffsetProperty : IRefresh, IIdProperty
{
    ushort YearsOffset { get; set; }
}