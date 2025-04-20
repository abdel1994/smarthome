FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# ? Dependencies voor Bluetooth, audio en expect
RUN apt-get update && apt-get install -y \
    bluetooth \
    bluez \
    dbus \
    pulseaudio \
    mplayer \
    expect \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ? Restore en build
COPY ["smarthome/smarthome.csproj", "smarthome/"]
RUN dotnet restore "smarthome/smarthome.csproj"
COPY . .
WORKDIR "/src/smarthome"
RUN dotnet build "smarthome.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "smarthome.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

# ? Voeg publish output toe
COPY --from=publish /app/publish .

# ? Voeg wwwroot toe met Athan.mp3
COPY smarthome/Audio /app/Audio

# ? Voeg expect script toe en maak uitvoerbaar
COPY smarthome/scan.expect /app/scan.expect
COPY smarthome/connect.expect /app/connect.expect
RUN chmod +x /app/scan.expect
RUN chmod +x /app/connect.expect
# ? Voeg bluetooth script toe en maak uitvoerbaar  

# ? Start de app
ENTRYPOINT ["dotnet", "smarthome.dll"]
