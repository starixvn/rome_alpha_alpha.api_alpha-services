FROM hub.nexdev.net/base/dotnet/core/sdk:6.0 AS builder
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ./src/Alpha.Identity.API/Alpha.Identity.API.csproj ./Alpha.Identity.API/Alpha.Identity.API.csproj
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN dotnet restore Alpha.Identity.API/Alpha.Identity.API.csproj -v n

# Copy everything else and build
COPY /src/Alpha.Identity.API ./Alpha.Identity.API
WORKDIR /src/Alpha.Identity.API
RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0 as base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser

WORKDIR /app
COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Alpha.Identity.API.dll"]
