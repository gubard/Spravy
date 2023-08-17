FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Spravy.Db.Sqlite.Migrator/Spravy.Db.Sqlite.Migrator.csproj", "Spravy.Db.Sqlite.Migrator/"]
RUN dotnet restore "Spravy.Db.Sqlite.Migrator/Spravy.Db.Sqlite.Migrator.csproj"
COPY . .
WORKDIR "/src/Spravy.Db.Sqlite.Migrator"
RUN dotnet build "Spravy.Db.Sqlite.Migrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spravy.Db.Sqlite.Migrator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spravy.Db.Sqlite.Migrator.dll"]
