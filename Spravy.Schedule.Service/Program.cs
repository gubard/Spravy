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
