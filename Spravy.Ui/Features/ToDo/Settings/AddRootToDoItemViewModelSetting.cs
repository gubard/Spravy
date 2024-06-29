namespace Spravy.Ui.Features.ToDo.Settings;

[ProtoContract]
public class AddRootToDoItemViewModelSetting : IViewModelSetting<AddRootToDoItemViewModelSetting>
{
    static AddRootToDoItemViewModelSetting()
    {
        Default = new();
    }

    public AddRootToDoItemViewModelSetting() { }

    public AddRootToDoItemViewModelSetting(AddRootToDoItemViewModel viewModel)
    {
        Name = viewModel.ToDoItemContent.Name;
        Type = viewModel.ToDoItemContent.Type;
        Link = viewModel.ToDoItemContent.Link;
        Description = viewModel.DescriptionContent.Description;
        DescriptionType = viewModel.DescriptionContent.Type;
    }

    [ProtoMember(1)]
    public string Name { get; set; } = string.Empty;

    [ProtoMember(2)]
    public ToDoItemType Type { get; set; }

    [ProtoMember(3)]
    public string Link { get; set; } = string.Empty;

    [ProtoMember(4)]
    public string Description { get; set; } = string.Empty;

    [ProtoMember(5)]
    public DescriptionType DescriptionType { get; set; }

    public static AddRootToDoItemViewModelSetting Default { get; }
}
