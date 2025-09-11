FROM hub.nexdev.net/base/dotnet/core/sdk:7.0 AS builder
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ./src/ApiGateways/Alpha.ApiGateways/Alpha.ApiGateways.csproj ./ApiGateways/Alpha.ApiGateways/
COPY ./src/BuildingBlocks/Common.Logging/Common.Logging.csproj ./BuildingBlocks/Common.Logging/
COPY ./src/BuildingBlocks/FanexLibs/FanexLibs.Logging/FanexLibs.Logging.csproj ./BuildingBlocks/FanexLibs/FanexLibs.Logging/
COPY ./src/BuildingBlocks/Alpha.Utilities/Alpha.Utilities.csproj ./BuildingBlocks/Alpha.Utilities/

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore ApiGateways/Alpha.ApiGateways/Alpha.ApiGateways.csproj -v n

# Copy everything else and build
COPY /src/ApiGateways/Alpha.ApiGateways ./ApiGateways/Alpha.ApiGateways
COPY /src/BuildingBlocks/Common.Logging ./BuildingBlocks/Common.Logging
COPY /src/BuildingBlocks/FanexLibs/FanexLibs.Logging ./BuildingBlocks/FanexLibs/FanexLibs.Logging
COPY /src/BuildingBlocks/Alpha.Utilities ./BuildingBlocks/Alpha.Utilities
WORKDIR /src/ApiGateways/Alpha.ApiGateways
RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0-alpine-25h1 AS base

RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser

FROM base AS final
WORKDIR /app
COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Alpha.ApiGateways.dll"]