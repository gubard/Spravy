using AutoMapper;
using Spravy.Schedule.Db.Models;
using Spravy.Schedule.Domain.Models;

namespace Spravy.Schedule.Db.Mapper.Profiles;

public class SpravyScheduleDbProfile : Profile
{
    public SpravyScheduleDbProfile()
    {
        CreateMap<TimerItem, TimerEntity>();
        CreateMap<TimerEntity, TimerItem>();
    }
}