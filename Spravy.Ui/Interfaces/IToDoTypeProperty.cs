namespace Spravy.Ui.Interfaces;

public interface IToDoTypeProperty : IRefresh, IIdProperty
{
    ToDoItemType Type { get; set; }
}
