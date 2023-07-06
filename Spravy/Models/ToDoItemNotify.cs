using System;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Core.Enums;

namespace Spravy.Models;

public class ToDoItemNotify : NotifyBase, IEquatable<ToDoItemNotify>
{
    private string name = string.Empty;
    private Guid id;
    private ulong orderIndex;
    private DateTimeOffset? dueDate;
    private bool isComplete;
    private ToDoItemStatus status;

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public DateTimeOffset? DueDate
    {
        get => dueDate;
        set => this.RaiseAndSetIfChanged(ref dueDate, value);
    }

    public bool IsComplete
    {
        get => isComplete;
        set => this.RaiseAndSetIfChanged(ref isComplete, value);
    }

    public ulong OrderIndex
    {
        get => orderIndex;
        set => this.RaiseAndSetIfChanged(ref orderIndex, value);
    }

    public ToDoItemStatus Status
    {
        get => status;
        set => this.RaiseAndSetIfChanged(ref status, value);
    }

    public bool Equals(ToDoItemNotify? other)
    {
        if (other is null)
        {
            return false;
        }

        return id.Equals(other.id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;
        return Equals((ToDoItemNotify)obj);
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}