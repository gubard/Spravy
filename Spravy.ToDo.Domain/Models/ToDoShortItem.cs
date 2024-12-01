using Spravy.Domain.Enums;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoShortItem
{
    public ToDoShortItem(
        Guid id,
        string name,
        uint orderIndex,
        string description,
        ToDoItemType type,
        bool isBookmark,
        bool isFavorite,
        DateOnly dueDate,
        TypeOfPeriodicity typeOfPeriodicity,
        ReadOnlyMemory<DayOfYear> annuallyDays,
        ReadOnlyMemory<byte> monthlyDays,
        ReadOnlyMemory<DayOfWeek> weeklyDays,
        ushort daysOffset,
        ushort monthsOffset,
        ushort weeksOffset,
        ushort yearsOffset,
        ToDoItemChildrenType childrenType,
        bool isRequiredCompleteInDueDate,
        DescriptionType descriptionType,
        string icon,
        string color,
        OptionStruct<Guid> referenceId,
        OptionStruct<Guid> parentId,
        Option<Uri> link,
        uint remindDaysBefore
    )
    {
        Id = id;
        Name = name;
        OrderIndex = orderIndex;
        Description = description;
        Type = type;
        IsBookmark = isBookmark;
        IsFavorite = isFavorite;
        DueDate = dueDate;
        TypeOfPeriodicity = typeOfPeriodicity;
        AnnuallyDays = annuallyDays;
        MonthlyDays = monthlyDays;
        WeeklyDays = weeklyDays;
        DaysOffset = daysOffset;
        MonthsOffset = monthsOffset;
        WeeksOffset = weeksOffset;
        YearsOffset = yearsOffset;
        ChildrenType = childrenType;
        IsRequiredCompleteInDueDate = isRequiredCompleteInDueDate;
        DescriptionType = descriptionType;
        Icon = icon;
        Color = color;
        ReferenceId = referenceId;
        ParentId = parentId;
        Link = link;
        RemindDaysBefore = remindDaysBefore;
    }

    public Guid Id { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public string Description { get; }
    public ToDoItemType Type { get; }
    public bool IsBookmark { get; }
    public bool IsFavorite { get; }
    public DateOnly DueDate { get; }
    public TypeOfPeriodicity TypeOfPeriodicity { get; }
    public ReadOnlyMemory<DayOfYear> AnnuallyDays { get; }
    public ReadOnlyMemory<byte> MonthlyDays { get; }
    public ReadOnlyMemory<DayOfWeek> WeeklyDays { get; }
    public ushort DaysOffset { get; }
    public ushort MonthsOffset { get; }
    public ushort WeeksOffset { get; }
    public ushort YearsOffset { get; }
    public ToDoItemChildrenType ChildrenType { get; }
    public Option<Uri> Link { get; }
    public bool IsRequiredCompleteInDueDate { get; }
    public DescriptionType DescriptionType { get; }
    public string Icon { get; }
    public string Color { get; }
    public OptionStruct<Guid> ReferenceId { get; }
    public OptionStruct<Guid> ParentId { get; }
    public uint RemindDaysBefore { get; }
}