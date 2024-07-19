FROM hub.nexdev.net/base/dotnet/core/sdk:7.0 AS builder
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ./src/BuildingBlocks/Common.Logging/Common.Logging.csproj ./BuildingBlocks/Common.Logging/Common.Logging.csproj
COPY ./src/BuildingBlocks/FanexLibs/FanexLibs.Data/FanexLibs.Data.csproj ./BuildingBlocks/FanexLibs/FanexLibs.Data/FanexLibs.Data.csproj
COPY ./src/BuildingBlocks/FanexLibs/FanexLibs.Logging/FanexLibs.Logging.csproj ./BuildingBlocks/FanexLibs/FanexLibs.Logging/FanexLibs.Logging.csproj
COPY ./src/BuildingBlocks/Alpha.Utilities/Alpha.Utilities.csproj ./BuildingBlocks/Alpha.Utilities/
COPY ./src/Services/Alpha.ApiKey.API/Alpha.ApiKey.API.csproj ./Services/Alpha.ApiKey.API/Alpha.ApiKey.API.csproj
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore Services/Alpha.ApiKey.API/Alpha.ApiKey.API.csproj -v n

# Copy everything else and build
COPY /src/BuildingBlocks/Common.Logging ./BuildingBlocks/Common.Logging
COPY /src/BuildingBlocks/FanexLibs/FanexLibs.Data ./BuildingBlocks/FanexLibs/FanexLibs.Data
COPY /src/BuildingBlocks/FanexLibs/FanexLibs.Logging ./BuildingBlocks/FanexLibs/FanexLibs.Logging
COPY /src/BuildingBlocks/Alpha.Utilities ./BuildingBlocks/Alpha.Utilities
COPY /src/Services/Alpha.ApiKey.API ./Services/Alpha.ApiKey.API
WORKDIR /src/Services/Alpha.ApiKey.API
RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0-alpine-25h1 as base

RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser

FROM base AS final
WORKDIR /app
COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Alpha.ApiKey.API.dll"]
