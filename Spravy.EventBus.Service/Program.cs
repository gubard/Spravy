using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Spravy.Domain.Extensions;
using Spravy.EventBus.Domain.Mapper.Profiles;
using Spravy.EventBus.Service.Interfaces;
using Spravy.EventBus.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddAuthorization();
builder.Services.AddScoped<IEventPusher, EventPusher>();
builder.Services.AddSingleton<IMapper>(sp => new Mapper(sp.GetRequiredService<MapperConfiguration>()));

builder.Services.AddSingleton(
    _ => new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile<SpravyEventBusProfile>();
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

builder.Host.UseSerilog(
    (_, _, configuration) =>
    {
        configuration.WriteTo.File("/tmp/Spravy/Spravy.EventBus.Service.log");
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
app.MapGrpcService<GrpcEventBusService>().EnableGrpcWeb();
;

#if DEBUG
app.Urls.Clear();
app.Urls.Add("http://localhost:5002");
#endif

app.Run();