namespace Spravy.Ui.Interfaces;

public interface IToDoChildrenTypeProperty : IRefresh, IIdProperty
{
    ToDoItemChildrenType ChildrenType { get; set; }
}