namespace Spravy.Ui.Features.ToDo.Settings;

[ProtoContract]
public class AddToDoItemViewModelSetting : IViewModelSetting<AddToDoItemViewModelSetting>
{
    static AddToDoItemViewModelSetting()
    {
        Default = new();
    }

    public AddToDoItemViewModelSetting() { }

    public AddToDoItemViewModelSetting(AddToDoItemViewModel viewModel)
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

    public static AddToDoItemViewModelSetting Default { get; }
}
