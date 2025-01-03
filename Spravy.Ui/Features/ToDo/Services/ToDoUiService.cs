namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoUiService : IToDoUiService
{
    private readonly IToDoService toDoService;

    public ToDoUiService(IToDoService toDoService, IToDoCache toDoCache)
    {
        this.toDoService = toDoService;

        Response += response => this.InvokeUiBackgroundAsync(
            () => response.ActiveItems
               .Select(x => x.Item.GetValueOrNull())
               .Where(x => x.HasValue)
               .Select(x => x!.Value)
               .ToArray()
               .ToReadOnlyMemory()
               .IfSuccessForEach(toDoCache.UpdateUi)
               .IfSuccess(_ => response.BookmarkItems.IfSuccessForEach(toDoCache.UpdateUi))
               .IfSuccess(_ => toDoCache.UpdateUi(response.SelectorItems))
               .IfSuccess(
                    _ =>
                    {
                        if (response.CurrentActive.TryGetValue(out var currentActive))
                        {
                            return toDoCache.UpdateUi(currentActive).ToResultOnly();
                        }

                        return Result.Success;
                    }
                )
               .IfSuccess(() => response.FavoriteItems.IfSuccessForEach(toDoCache.UpdateUi))
               .IfSuccess(_ => toDoCache.SetFavoriteItems(response.FavoriteItems.Select(x => x.Item.Id)))
               .IfSuccess(() => response.BookmarkItems.IfSuccessForEach(toDoCache.UpdateUi))
               .IfSuccess(_ => response.SearchItems.IfSuccessForEach(toDoCache.UpdateUi))
               .IfSuccess(_ => response.ParentItems.IfSuccessForEach(x => toDoCache.UpdateParentsUi(x.Id, x.Parents)))
               .IfSuccess(() => response.TodayItems.IfSuccessForEach(toDoCache.UpdateUi))
               .IfSuccess(_ => response.RootItems.IfSuccessForEach(toDoCache.UpdateUi))
               .IfSuccess(_ => toDoCache.UpdateRootItems(response.RootItems.Select(x => x.Item.Id)))
               .IfSuccess(
                    _ => response.ChildrenItems.IfSuccessForEach(
                        x => toDoCache.UpdateChildrenItemsUi(x.Id, x.Children.Select(y => y.Item.Id))
                           .IfSuccess(_ => x.Children.IfSuccessForEach(toDoCache.UpdateUi))
                    )
                )
               .ToResultOnly()
        );
    }

    public event Func<ToDoResponse, Cvtar>? Response;

    public ConfiguredValueTaskAwaitable<Result<ToDoResponse>> GetRequest(GetToDo get, CancellationToken ct)
    {
        return toDoService.GetAsync(get, ct)
           .IfSuccessAsync(
                response => Response is null
                    ? response.ToResult().ToValueTaskResult().ConfigureAwait(false)
                    : Response.Invoke(response).IfSuccessAsync(() => response.ToResult(), ct),
                ct
            );
    }
}