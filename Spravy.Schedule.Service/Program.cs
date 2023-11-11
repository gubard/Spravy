using Spravy.Schedule.Db.Contexts;
using Spravy.Schedule.Service;
using Spravy.Schedule.Service.Extensions;
using Spravy.Schedule.Service.Services;
using Spravy.Service.Extensions;
using Spravy.Service.Middlewares;

WebApplication.CreateBuilder(args)
    .BuildSpravy<GrpcScheduleService, SpravyScheduleServiceMark>(
        args,
        typeof(DataBaseSetupSqliteMiddleware<SpravyDbScheduleDbContext>),
        x => x.AddSchedule()
    )
    .Run();