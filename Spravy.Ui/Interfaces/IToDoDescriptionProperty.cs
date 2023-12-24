namespace Spravy.Ui.Interfaces;

public interface IToDoDescriptionProperty : IRefresh, IIdProperty
{
    string Description { get; set; }
}