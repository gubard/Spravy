using AutoMapper;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.PasswordGenerator.Db.Models;
using Spravy.PasswordGenerator.Domain.Models;

namespace Spravy.PasswordGenerator.Db.Mapper.Profiles;

public class SpravyPasswordGeneratorDbProfile : Profile
{
    public SpravyPasswordGeneratorDbProfile()
    {
        CreateMap<AddPasswordOptions, PasswordItemEntity>();
        CreateMap<PasswordItemEntity, PasswordItem>();

        CreateMap<PasswordItemEntity, GeneratePasswordOptions>()
            .ConvertUsing(
                x => new GeneratePasswordOptions(
                    $"{x.IsAvailableNumber.IfTrueElseEmpty(StringHelper.Number)}{x.IsAvailableLowerLatin.IfTrueElseEmpty(StringHelper.LowerLatin)}{x.IsAvailableUpperLatin.IfTrueElseEmpty(StringHelper.UpperLatin)}{x.IsAvailableSpecialSymbols.IfTrueElseEmpty(StringHelper.SpecialSymbols)}{x.CustomAvailableCharacters}",
                    x.Length,
                    x.Regex
                )
            );
    }
}