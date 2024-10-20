using Spravy.Domain.Enums;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct EditToDoItems
{
    public EditToDoItems(
        ReadOnlyMemory<Guid> ids,
        EditPropertyValue<string> name,
        EditPropertyValue<bool> isFavorite,
        EditPropertyValue<ToDoItemType> type,
        EditPropertyValue<string> description,
        EditPropertyValue<Option<Uri>> link,
        EditPropertyValue<OptionStruct<Guid>> parentId,
        EditPropertyValue<DescriptionType> descriptionType,
        EditPropertyValue<OptionStruct<Guid>> referenceId,
        EditPropertyValue<ReadOnlyMemory<DayOfYear>> annuallyDays,
        EditPropertyValue<ReadOnlyMemory<byte>> monthlyDays,
        EditPropertyValue<ToDoItemChildrenType> childrenType,
        EditPropertyValue<DateOnly> dueDate,
        EditPropertyValue<ushort> daysOffset,
        EditPropertyValue<ushort> monthsOffset,
        EditPropertyValue<ushort> weeksOffset,
        EditPropertyValue<ushort> yearsOffset,
        EditPropertyValue<bool> isRequiredCompleteInDueDate,
        EditPropertyValue<TypeOfPeriodicity> typeOfPeriodicity,
        EditPropertyValue<ReadOnlyMemory<DayOfWeek>> weeklyDays,
        EditPropertyValue<bool> isBookmark,
        EditPropertyValue<string> icon,
        EditPropertyValue<string> color,
        EditPropertyValue<uint> remindDaysBefore
    )
    {
        Ids = ids;
        Name = name;
        IsFavorite = isFavorite;
        Type = type;
        Description = description;
        Link = link;
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
        Color = color;
        RemindDaysBefore = remindDaysBefore;
    }

    public ReadOnlyMemory<Guid> Ids { get; }
    public EditPropertyValue<string> Name { get; }
    public EditPropertyValue<bool> IsFavorite { get; }
    public EditPropertyValue<ToDoItemType> Type { get; }
    public EditPropertyValue<string> Description { get; }
    public EditPropertyValue<Option<Uri>> Link { get; }
    public EditPropertyValue<OptionStruct<Guid>> ParentId { get; }
    public EditPropertyValue<DescriptionType> DescriptionType { get; }
    public EditPropertyValue<OptionStruct<Guid>> ReferenceId { get; }
    public EditPropertyValue<ReadOnlyMemory<DayOfYear>> AnnuallyDays { get; }
    public EditPropertyValue<ReadOnlyMemory<byte>> MonthlyDays { get; }
    public EditPropertyValue<ToDoItemChildrenType> ChildrenType { get; }
    public EditPropertyValue<DateOnly> DueDate { get; }
    public EditPropertyValue<ushort> DaysOffset { get; }
    public EditPropertyValue<ushort> MonthsOffset { get; }
    public EditPropertyValue<ushort> WeeksOffset { get; }
    public EditPropertyValue<ushort> YearsOffset { get; }
    public EditPropertyValue<bool> IsRequiredCompleteInDueDate { get; }
    public EditPropertyValue<TypeOfPeriodicity> TypeOfPeriodicity { get; }
    public EditPropertyValue<ReadOnlyMemory<DayOfWeek>> WeeklyDays { get; }
    public EditPropertyValue<bool> IsBookmark { get; }
    public EditPropertyValue<string> Icon { get; }
    public EditPropertyValue<string> Color { get; }
    public EditPropertyValue<uint> RemindDaysBefore { get; }

    public EditToDoItems SetIsRequiredCompleteInDueDate(EditPropertyValue<bool> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            value,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetChildrenType(EditPropertyValue<ToDoItemChildrenType> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            value,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetDueDate(EditPropertyValue<DateOnly> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            value,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetTypeOfPeriodicity(EditPropertyValue<TypeOfPeriodicity> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            value,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetIcon(EditPropertyValue<string> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            value,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetColor(EditPropertyValue<string> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            value,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetIds(ReadOnlyMemory<Guid> value)
    {
        return new(
            value,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetDaysOffset(EditPropertyValue<ushort> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            value,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetMonthsOffset(EditPropertyValue<ushort> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            value,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetWeeksOffset(EditPropertyValue<ushort> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            value,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetYearsOffset(EditPropertyValue<ushort> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            value,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetReferenceId(EditPropertyValue<OptionStruct<Guid>> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            value,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetMonthlyDays(EditPropertyValue<ReadOnlyMemory<byte>> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            value,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetWeeklyDays(EditPropertyValue<ReadOnlyMemory<DayOfWeek>> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            value,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetAnnuallyDays(EditPropertyValue<ReadOnlyMemory<DayOfYear>> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            value,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetType(EditPropertyValue<ToDoItemType> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            value,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetName(EditPropertyValue<string> value)
    {
        return new(
            Ids,
            value,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetLink(EditPropertyValue<Option<Uri>> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            value,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetParentId(EditPropertyValue<OptionStruct<Guid>> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            value,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetIsFavorite(EditPropertyValue<bool> value)
    {
        return new(
            Ids,
            Name,
            value,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetDescriptionType(EditPropertyValue<DescriptionType> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            value,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetDescription(EditPropertyValue<string> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            value,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetIsBookmark(EditPropertyValue<bool> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            value,
            Icon,
            Color,
            RemindDaysBefore
        );
    }

    public EditToDoItems SetRemindDaysBefore(EditPropertyValue<uint> value)
    {
        return new(
            Ids,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            ParentId,
            DescriptionType,
            ReferenceId,
            AnnuallyDays,
            MonthlyDays,
            ChildrenType,
            DueDate,
            DaysOffset,
            MonthsOffset,
            WeeksOffset,
            YearsOffset,
            IsRequiredCompleteInDueDate,
            TypeOfPeriodicity,
            WeeklyDays,
            IsBookmark,
            Icon,
            Color,
            value
        );
    }
}
