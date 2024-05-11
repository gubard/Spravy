namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface ICachedToDoViewModel
{
    IToDoCache ToDoCache { get; }
    IToDoService ToDoService { get; }
}