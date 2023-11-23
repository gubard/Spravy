namespace Spravy.ToDo.Domain.Enums;

[Flags]
public enum ToDoItemIsCan : byte
{
    None = 0,
    CanComplete = 1,
    CanSkip = 2,
    CanFail = 4,
    CanIncomplete = 8,
}