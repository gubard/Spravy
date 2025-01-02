namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoUiService : IToDoUiService
{
    private readonly AppOptions appOptions;
    private readonly IToDoCache toDoCache;
    private readonly IToDoService toDoService;

    public ToDoUiService(IToDoService toDoService, IToDoCache toDoCache, AppOptions appOptions)
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        this.appOptions = appOptions;

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
                            return this.toDoCache.UpdateUi(currentActive).ToResultOnly();
                        }

                        return Result.Success;
                    }
                )
               .IfSuccess(() => response.FavoriteItems.IfSuccessForEach(toDoCache.UpdateUi))
               .IfSuccess(_ => response.BookmarkItems.IfSuccessForEach(toDoCache.UpdateUi))
               .IfSuccess(_ => response.SearchItems.IfSuccessForEach(toDoCache.UpdateUi))
               .IfSuccess(_ => response.ParentItems.IfSuccessForEach(x => toDoCache.UpdateParentsUi(x.Id, x.Parents)).IfSuccess(() => response.TodayItems.IfSuccessForEach(toDoCache.UpdateUi)).IfSuccess(_ => response.RootItems.IfSuccessForEach(toDoCache.UpdateUi)).ToResultOnly())
        );
    }

    public event Func<ToDoResponse, Cvtar>? Response;

    private Cvtar GetRequest(GetToDo get, CancellationToken ct)
    {
        return toDoService.GetAsync(get, ct)
           .IfSuccessAsync(
                response =>
                {
                    if (Response is null)
                    {
                        return Result.AwaitableSuccess;
                    }

                    return Response.Invoke(response);
                },
                ct
            );
    }

    public Cvtar UpdateItemAsync(ToDoItemEntityNotify item, CancellationToken ct)
    {
        return GetRequest(
            new(
                false,
                ReadOnlyMemory<GetToStringItem>.Empty,
                false,
                ReadOnlyMemory<Guid>.Empty,
                true,
                true,
                ReadOnlyMemory<Guid>.Empty,
                ReadOnlyMemory<Guid>.Empty,
                string.Empty,
                ReadOnlyMemory<Guid>.Empty,
                false,
                false,
                new[]
                {
                    item.Id,
                }
            ),
            ct
        );
    }

    public Cvtar UpdateSelectorItemsAsync(Guid? selectedId, ReadOnlyMemory<Guid> ignoreIds, CancellationToken ct)
    {
        return this.InvokeUiAsync(() => toDoCache.ResetItemsUi())
           .IfSuccessAsync(
                () => GetRequest(
                    new(
                        true,
                        ReadOnlyMemory<GetToStringItem>.Empty,
                        false,
                        ReadOnlyMemory<Guid>.Empty,
                        true,
                        true,
                        ReadOnlyMemory<Guid>.Empty,
                        ReadOnlyMemory<Guid>.Empty,
                        string.Empty,
                        ReadOnlyMemory<Guid>.Empty,
                        false,
                        false,
                        selectedId.HasValue ? new[]
                        {
                            selectedId.Value,
                        } : ReadOnlyMemory<Guid>.Empty
                    ),
                    ct
                ),
                ct
            )
           .IfSuccessAsync(() => this.InvokeUiAsync(() => toDoCache.IgnoreItemsUi(ignoreIds)), ct)
           .IfSuccessAsync(
                () =>
                {
                    if (selectedId is null)
                    {
                        return Result.AwaitableSuccess;
                    }

                    return this.InvokeUiAsync(() => toDoCache.ExpandItemUi(selectedId.Value));
                },
                ct
            );
    }

    public Cvtar UpdateLeafToDoItemsAsync(ToDoItemEntityNotify item, IToDoItemsView toDoItemsView, CancellationToken ct)
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => toDoItemsView.RefreshAsync(ct),
            () => GetRequest(
                new(
                    false,
                    ReadOnlyMemory<GetToStringItem>.Empty,
                    false,
                    ReadOnlyMemory<Guid>.Empty,
                    true,
                    true,
                    ReadOnlyMemory<Guid>.Empty,
                    new[]
                    {
                        item.Id,
                    },
                    string.Empty,
                    ReadOnlyMemory<Guid>.Empty,
                    false,
                    false,
                    new[]
                    {
                        item.Id,
                    }
                ),
                ct
            )
               .IfSuccessAsync(ids => ids.IfSuccessForEach(id => toDoCache.GetToDoItem(id)).IfSuccess(x => this.PostUiBackground(() => toDoItemsView.SetItemsUi(x), ct)).IfSuccess(() => ids.ToResult()), ct)
               .IfSuccessAsync(
                    ids =>
                    {
                        ushort loadedIndex = 0;

                        return toDoService.GetToDoItemsAsync(ids, appOptions.ToDoItemsChunkSize, ct)
                           .IfSuccessForEachAsync(
                                x => this.PostUiBackground(
                                    () => x.IfSuccessForEach(
                                            i => toDoCache.UpdateUi(i)
                                               .IfSuccess(
                                                    notify =>
                                                    {
                                                        notify.LoadedIndex = loadedIndex;
                                                        loadedIndex++;

                                                        return notify.ToResult();
                                                    }
                                                )
                                        )
                                       .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                    ct
                                ),
                                ct
                            );
                    },
                    ct
                )
        );
    }

    public Cvtar UpdateRootItemsAsync(IToDoItemsView toDoItemsView, CancellationToken ct)
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(ct, () => toDoItemsView.RefreshAsync(ct), () => toDoCache.GetRootItems().IfSuccess(items => this.PostUiBackground(() => toDoItemsView.SetItemsUi(items), ct)).IfSuccessAsync(() => toDoService.GetChildrenToDoItemIdsAsync(OptionStruct<Guid>.Default, ReadOnlyMemory<Guid>.Empty, ct), ct).IfSuccessAsync(ids => toDoCache.UpdateRootItems(ids).IfSuccess(items => this.PostUiBackground(() => toDoItemsView.SetItemsUi(items), ct)).IfSuccess(() => ids.ToResult()), ct).IfSuccessAsync(ids => toDoService.GetToDoItemsAsync(ids, appOptions.ToDoItemsChunkSize, ct).IfSuccessForEachAsync(x => this.PostUiBackground(() => x.IfSuccessForEach(i => toDoCache.UpdateUi(i)).IfSuccess(toDoItemsView.AddOrUpdateUi), ct), ct), ct));
    }

    public Cvtar UpdateSearchToDoItemsAsync(string searchText, IToDoItemsView toDoItemsView, CancellationToken ct)
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => toDoItemsView.RefreshAsync(ct),
            () => (searchText.IsNullOrWhiteSpace() ? ReadOnlyMemory<Guid>.Empty.ToResult().ToValueTaskResult().ConfigureAwait(false) : toDoService.SearchToDoItemIdsAsync(searchText, ct)).IfSuccessAsync(ids => ids.IfSuccessForEach(id => toDoCache.GetToDoItem(id)).IfSuccess(x => this.PostUiBackground(() => toDoItemsView.SetItemsUi(x), ct)).IfSuccess(() => ids.ToResult()), ct)
               .IfSuccessAsync(
                    ids =>
                    {
                        ushort loadedIndex = 0;

                        return toDoService.GetToDoItemsAsync(ids, appOptions.ToDoItemsChunkSize, ct)
                           .IfSuccessForEachAsync(
                                x => this.PostUiBackground(
                                    () => x.IfSuccessForEach(
                                            i => toDoCache.UpdateUi(i)
                                               .IfSuccess(
                                                    notify =>
                                                    {
                                                        notify.LoadedIndex = loadedIndex;
                                                        loadedIndex++;

                                                        return notify.ToResult();
                                                    }
                                                )
                                        )
                                       .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                    ct
                                ),
                                ct
                            );
                    },
                    ct
                )
        );
    }

    public Cvtar UpdateItemChildrenAsync(ToDoItemEntityNotify item, IToDoItemsView toDoItemsView, CancellationToken ct)
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(ct, () => this.PostUiBackground(() => toDoItemsView.SetItemsUi(item.Children.ToArray()), ct).IfSuccessAsync(() => toDoService.GetChildrenToDoItemIdsAsync(item.Id.ToOption(), ReadOnlyMemory<Guid>.Empty, ct), ct).IfSuccessAsync(ids => ids.IfSuccessForEach(id => toDoCache.GetToDoItem(id)).IfSuccess(x => this.PostUiBackground(() => toDoItemsView.SetItemsUi(x).IfSuccess(() => toDoCache.UpdateChildrenItemsUi(item.Id, ids)).ToResultOnly(), ct)).IfSuccess(() => ids.ToResult()), ct).IfSuccessAsync(ids => toDoService.GetToDoItemsAsync(ids, appOptions.ToDoItemsChunkSize, ct).IfSuccessForEachAsync(x => this.PostUiBackground(() => x.IfSuccessForEach(i => toDoCache.UpdateUi(i)).IfSuccess(toDoItemsView.AddOrUpdateUi), ct), ct), ct), () => UpdateItemAsync(item, ct), () => toDoItemsView.RefreshAsync(ct));
    }

    public Cvtar UpdateTodayItemsAsync(IToDoItemsView toDoItemsView, CancellationToken ct)
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => toDoItemsView.RefreshAsync(ct),
            () => toDoService.GetTodayToDoItemsAsync(ct)
               .IfSuccessAsync(ids => ids.IfSuccessForEach(id => toDoCache.GetToDoItem(id)).IfSuccess(x => this.PostUiBackground(() => toDoItemsView.SetItemsUi(x), ct)).IfSuccess(() => ids.ToResult()), ct)
               .IfSuccessAsync(
                    ids =>
                    {
                        ushort loadedIndex = 0;

                        return toDoService.GetToDoItemsAsync(ids, appOptions.ToDoItemsChunkSize, ct)
                           .IfSuccessForEachAsync(
                                x => this.PostUiBackground(
                                    () => x.IfSuccessForEach(
                                            i => toDoCache.UpdateUi(i)
                                               .IfSuccess(
                                                    notify =>
                                                    {
                                                        notify.LoadedIndex = loadedIndex;
                                                        loadedIndex++;

                                                        return notify.ToResult();
                                                    }
                                                )
                                        )
                                       .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                    ct
                                ),
                                ct
                            );
                    },
                    ct
                )
        );
    }
}