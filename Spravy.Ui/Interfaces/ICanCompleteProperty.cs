namespace Spravy.Ui.Interfaces;

public interface ICanCompleteProperty : ICurrentIdProperty
{
    ToDoItemIsCan IsCan { get; }
    bool IsBusy { get; set; }
}