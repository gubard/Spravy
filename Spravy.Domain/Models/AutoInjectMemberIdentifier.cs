namespace Spravy.Domain.Models;

public readonly record struct AutoInjectMemberIdentifier(
    TypeInformation Type,
    AutoInjectMember Member
);