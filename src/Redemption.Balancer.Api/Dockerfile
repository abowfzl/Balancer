FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

ENV TZ=Asia/Tehran

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY . .

RUN apt-get update && apt-get --yes install curl && update-ca-certificates
RUN curl https://buget.exbito.com/v3/index.json -v

RUN dotnet restore "Redemption.Balancer.Api.csproj" --configfile nuget.config

RUN dotnet build "Redemption.Balancer.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Redemption.Balancer.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app

COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Redemption.Balancer.Api.dll"]