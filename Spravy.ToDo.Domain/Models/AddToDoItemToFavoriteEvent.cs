namespace Spravy.ToDo.Domain.Models;

public readonly struct AddToDoItemToFavoriteEvent
{
    public static readonly Guid EventId = Guid.Parse("7511B806-764B-4BB1-873A-BE2D36A457F6");

    public AddToDoItemToFavoriteEvent(Guid toDoItemId)
    {
        ToDoItemId = toDoItemId;
    }

    public Guid ToDoItemId { get; }
}
