FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Kopieer alleen de .csproj eerst
COPY *.csproj ./
RUN dotnet restore

# Kopieer nu alle projectbestanden
COPY SmartFront.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish-client


#runtime 
FROM nginx:alpine
COPY --from=build /app/publish-client/wwwroot /usr/share/nginx/html
EXPOSE 80
