using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Spravy.Db.Core.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Db.Core.Profiles;
using Spravy.ToDo.Db.Sqlite.Services;
using Spravy.ToDo.Service.Profiles;
using Spravy.ToDo.Service.Services;
using Spravy.ToDo.Service.Services.Grpcs;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Sqlite.Migrator;
using Spravy.ToDo.Domain.Client.Profiles;
using Spravy.ToDo.Service.HostedServices;
using Spravy.ToDo.Service.Models;

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

builder.Services.AddAuthentication(
        x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    )
    .AddJwtBearer(
        x =>
        {
            var key = builder.Configuration["Jwt:Key"].ThrowIfNull();

            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
            };
        }
    );

builder.Services.AddAuthorization();
builder.Services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<MapperConfiguration>()));
builder.Services.AddScoped<IToDoService, EntityFrameworkToDoService>();
builder.Services.AddSingleton<IDbContextSetup, SqliteToDoDbContextSetup>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<MigratorHostedService>();
builder.Services.AddSingleton<IFactory<string, SpravyToDoDbContext>, SpravyToDoDbContextFactory>();

builder.Services.AddSingleton(
    sp => sp.GetRequiredService<IConfiguration>().GetSection(SqliteOptions.Section).Get<SqliteOptions>().ThrowIfNull()
);

builder.Services.AddDbContextFactory<SpravyToDoDbContext>(
    (sp, options) =>
    {
        var sqliteOptions = sp.GetRequiredService<SqliteOptions>();
        var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
        var claims = httpContextAccessor.HttpContext.ThrowIfNull().User.Claims;
        var nameIdentifier = claims.Single(x => x.Type == ClaimTypes.NameIdentifier);
        var fileName = $"{nameIdentifier.Value}.db";
        var connectionString = $"DataSource={Path.Combine(sqliteOptions.DataBasesFolder, fileName)}";
        options.UseSqlite(connectionString, b => b.MigrationsAssembly(SpravyToDoDbSqliteMigratorMark.AssemblyFullName));
    }
);

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

app.UseAuthentication();
app.UseAuthorization();

#if DEBUG
app.Urls.Clear();
app.Urls.Add("http://localhost:5000");
#endif

app.MapGrpcService<GrpcToDoService>().EnableGrpcWeb();
app.Run();