using System;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.Ui.Interfaces;

public interface ICanComplete
{
    ToDoItemIsCan IsCan { get; }
    Guid Id { get; }
}