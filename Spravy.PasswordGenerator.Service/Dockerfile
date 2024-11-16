FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Spravy.PasswordGenerator.Service/Spravy.PasswordGenerator.Service.csproj", "Spravy.PasswordGenerator.Service/"]
COPY ["Directory.Build.props", "Spravy.PasswordGenerator.Service/Directory.Build.props"]
RUN dotnet restore "Spravy.PasswordGenerator.Service/Spravy.PasswordGenerator.Service.csproj"
COPY . .
WORKDIR "/src/Spravy.PasswordGenerator.Service"
RUN dotnet build "Spravy.PasswordGenerator.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spravy.PasswordGenerator.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spravy.PasswordGenerator.Service.dll"]