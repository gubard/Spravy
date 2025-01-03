namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoUiService
{
    event Func<ToDoResponse, Cvtar>? Response;
    ConfiguredValueTaskAwaitable<Result<ToDoResponse>> GetRequest(GetToDo get, CancellationToken ct);
}