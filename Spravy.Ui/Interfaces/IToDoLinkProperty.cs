namespace Spravy.Ui.Interfaces;

public interface IToDoLinkProperty : IRefresh, IIdProperty
{
    string Link { get; set; }
}