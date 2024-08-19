namespace Spravy.Ui.Features.Schedule.ViewModels;

public class AddToDoItemToFavoriteEventViewModel : ViewModelBase, IEventViewModel
{
    private readonly ISerializer serializer;

    public AddToDoItemToFavoriteEventViewModel(
        ToDoItemSelectorViewModel toDoItemSelectorViewModel,
        ISerializer serializer
    )
    {
        ToDoItemSelectorViewModel = toDoItemSelectorViewModel;
        this.serializer = serializer;
    }

    public Guid Id => AddToDoItemToFavoriteEventOptions.EventId;
    public ToDoItemSelectorViewModel ToDoItemSelectorViewModel { get; }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<byte>>> GetContentAsync(
        CancellationToken ct
    )
    {
        return serializer.SerializeAsync(
            new AddToDoItemToFavoriteEventOptions
            {
                ToDoItemId = ToDoItemSelectorViewModel.SelectedItem.ThrowIfNull().Id
            },
            ct
        );
    }
}
