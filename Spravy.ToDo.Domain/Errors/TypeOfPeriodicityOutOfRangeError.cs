using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Errors;

public class TypeOfPeriodicityOutOfRangeError : ValueOutOfRangeError<TypeOfPeriodicity>
{
    public static readonly Guid MainId = new("7FA4CBA6-0D1F-4BEB-B56E-6E2E1E77E38D");

    protected TypeOfPeriodicityOutOfRangeError() : base(TypeOfPeriodicity.Daily, MainId)
    {
    }

    public TypeOfPeriodicityOutOfRangeError(TypeOfPeriodicity type) : base(type, MainId)
    {
    }
}