namespace Spravy.Ui.Interfaces;

public interface ITaskProgressService
{
    ConfiguredValueTaskAwaitable<Result<TaskProgressItem>> AddItemAsync(ushort impact);
}
