namespace Spravy.Ui.Features.ToDo.Settings;

public class LeafToDoItemsViewModelSetting : IViewModelSetting<LeafToDoItemsViewModelSetting>
{
    public LeafToDoItemsViewModelSetting(LeafToDoItemsViewModel viewModel)
    {
        GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
        IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
    }

    public LeafToDoItemsViewModelSetting() { }

    static LeafToDoItemsViewModelSetting()
    {
        Default = new() { GroupBy = GroupBy.ByStatus, };
    }

    public GroupBy GroupBy { get; set; }
    public bool IsMulti { get; set; }
    public static LeafToDoItemsViewModelSetting Default { get; }
}
