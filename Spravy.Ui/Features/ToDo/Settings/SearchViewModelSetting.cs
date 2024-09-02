namespace Spravy.Ui.Features.ToDo.Settings;

public class SearchViewModelSetting : IViewModelSetting<SearchViewModelSetting>
{
    public SearchViewModelSetting(SearchToDoItemsViewModel viewModel)
    {
        SearchText = viewModel.SearchText;
        var searchTexts = viewModel.SearchTexts.ToArray(viewModel.SearchTexts.Count + 1).AsSpan();
        searchTexts[^1] = viewModel.SearchText;
        SearchTexts.AddRange(searchTexts.DistinctIgnoreNullOrWhiteSpace());
    }

    public SearchViewModelSetting() { }

    static SearchViewModelSetting()
    {
        Default = new();
    }

    public string SearchText { get; set; } = string.Empty;
    public List<string> SearchTexts { get; set; } = new();
    public static SearchViewModelSetting Default { get; }
}
