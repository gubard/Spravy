namespace Spravy.Ui.Features.ToDo.Settings;

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
        DescriptionType = viewModel.DescriptionContent.DescriptionType;
    }

    public string Name { get; set; } = string.Empty;
    public ToDoItemType Type { get; set; }
    public string Link { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DescriptionType DescriptionType { get; set; }
    public static AddToDoItemViewModelSetting Default { get; }
}
