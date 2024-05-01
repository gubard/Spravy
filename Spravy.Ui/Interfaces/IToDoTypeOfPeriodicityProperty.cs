namespace Spravy.Ui.Interfaces;

public interface IToDoTypeOfPeriodicityProperty : IRefresh, IIdProperty
{
    TypeOfPeriodicity TypeOfPeriodicity { get; set; }
}