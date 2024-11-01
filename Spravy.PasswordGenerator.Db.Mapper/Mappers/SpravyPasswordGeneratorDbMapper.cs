using Riok.Mapperly.Abstractions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.PasswordGenerator.Db.Models;
using Spravy.PasswordGenerator.Domain.Models;

namespace Spravy.PasswordGenerator.Db.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class SpravyPasswordGeneratorDbMapper
{
    public static PasswordItemEntity ToPasswordItemEntity(this AddPasswordOptions options)
    {
        return new()
        {
            Login = options.Login,
            CustomAvailableCharacters = options.CustomAvailableCharacters,
            IsAvailableNumber = options.IsAvailableNumber,
            IsAvailableLowerLatin = options.IsAvailableLowerLatin,
            IsAvailableSpecialSymbols = options.IsAvailableSpecialSymbols,
            IsAvailableUpperLatin = options.IsAvailableUpperLatin,
            Length = options.Length,
            Regex = options.Regex,
            Key = options.Key,
            Name = options.Name,
        };
    }

    public static partial PasswordItem ToPasswordItem(this PasswordItemEntity options);

    public static partial ReadOnlyMemory<PasswordItem> ToPasswordItem(
        this ReadOnlyMemory<PasswordItemEntity> options
    );

    public static GeneratePasswordOptions ToGeneratePasswordOptions(this PasswordItemEntity entity)
    {
        return new(
            $"{entity.IsAvailableNumber.IfTrueElseEmpty(StringHelper.Number)}{entity.IsAvailableLowerLatin.IfTrueElseEmpty(StringHelper.LowerLatin)}{entity.IsAvailableUpperLatin.IfTrueElseEmpty(StringHelper.UpperLatin)}{entity.IsAvailableSpecialSymbols.IfTrueElseEmpty(StringHelper.SpecialSymbols)}{entity.CustomAvailableCharacters}",
            entity.Length,
            entity.Regex
        );
    }
}
