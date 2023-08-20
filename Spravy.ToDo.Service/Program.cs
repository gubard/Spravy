using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Spravy.Db.Core.Interfaces;
using Spravy.ToDo.Domain.Core.Profiles;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Db.Core.Profiles;
using Spravy.ToDo.Db.Sqlite.Services;
using Spravy.ToDo.Service.Profiles;
using Spravy.ToDo.Service.Services;
using Spravy.ToDo.Service.Services.Grpcs;
using Spravy.ToDo.Db.Contexts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

builder.Services.AddScoped(
    _ => new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile<SpravyServiceProfile>();
            cfg.AddProfile<SpravyToDoProfile>();
            cfg.AddProfile<SpravyToDoDbProfile>();
        }
    )
);

builder.Services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<MapperConfiguration>()));
builder.Services.AddScoped<IToDoService, EntityFrameworkToDoService>();
builder.Services.AddScoped<IDbContextSetup, SqliteToDoDbContextSetup>();

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

builder.Services.AddDbContext<SpravyToDoDbContext>(
    (sp, options) => options.UseSqlite(sp.GetRequiredService<IConfiguration>()["Sqlite:ConnectionString"])
);

builder.Host.UseSerilog(
    (_, _, configuration) =>
    {
        configuration.WriteTo.File("/tmp/Spravy/Spravy.ToDo.Service.log");
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