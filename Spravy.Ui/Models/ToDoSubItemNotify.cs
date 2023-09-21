using System;
using ReactiveUI;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.Ui.Models;

public abstract class ToDoSubItemNotify : NotifyBase, IEquatable<ToDoSubItemNotify>
{
    private Guid id;
    private string name = string.Empty;
    private ulong orderIndex;
    private ToDoItemStatus status;
    private string description = string.Empty;
    private bool isCurrent;
    private ActiveToDoItemNotify? active;
    private DateTimeOffset? lastCompleted;
    private bool isVisible = true;

    public bool IsVisible
    {
        get => isVisible;
        set => this.RaiseAndSetIfChanged(ref isVisible, value);
    }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
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

    public string Description
    {
        get => description;
        set => this.RaiseAndSetIfChanged(ref description, value);
    }

    public bool IsCurrent
    {
        get => isCurrent;
        set => this.RaiseAndSetIfChanged(ref isCurrent, value);
    }

    public ActiveToDoItemNotify? Active
    {
        get => active;
        set => this.RaiseAndSetIfChanged(ref active, value);
    }

    public DateTimeOffset? LastCompleted
    {
        get => lastCompleted;
        set => this.RaiseAndSetIfChanged(ref lastCompleted, value);
    }

    public bool Equals(ToDoSubItemNotify? other)
    {
        return other is not null && id.Equals(other.id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == GetType() && Equals((ToDoSubItemNotify)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}