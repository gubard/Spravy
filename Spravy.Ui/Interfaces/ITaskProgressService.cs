namespace Spravy.Ui.Interfaces;

public interface ITaskProgressService
{
    ConfiguredValueTaskAwaitable<Result<TaskProgressItem>> AddItemAsync(double impact);
    ConfiguredValueTaskAwaitable<Result> DeleteItemAsync(TaskProgressItem item);
}