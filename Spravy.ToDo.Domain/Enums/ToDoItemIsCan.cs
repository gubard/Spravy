namespace Spravy.ToDo.Domain.Enums;

[Flags]
public enum ToDoItemIsCan
{
    None = 0,
    CanComplete = 1,
    CanIncomplete = 2,
}
