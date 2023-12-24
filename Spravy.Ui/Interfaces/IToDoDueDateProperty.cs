using System;

namespace Spravy.Ui.Interfaces;

public interface IToDoDueDateProperty : IIdProperty, IRefresh
{
    DateOnly DueDate { get; set; }
}