﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Spravy.Authentication.Service/Spravy.Authentication.Service.csproj", "Spravy.Authentication.Service/"]
RUN dotnet restore "Spravy.Authentication.Service/Spravy.Authentication.Service.csproj"
COPY . .
WORKDIR "/src/Spravy.Authentication.Service"
RUN dotnet build "Spravy.Authentication.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spravy.Authentication.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spravy.Authentication.Service.dll"]
