using Spravy.Domain.Enums;

namespace Spravy.Ui.Interfaces;

public interface IToDoDescriptionProperty : IRefresh, IIdProperty
{
    string Name { get; }
    string Description { get; }
    DescriptionType DescriptionType { get; }
}