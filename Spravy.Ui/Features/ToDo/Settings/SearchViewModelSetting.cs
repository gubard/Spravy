namespace Spravy.Ui.Features.ToDo.Settings;

public class SearchViewModelSetting : IViewModelSetting<SearchViewModelSetting>
{
    public SearchViewModelSetting(SearchToDoItemsViewModel toDoItemsViewModel)
    {
        SearchText = toDoItemsViewModel.SearchText;
    }

    public SearchViewModelSetting() { }

    static SearchViewModelSetting()
    {
        Default = new();
    }

    public string SearchText { get; set; } = string.Empty;
    public static SearchViewModelSetting Default { get; }
}
