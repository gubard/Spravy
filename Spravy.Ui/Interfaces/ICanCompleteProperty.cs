using Spravy.ToDo.Domain.Enums;

namespace Spravy.Ui.Interfaces;

public interface ICanCompleteProperty : IIdProperty
{
    ToDoItemIsCan IsCan { get; }
    bool IsBusy { get; set; }
}