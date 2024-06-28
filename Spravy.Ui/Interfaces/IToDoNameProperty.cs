namespace Spravy.Ui.Interfaces;

public interface IToDoNameProperty : IRefresh, IIdProperty
{
    string Name { get; set; }
}
