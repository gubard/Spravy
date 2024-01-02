using System;

namespace Spravy.Ui.Interfaces;

public interface IDeletable
{
    Guid Id { get; }
    object? Header { get; }
    Guid? ParentId { get; }
    bool IsNavigateToParent { get; }
}