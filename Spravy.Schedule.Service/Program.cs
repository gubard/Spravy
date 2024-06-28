using Serilog;
using Spravy.Schedule.Db.Contexts;
using Spravy.Schedule.Service;
using Spravy.Schedule.Service.Extensions;
using Spravy.Schedule.Service.Services;
using Spravy.Service.Extensions;
using Spravy.Service.Middlewares;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    Log.Information("Starting web app");

    WebApplication
        .CreateBuilder(args)
        .BuildSpravy<GrpcScheduleService, SpravyScheduleServiceMark>(
            args,
            typeof(DataBaseSetupSqliteMiddleware<SpravyDbScheduleDbContext>),
            x => x.RegisterSchedule()
        )
        .Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
