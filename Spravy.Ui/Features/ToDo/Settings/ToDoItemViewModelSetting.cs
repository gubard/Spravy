namespace Spravy.Ui.Features.ToDo.Settings;

public class ToDoItemViewModelSetting : IViewModelSetting<ToDoItemViewModelSetting>
{
    static ToDoItemViewModelSetting()
    {
        Default = new()
        {
            GroupBy = GroupBy.ByStatus,
        };
    }

    public ToDoItemViewModelSetting(ToDoItemViewModel viewModel)
    {
        GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
        IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
    }

    public ToDoItemViewModelSetting()
    {
    }

    public GroupBy GroupBy { get; set; }
    public bool IsMulti { get; set; }
    public static ToDoItemViewModelSetting Default { get; }
}