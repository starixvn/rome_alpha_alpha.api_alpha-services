FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder
WORKDIR /src

RUN apk add --no-cache bash curl && \
    GLIBC_VER="2.35-r0" && \
    curl -sSL -o /tmp/glibc.apk https://github.com/sgerrand/alpine-pkg-glibc/releases/download/${GLIBC_VER}/glibc-${GLIBC_VER}.apk && \
    curl -sSL -o /tmp/glibc-bin.apk https://github.com/sgerrand/alpine-pkg-glibc/releases/download/${GLIBC_VER}/glibc-bin-${GLIBC_VER}.apk && \
    apk add --no-cache /tmp/glibc.apk /tmp/glibc-bin.apk && \
    rm -f /tmp/*.apk

# Copy csproj and restore as distinct layers
COPY ./src/BuildingBlocks/Common.Logging/Common.Logging.csproj ./BuildingBlocks/Common.Logging/Common.Logging.csproj
COPY ./src/BuildingBlocks/FanexLibs/FanexLibs.Data/FanexLibs.Data.csproj ./BuildingBlocks/FanexLibs/FanexLibs.Data/FanexLibs.Data.csproj
COPY ./src/BuildingBlocks/FanexLibs/FanexLibs.Logging/FanexLibs.Logging.csproj ./BuildingBlocks/FanexLibs/FanexLibs.Logging/FanexLibs.Logging.csproj
COPY ./src/BuildingBlocks/Alpha.PartialUM/Alpha.PartialUM.csproj ./BuildingBlocks/Alpha.PartialUM/Alpha.PartialUM.csproj
COPY ./src/BuildingBlocks/Alpha.Utilities/Alpha.Utilities.csproj ./BuildingBlocks/Alpha.Utilities/
COPY ./src/BuildingBlocks/Alpha.Shared.Services/Alpha.Shared.Services.csproj ./BuildingBlocks/Alpha.Shared.Services/Alpha.Shared.Services.csproj

COPY ./src/Services/Alpha.BetLists.API/Alpha.BetLists.API.csproj ./Services/Alpha.BetLists.API/Alpha.BetLists.API.csproj
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore Services/Alpha.BetLists.API/Alpha.BetLists.API.csproj -v n

# Copy everything else and build
COPY /src/BuildingBlocks/Common.Logging ./BuildingBlocks/Common.Logging
COPY /src/BuildingBlocks/FanexLibs/FanexLibs.Data ./BuildingBlocks/FanexLibs/FanexLibs.Data
COPY /src/BuildingBlocks/FanexLibs/FanexLibs.Logging ./BuildingBlocks/FanexLibs/FanexLibs.Logging
COPY /src/BuildingBlocks/Alpha.PartialUM ./BuildingBlocks/Alpha.PartialUM
COPY /src/BuildingBlocks/Alpha.Utilities ./BuildingBlocks/Alpha.Utilities
COPY /src/BuildingBlocks/Alpha.Shared.Services ./BuildingBlocks/Alpha.Shared.Services
COPY /src/Services/Alpha.BetLists.API ./Services/Alpha.BetLists.API
WORKDIR /src/Services/Alpha.BetLists.API
RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine AS base

RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser

FROM base AS final
WORKDIR /app
COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Alpha.BetLists.API.dll"]
