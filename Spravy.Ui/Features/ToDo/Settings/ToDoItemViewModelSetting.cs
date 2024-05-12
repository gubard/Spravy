namespace Spravy.Ui.Features.ToDo.Settings;

[ProtoContract]
public class ToDoItemViewModelSetting : IViewModelSetting<ToDoItemViewModelSetting>
{
    public ToDoItemViewModelSetting(ToDoItemViewModel viewModel)
    {
        GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
        IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
    }
    
    public ToDoItemViewModelSetting()
    {
    }
    
    static ToDoItemViewModelSetting()
    {
        Default = new()
        {
            GroupBy = GroupBy.ByStatus,
        };
    }
    
    [ProtoMember(1)]
    public GroupBy GroupBy { get; set; }
    
    [ProtoMember(2)]
    public bool IsMulti { get; set; }
    
    public static ToDoItemViewModelSetting Default { get; }
}