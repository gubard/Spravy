Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    Log.Information("Starting web app");

    WebApplication.CreateBuilder(args)
       .BuildSpravy<GrpcUserSecretService, GrpcPasswordService, SpravyPasswordGeneratorMark>(
            args,
            typeof(DataBaseSetupSqliteMiddleware<PasswordSpravyDbContext>).ToEnumerable(),
            x => x.RegisterPasswordGenerator()
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