FROM mcr.microsoft.com/dotnet/sdk:9.0 AS SDK

COPY ./ /build
WORKDIR /build

RUN dotnet restore
RUN dotnet build --no-restore --configuration Release
RUN dotnet publish --no-restore --no-build --configuration Release -o /publish ./src/Hetzner.DynDNS-Bridge

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS RUN

WORKDIR /app
COPY --from=SDK /publish ./

ENTRYPOINT ["dotnet", "Hetzner.DynDNS-Bridge.dll"]
