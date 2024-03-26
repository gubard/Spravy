using AutoMapper;
using Spravy.PasswordGenerator.Db.Models;
using Spravy.PasswordGenerator.Domain.Models;

namespace Spravy.PasswordGenerator.Db.Mapper.Profiles;

public class SpravyPasswordGeneratorDbProfile : Profile
{
    public SpravyPasswordGeneratorDbProfile()
    {
        CreateMap<AddPasswordOptions, PasswordItemEntity>();
        CreateMap<PasswordItemEntity, PasswordItem>();
    }
}