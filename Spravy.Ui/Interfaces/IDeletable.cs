namespace Spravy.Ui.Interfaces;

public interface IDeletable
{
    Guid Id { get; }
    string Name { get; }
    Guid? ParentId { get; }
    bool IsNavigateToParent { get; }
}