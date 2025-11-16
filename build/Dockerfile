FROM mcr.microsoft.com/dotnet/sdk:10.0 AS SDK

WORKDIR /build
COPY ./ ./

RUN dotnet restore
RUN dotnet build --no-restore --configuration Release
RUN dotnet publish --no-restore --no-build --configuration Release -o /publish ./src/DynDNS.Host

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS RUN

WORKDIR /app
COPY --from=SDK /publish ./

ENTRYPOINT ["dotnet", "DynDNS.Host.dll"]
