namespace Spravy.Ui.Models;

public class AppOptions
{
    public byte ToDoItemsChunkSize { get; set; } = 10;
}

public class AppOptionsConfiguration
{
    public AppOptions? AppOptions { get; set; } = new();
}