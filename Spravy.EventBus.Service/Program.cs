using Spravy.EventBus.Db.Contexts;
using Spravy.EventBus.Service;
using Spravy.EventBus.Service.Extensions;
using Spravy.EventBus.Service.Services;
using Spravy.Service.Extensions;
using Spravy.Service.Middlewares;

WebApplication.CreateBuilder(args)
    .BuildSpravy<GrpcEventBusService, SpravyEventBusServiceMark>(
        args,
        typeof(DataBaseSetupSqliteMiddleware<SpravyDbEventBusDbContext>),
        x => x.AddEventBus()
    )
    .Run();