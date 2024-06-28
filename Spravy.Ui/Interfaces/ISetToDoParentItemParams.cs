namespace Spravy.Ui.Interfaces;

public interface ISetToDoParentItemParams : IIdProperty
{
    Guid? ParentId { get; }
}
