FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

ENV TZ=Asia/Tehran

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "Redemption.Balancer.Api.csproj" --configfile nuget.config

RUN dotnet build "Redemption.Balancer.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Redemption.Balancer.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app

RUN apt-get update 
RUN apt-get --yes install curl

COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Redemption.Balancer.Api.dll"]