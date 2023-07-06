#runtime for framework dependant
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine3.18  AS base   
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine3.18 AS build
WORKDIR /app

COPY SignalRChat.csproj ./

RUN dotnet restore SignalRChat.csproj --no-cache

COPY . ./

RUN dotnet publish SignalRChat.csproj -c Release -o out --no-restore -p:UseAppHost=false

FROM base as fina
WORKDIR /app
COPY --from=build /app/out .

# Configure ASP.NET Core
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production


ENTRYPOINT ["dotnet", "SignalRChat.dll"]
