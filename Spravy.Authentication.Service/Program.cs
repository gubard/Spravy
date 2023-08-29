using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Spravy.Authentication.Db.Contexts;
using Spravy.Authentication.Db.Core.Profiles;
using Spravy.Authentication.Db.Sqlite.Services;
using Spravy.Authentication.Domain.Core.Profiles;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Service.Helpers;
using Spravy.Authentication.Service.Interfaces;
using Spravy.Authentication.Service.Models;
using Spravy.Authentication.Service.Services;
using Spravy.Db.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Client.Services;
using Spravy.EventBus.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddSingleton(
    _ => new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile<SpravyAuthenticationProfile>();
            cfg.AddProfile<SpravyAuthenticationDbProfile>();
        }
    )
);

builder.Services.AddScoped<IAuthenticationService, EntityFrameworkAuthenticationService>();
builder.Services.AddScoped<IDbContextSetup, SqliteAuthenticationDbContextSetup>();
builder.Services.AddScoped<IHasher, Hasher>();
builder.Services.AddScoped<IFactory<string, IHasher>, HasherFactory>();
builder.Services.AddScoped<IFactory<string, Named<IBytesToString>>, BytesToStringFactory>();
builder.Services.AddScoped<IFactory<string, Named<IStringToBytes>>, StringToBytesFactory>();
builder.Services.AddScoped<IFactory<string, Named<IHashService>>, HashServiceFactory>();
builder.Services.AddScoped<ITokenFactory, JwtTokenFactory>();
builder.Services.AddScoped<JwtSecurityTokenHandler>();
builder.Services.AddSingleton<IEventBusService, GrpcEventBusService>();
builder.Services.AddSingleton<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();

builder.Services.AddSingleton(
    sp => sp.GetRequiredService<IConfiguration>()
        .GetSection("GrpcEventBusService")
        .Get<GrpcEventBusServiceOptions>()
        .ThrowIfNull()
);

builder.Services.AddScoped<JwtTokenFactoryOptions>(
    sp => sp.GetRequiredService<IConfiguration>().GetSection("Jwt").Get<JwtTokenFactoryOptions>().ThrowIfNull()
);

builder.Services.AddScoped<Ref<Named<IBytesToString>>>(
    _ => new Ref<Named<IBytesToString>>(NamedHelper.BytesToUpperCaseHexString)
);

builder.Services.AddScoped<Ref<Named<IHashService>>>(
    _ => new Ref<Named<IHashService>>(NamedHelper.Sha512Hash)
);

builder.Services.AddScoped<Ref<Named<IStringToBytes>>>(
    _ => new Ref<Named<IStringToBytes>>(NamedHelper.StringToUtf8Bytes)
);


builder.Services.AddDbContext<SpravyAuthenticationDbContext>(
    (sp, options) => options.UseSqlite(sp.GetRequiredService<IConfiguration>()["Sqlite:ConnectionString"])
);

builder.Services.AddCors(
    o => o.AddPolicy(
        "AllowAll",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }
    )
);

builder.Services.AddSingleton<IMapper>(sp => new Mapper(sp.GetRequiredService<MapperConfiguration>()));

builder.Host.UseSerilog(
    (_, _, configuration) =>
    {
        configuration.WriteTo.File("/tmp/Spravy/Spravy.Authentication.Service.log");
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
app.MapGrpcService<GrpcAuthenticationService>().EnableGrpcWeb();

#if DEBUG
app.Urls.Clear();
app.Urls.Add("http://localhost:5001");
#endif

app.Run();