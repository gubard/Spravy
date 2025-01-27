using System.Text.Json.Serialization;
using Spravy.Core.Extensions;
using Spravy.Core.Services;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Interfaces;
using Spravy.Picture.Db.Contexts;
using Spravy.Picture.Db.Sqlite.Migrator;
using Spravy.Picture.Db.Sqlite.Services;
using Spravy.Picture.Domain.Interfaces;
using Spravy.Picture.Domain.Services;
using Spravy.Picture.Service.Models;
using Spravy.Picture.Service.Services;
using Spravy.Service.Extensions;

namespace Spravy.Picture.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterPicture(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IDbContextSetup, SqlitePictureDbContextSetup>();
        serviceCollection.AddTransient<IPictureService, PictureService>();
        serviceCollection.AddTransient<IPictureEditor, PictureEditor>();
        serviceCollection.AddTransient<IFactory<IPictureEntryService>, PictureEntryServiceFactory>();
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<SqliteFolderOptions>());
        serviceCollection.AddSingleton(sp => sp.GetConfigurationSection<PictureOptions>());
        serviceCollection.AddSingleton<IFactory<string, PictureSpravyDbContext>, SpravyPictureDbContextFactory>();
        serviceCollection.AddSpravySqliteFolderContext<PictureSpravyDbContext, SpravyPictureDbSqliteMigratorMark>();
        serviceCollection.AddTransient<ISerializer, SpravyJsonSerializer>();
        serviceCollection.AddTransient<JsonSerializerContext, SpravyJsonSerializerContext>();
        
        return serviceCollection;
    }
}