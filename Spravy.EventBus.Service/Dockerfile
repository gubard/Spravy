﻿FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Spravy.EventBus.Service/Spravy.EventBus.Service.csproj", "Spravy.EventBus.Service/"]
COPY ["Directory.Build.props", "Spravy.EventBus.Service/Directory.Build.props"]
RUN dotnet restore "Spravy.EventBus.Service/Spravy.EventBus.Service.csproj"
COPY . .
WORKDIR "/src/Spravy.EventBus.Service"
RUN dotnet build "Spravy.EventBus.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spravy.EventBus.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spravy.EventBus.Service.dll"]
