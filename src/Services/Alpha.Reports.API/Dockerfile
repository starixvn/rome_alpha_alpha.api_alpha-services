FROM hub.nexdev.net/base/dotnet/core/sdk:7.0 AS builder
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ./src/BuildingBlocks/Common.Logging/Common.Logging.csproj ./BuildingBlocks/Common.Logging/Common.Logging.csproj
COPY ./src/BuildingBlocks/FanexLibs/FanexLibs.Data/FanexLibs.Data.csproj ./BuildingBlocks/FanexLibs/FanexLibs.Data/FanexLibs.Data.csproj
COPY ./src/BuildingBlocks/FanexLibs/FanexLibs.Logging/FanexLibs.Logging.csproj ./BuildingBlocks/FanexLibs/FanexLibs.Logging/FanexLibs.Logging.csproj
COPY ./src/Services/Alpha.Reports.API/Alpha.Reports.API.csproj ./Services/Alpha.Reports.API/Alpha.Reports.API.csproj
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN dotnet restore Services/Alpha.Reports.API/Alpha.Reports.API.csproj -v n

# Copy everything else and build
COPY /src/BuildingBlocks/Common.Logging ./BuildingBlocks/Common.Logging
COPY /src/BuildingBlocks/FanexLibs/FanexLibs.Data ./BuildingBlocks/FanexLibs/FanexLibs.Data
COPY /src/BuildingBlocks/FanexLibs/FanexLibs.Logging ./BuildingBlocks/FanexLibs/FanexLibs.Logging
COPY /src/Services/Alpha.Reports.API ./Services/Alpha.Reports.API
WORKDIR /src/Services/Alpha.Reports.API
RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0 as base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser

FROM base AS final
WORKDIR /app
COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Alpha.Reports.API.dll"]
