using Spravy.Ui.Features.Schedule.Settings;

namespace Spravy.Ui.Features.Schedule.ViewModels;

public class AddToDoItemToFavoriteEventViewModel : ViewModelBase, IEventViewModel
{
    private readonly ISerializer serializer;
    private readonly IObjectStorage objectStorage;
    private readonly IToDoCache toDoCache;

    public AddToDoItemToFavoriteEventViewModel(
        ToDoItemSelectorViewModel toDoItemSelectorViewModel,
        ISerializer serializer,
        IObjectStorage objectStorage,
        IToDoCache toDoCache
    )
    {
        ToDoItemSelectorViewModel = toDoItemSelectorViewModel;
        this.serializer = serializer;
        this.objectStorage = objectStorage;
        this.toDoCache = toDoCache;
    }

    public string ViewId => TypeCache<AddToDoItemToFavoriteEventViewModel>.Type.ToString();
    public Guid Id => AddToDoItemToFavoriteEventOptions.EventId;
    public ToDoItemSelectorViewModel ToDoItemSelectorViewModel { get; }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<byte>>> GetContentAsync(
        CancellationToken ct
    )
    {
        return ToDoItemSelectorViewModel
            .GetSelectedItem()
            .IfSuccessAsync(
                selectedItem =>
                    serializer.SerializeAsync(
                        new AddToDoItemToFavoriteEventOptions { ToDoItemId = selectedItem.Id, },
                        ct
                    ),
                ct
            );
    }

    public Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<AddToDoItemToFavoriteEventViewModelSettings>(ViewId, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
                        () =>
                        {
                            ToDoItemSelectorViewModel.SelectedItem = toDoCache
                                .GetToDoItem(setting.ItemId)
                                .GetValueOrDefault();

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
    }

    public Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(
            ViewId,
            new AddToDoItemToFavoriteEventViewModelSettings(this),
            ct
        );
    }
}
