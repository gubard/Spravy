using Spravy.Domain.Enums;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct FullToDoItem
{
    public FullToDoItem(
        Guid id,
        string name,
        bool isFavorite,
        ToDoItemType type,
        string description,
        Option<Uri> link,
        uint orderIndex,
        ToDoItemStatus status,
        OptionStruct<ActiveToDoItem> active,
        ToDoItemIsCan isCan,
        OptionStruct<Guid> parentId,
        DescriptionType descriptionType,
        OptionStruct<Guid> referenceId,
        ReadOnlyMemory<DayOfYear> annuallyDays,
        ReadOnlyMemory<byte> monthlyDays,
        ToDoItemChildrenType childrenType,
        DateOnly dueDate,
        ushort daysOffset,
        ushort monthsOffset,
        ushort weeksOffset,
        ushort yearsOffset,
        bool isRequiredCompleteInDueDate,
        TypeOfPeriodicity typeOfPeriodicity,
        ReadOnlyMemory<DayOfWeek> weeklyDays,
        bool isBookmark,
        string icon
    )
    {
        Id = id;
        Name = name;
        IsFavorite = isFavorite;
        Type = type;
        Description = description;
        Link = link;
        OrderIndex = orderIndex;
        Status = status;
        Active = active;
        IsCan = isCan;
        ParentId = parentId;
        DescriptionType = descriptionType;
        ReferenceId = referenceId;
        AnnuallyDays = annuallyDays;
        MonthlyDays = monthlyDays;
        ChildrenType = childrenType;
        DueDate = dueDate;
        DaysOffset = daysOffset;
        MonthsOffset = monthsOffset;
        WeeksOffset = weeksOffset;
        YearsOffset = yearsOffset;
        IsRequiredCompleteInDueDate = isRequiredCompleteInDueDate;
        TypeOfPeriodicity = typeOfPeriodicity;
        WeeklyDays = weeklyDays;
        IsBookmark = isBookmark;
        Icon = icon;
    }

    public Guid Id { get; }
    public string Name { get; }
    public bool IsFavorite { get; }
    public ToDoItemType Type { get; }
    public string Description { get; }
    public Option<Uri> Link { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public OptionStruct<ActiveToDoItem> Active { get; }
    public ToDoItemIsCan IsCan { get; }
    public OptionStruct<Guid> ParentId { get; }
    public DescriptionType DescriptionType { get; }
    public OptionStruct<Guid> ReferenceId { get; }
    public ReadOnlyMemory<DayOfYear> AnnuallyDays { get; }
    public ReadOnlyMemory<byte> MonthlyDays { get; }
    public ToDoItemChildrenType ChildrenType { get; }
    public DateOnly DueDate { get; }
    public ushort DaysOffset { get; }
    public ushort MonthsOffset { get; }
    public ushort WeeksOffset { get; }
    public ushort YearsOffset { get; }
    public bool IsRequiredCompleteInDueDate { get; }
    public TypeOfPeriodicity TypeOfPeriodicity { get; }
    public ReadOnlyMemory<DayOfWeek> WeeklyDays { get; }
    public bool IsBookmark { get; }
    public string Icon { get; }
}
