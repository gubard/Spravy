namespace Spravy.Ui.Features.Schedule.Settings;

public class AddToDoItemToFavoriteEventViewModelSettings
    : IViewModelSetting<AddToDoItemToFavoriteEventViewModelSettings>
{
    public static AddToDoItemToFavoriteEventViewModelSettings Default { get; } = new();

    public AddToDoItemToFavoriteEventViewModelSettings() { }

    public AddToDoItemToFavoriteEventViewModelSettings(
        AddToDoItemToFavoriteEventViewModel viewModel
    )
    {
        ItemId = viewModel.ToDoItemSelectorViewModel.SelectedItem?.Id ?? Guid.Empty;
    }

    public Guid ItemId { get; set; } = Guid.Empty;
}