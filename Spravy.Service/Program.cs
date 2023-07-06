using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spravy.Core.Interfaces;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Services;
using Spravy.Service.Profiles;
using Spravy.Service.Services;
using Spravy.Service.Services.Grpcs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddScoped(_ => new MapperConfiguration(cfg => cfg.AddProfile<SpravyServiceProfile>()));
builder.Services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<MapperConfiguration>()));
builder.Services.AddScoped<IToDoService, EntityFrameworkToDoService>();
builder.Services.AddScoped<IDbContextSetup, SqliteDbContextSetup>();

#if DEBUG
builder.Services.AddDbContext<SpravyDbContext>(options => options.UseInMemoryDatabase("SpravyDbContext"));
#else 
builder.Services.AddDbContext<SpravyDbContext>(
    (sp, options) => options.UseSqlite(sp.GetRequiredService<IConfiguration>()["Sqlite:ConnectionString"])
);
#endif

var app = builder.Build();
app.MapGrpcService<GrpcToDoService>();
app.Run();