FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopieer het .csproj-bestand de container in
COPY ["smarthome/smarthome.csproj", "smarthome/"]
RUN dotnet restore "smarthome/smarthome.csproj"

# Kopieer alle bestanden
COPY . .
WORKDIR "/src/smarthome"
RUN dotnet build "smarthome.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "smarthome.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "smarthome.dll"]
