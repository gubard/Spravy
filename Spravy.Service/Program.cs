using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Spravy.Domain.Interfaces;
using Spravy.Db.Contexts;
using Spravy.Db.Core.Profiles;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Services;
using Spravy.Domain.Core.Profiles;
using Spravy.Service.Profiles;
using Spravy.Service.Services;
using Spravy.Service.Services.Grpcs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

builder.Services.AddScoped(
    _ => new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile<SpravyServiceProfile>();
            cfg.AddProfile<SpravyProfile>();
            cfg.AddProfile<SpravyDbProfile>();
        }
    )
);

builder.Services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<MapperConfiguration>()));
builder.Services.AddScoped<IToDoService, EntityFrameworkToDoService>();
builder.Services.AddScoped<IDbContextSetup, SqliteDbContextSetup>();

builder.Services.AddCors(
    o => o.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }
    )
);

#if DEBUG
builder.Services.AddDbContext<SpravyDbContext>(options => options.UseInMemoryDatabase("SpravyDbContext"));
#else
builder.Services.AddDbContext<SpravyDbContext>(
    (sp, options) => options.UseSqlite(sp.GetRequiredService<IConfiguration>()["Sqlite:ConnectionString"])
);
#endif

builder.Host.UseSerilog(
    (_, _, configuration) =>
    {
        configuration.WriteTo.File("/tmp/Spravy/Spravy.Service.log");
        configuration.WriteTo.Console();
    }
);

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseRouting();

app.UseGrpcWeb(
    new GrpcWebOptions
    {
        DefaultEnabled = true
    }
);

app.UseCors("AllowAll");

#if DEBUG
app.Urls.Clear();
app.Urls.Add("http://localhost:5000");
#endif

app.MapGrpcService<GrpcToDoService>().EnableGrpcWeb();
app.Run();