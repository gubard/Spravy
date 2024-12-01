namespace Spravy.Ui.Features.ToDo.Settings;

public class RootToDoItemsViewModelSetting : IViewModelSetting<RootToDoItemsViewModelSetting>
{
    static RootToDoItemsViewModelSetting()
    {
        Default = new()
        {
            GroupBy = GroupBy.ByStatus,
        };
    }

    public RootToDoItemsViewModelSetting(RootToDoItemsViewModel viewModel)
    {
        GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
        IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
    }

    public RootToDoItemsViewModelSetting()
    {
    }

    public GroupBy GroupBy { get; set; }
    public bool IsMulti { get; set; }
    public static RootToDoItemsViewModelSetting Default { get; }
}