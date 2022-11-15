FROM hub.nexdev.net/base/dotnet/core/sdk:7.0-alpine AS builder
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY /src/ApiGateways/Alpha.ApiGateways/Alpha.ApiGateways.csproj /src/ApiGateways/Alpha.ApiGateways/
COPY /src/BuildingBlocks/Common.Logging/Common.Logging.csproj /src/BuildingBlocks/Common.Logging/
COPY /src/BuildingBlocks/FanexLibs/FanexLibs.Logging/FanexLibs.Logging.csproj /src/BuildingBlocks/FanexLibs/FanexLibs.Logging/

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore /src/ApiGateways/Alpha.ApiGateways/Alpha.ApiGateways.csproj -v n

# Copy everything else and build
COPY . .
WORKDIR /src/ApiGateways/Alpha.ApiGateways
RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0-alpine as base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser

FROM builder AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Alpha.ApiGateways.dll"]