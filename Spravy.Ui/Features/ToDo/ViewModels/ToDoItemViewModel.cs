using Spravy.Picture.Domain.Enums;
using Spravy.Picture.Domain.Interfaces;
using Spravy.Picture.Domain.Models;
using Spravy.Ui.Features.Picture.Models;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemViewModel : NavigatableViewModelBase, IToDoItemEditId, IToDoSubItemsViewModelOwner
{
    private readonly IObjectStorage objectStorage;
    private readonly TaskWork refreshWork;
    private readonly IToDoUiService toDoUiService;
    private readonly IPictureService pictureService;

    [ObservableProperty]
    private bool isMulti;

    public ToDoItemViewModel(
        ToDoItemEntityNotify item,
        IObjectStorage objectStorage,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IErrorHandler errorHandler,
        IToDoUiService toDoUiService,
        IPictureService pictureService
    ) : base(true)
    {
        Images = new();
        this.objectStorage = objectStorage;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoUiService = toDoUiService;
        this.pictureService = pictureService;
        Item = item;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        Commands = new(Item.Commands);
        ToDoSubItemsViewModel.SetItemsUi(Item.Children.ToArray()).ThrowIfError();
    }

    public ToDoItemEntityNotify Item { get; }
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }
    public AvaloniaList<MemoryToDoImage> Images { get; }
    public override string ViewId => $"{TypeCache<ToDoItemViewModel>.Name}:{Item.Id}";

    public Result<ToDoItemEditId> GetToDoItemEditId()
    {
        if (!ToDoSubItemsViewModel.List.IsMulti)
        {
            return new ToDoItemEditId(Item.ToOption(), ReadOnlyMemory<ToDoItemEntityNotify>.Empty).ToResult();
        }

        return ToDoSubItemsViewModel.GetSelectedItems()
           .IfSuccess(selected => new ToDoItemEditId(Item.ToOption(), selected).ToResult());
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return RefreshCore().ConfigureAwait(false);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(IsMulti))
        {
            UpdateCommandsUi();
            ToDoSubItemsViewModel.List.IsMulti = IsMulti;
        }
    }

    private async ValueTask<Result> RefreshCore()
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private Cvtar RefreshCoreAsync(CancellationToken ct)
    {
        return this.PostUiBackground(() => ToDoSubItemsViewModel.RefreshUi(), ct)
           .IfSuccessAllAsync(
                ct,
                () => pictureService.GetPictureAsync(
                        new(
                            new IdPictureParameters[]
                            {
                                new(
                                    new EntryId[]
                                    {
                                        new(
                                            "ToDo",
                                            new[]
                                            {
                                                Item.Id,
                                            }
                                        ),
                                    },
                                    100,
                                    SizeType.Height
                                ),
                            }
                        ),
                        ct
                    )
                   .IfSuccessAsync(
                        response => this.PostUiBackground(
                            () => response.Pictures
                               .IfSuccessForEach(
                                    picture =>
                                    {
                                        using var stream = picture.Picture.Data;

                                        return new MemoryToDoImage(stream).ToResult();
                                    }
                                )
                               .IfSuccess(images => Images.UpdateUi(images)),
                            ct
                        ),
                        ct
                    ),
                () => toDoUiService.GetRequest(
                        GetToDo.WithDefaultItems
                           .SetParentItem(Item.Id)
                           .SetChildrenItem(Item.Id)
                           .SetItem(Item.Id),
                        ct
                    )
                   .IfSuccessAsync(
                        _ => this.PostUiBackground(
                            () =>
                                ToDoSubItemsViewModel.SetItemsUi(Item.Children.ToArray()).IfSuccess(UpdateCommandsUi),
                            ct
                        ),
                        ct
                    )
                   .IfSuccessAsync(
                        () =>
                        {
                            var pathIds = Item.Path
                               .OfType<ToDoItemEntityNotify>()
                               .Select(x => x.Id)
                               .ToArray()
                               .ToReadOnlyMemory();

                            var ids = Item.Children
                               .Select(x => x.Id)
                               .ToArray()
                               .ToReadOnlyMemory()
                               .Combine(pathIds);

                            return toDoUiService.GetRequest(
                                GetToDo.Default.SetItems(pathIds).SetParentItems(ids).SetChildrenItems(ids),
                                ct
                            );
                        },
                        ct
                    )
                   .ToResultOnlyAsync()
            );
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return ToDoSubItemsViewModel.Stop();
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new ToDoItemViewModelSetting(this), ct);
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage.GetObjectOrDefaultAsync<ToDoItemViewModelSetting>(ViewId, ct)
           .IfSuccessAsync(
                s => this.PostUiBackground(
                    () =>
                    {
                        ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                        IsMulti = s.IsMulti;

                        return Result.Success;
                    },
                    ct
                ),
                ct
            );
    }

    private Result UpdateCommandsUi()
    {
        return IsMulti ? Commands.UpdateUi(UiHelper.ToDoItemCommands) : Commands.UpdateUi(Item.Commands);
    }
}