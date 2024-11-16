FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Spravy.Schedule.Service/Spravy.Schedule.Service.csproj", "Spravy.Schedule.Service/"]
COPY ["Directory.Build.props", "Spravy.Schedule.Service/Directory.Build.props"]
RUN dotnet restore "Spravy.Schedule.Service/Spravy.Schedule.Service.csproj"
COPY . .
WORKDIR "/src/Spravy.Schedule.Service"
RUN dotnet build "Spravy.Schedule.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spravy.Schedule.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spravy.Schedule.Service.dll"]
