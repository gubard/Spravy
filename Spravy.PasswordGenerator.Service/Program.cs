using Serilog;
using Spravy.Domain.Extensions;
using Spravy.PasswordGenerator.Db.Contexts;
using Spravy.PasswordGenerator.Service;
using Spravy.PasswordGenerator.Service.Extensions;
using Spravy.PasswordGenerator.Service.Services;
using Spravy.Service.Extensions;
using Spravy.Service.Middlewares;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web app");

    WebApplication.CreateBuilder(args)
        .BuildSpravy<GrpcUserSecretService, GrpcPasswordService, SpravyPasswordGeneratorMark>(
            args,
            typeof(DataBaseSetupSqliteMiddleware<PasswordDbContext>).ToEnumerable(),
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