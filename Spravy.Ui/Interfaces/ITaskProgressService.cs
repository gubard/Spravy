namespace Spravy.Ui.Interfaces;

public interface ITaskProgressService
{
    Result<TaskProgressItem> AddItem(ushort impact, CancellationToken ct);
}