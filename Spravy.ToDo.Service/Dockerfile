FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Spravy.ToDo.Service/Spravy.Service.csproj", "Spravy.ToDo.Service/"]
COPY ["Directory.Build.props", "Spravy.ToDo.Service/Directory.Build.props"]
RUN dotnet restore "Spravy.ToDo.Service/Spravy.Service.csproj"
COPY . .
WORKDIR "/src/Spravy.ToDo.Service"
RUN dotnet build "Spravy.ToDo.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spravy.ToDo.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spravy.ToDo.Service.dll"]
