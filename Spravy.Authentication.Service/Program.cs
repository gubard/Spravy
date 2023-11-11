using Spravy.Authentication.Service;
using Spravy.Authentication.Service.Extensions;
using Spravy.Authentication.Service.Services;
using Spravy.Service.Extensions;

WebApplication.CreateBuilder(args)
    .BuildSpravy<GrpcAuthenticationService, SpravyAuthenticationServiceMark>(args, x => x.RegisterAuthentication())
    .Run();