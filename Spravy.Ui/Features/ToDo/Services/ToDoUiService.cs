namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoUiService : IToDoUiService
{
    private readonly IToDoService toDoService;

    public ToDoUiService(IToDoService toDoService, IToDoCache toDoCache)
    {
        this.toDoService = toDoService;

        Requested += response => this.InvokeUiBackgroundAsync(
            () => response.ActiveItems
               .Select(x => x.Item.GetValueOrNull())
               .Where(x => x.HasValue)
               .Select(x => x!.Value)
               .ToArray()
               .ToReadOnlyMemory()
               .IfSuccessForEach(toDoCache.UpdateUi)
               .IfSuccess(_ => response.Items.IsResponse?response.Items.Items.IfSuccessForEach(toDoCache.UpdateUi).ToResultOnly():Result.Success)
               .IfSuccess(() => response.SelectorItems.IsResponse? toDoCache.UpdateUi(response.SelectorItems.Items).ToResultOnly():Result.Success)
               .IfSuccess(
                    () =>
                    {
                        if (response.CurrentActive.TryGetValue(out var currentActive))
                        {
                            return toDoCache.UpdateUi(currentActive).ToResultOnly();
                        }

                        return Result.Success;
                    }
                )
               .IfSuccess(() =>response.FavoriteItems.IsResponse? response.FavoriteItems.Items.IfSuccessForEach(toDoCache.UpdateUi).ToResultOnly():Result.Success)
               .IfSuccess(() => toDoCache.SetFavoriteItems(response.FavoriteItems.Items.Select(x => x.Item.Id)))
               .IfSuccess(() => response.BookmarkItems.IsResponse?response.BookmarkItems.Items.IfSuccessForEach(toDoCache.UpdateUi).ToResultOnly():Result.Success)
               .IfSuccess(() => response.SearchItems.IsResponse?response.SearchItems.Items.IfSuccessForEach(toDoCache.UpdateUi).ToResultOnly():Result.Success)
               .IfSuccess(() => response.ParentItems.IfSuccessForEach(x => toDoCache.UpdateParentsUi(x.Id, x.Parents)))
               .IfSuccess(() =>response.TodayItems.IsResponse? response.TodayItems.Items.IfSuccessForEach(toDoCache.UpdateUi).ToResultOnly():Result.Success)
               .IfSuccess(() => response.RootItems.IsResponse?response.RootItems.Items.IfSuccessForEach(toDoCache.UpdateUi).ToResultOnly():Result.Success)
               .IfSuccess(() => response.RootItems.IsResponse?toDoCache.UpdateRootItems(response.RootItems.Items.Select(x => x.Item.Id)).ToResultOnly():Result.Success)
               .IfSuccess(
                        () => response.ChildrenItems.IfSuccessForEach(
                        x => toDoCache.UpdateChildrenItemsUi(x.Id, x.Children.Select(y => y.Item.Id))
                           .IfSuccess(_ => x.Children.IfSuccessForEach(toDoCache.UpdateUi))
                    )
                )
               .ToResultOnly()
        );
    }

    public event Func<ToDoResponse, Cvtar>? Requested;

    public ConfiguredValueTaskAwaitable<Result<ToDoResponse>> GetRequest(GetToDo get, CancellationToken ct)
    {
        return toDoService.GetAsync(get, ct)
           .IfSuccessAsync(
                response => Requested is null
                    ? response.ToResult().ToValueTaskResult().ConfigureAwait(false)
                    : Requested.Invoke(response).IfSuccessAsync(() => response.ToResult(), ct),
                ct
            );
    }
}